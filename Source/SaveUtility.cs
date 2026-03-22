using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MGAutoSell.Filter;
using TD_Find_Lib;
using Verse;

namespace MGAutoSell
{
    [HarmonyPatch(typeof(GameDataSaveLoader), nameof(GameDataSaveLoader.LoadGame), [typeof(string)])]
    public static class SaveUtility
    {
        public static List<SearchGroup> CustomAlertsList = new List<SearchGroup>();

        public static void Postfix(string saveFileName)
        {
            LongEventHandler.QueueLongEvent(() =>
            {
                var stopwatch = Stopwatch.StartNew();
                CustomAlertsList = GenFilePaths.AllSavedGameFiles.Where(x =>
                    Path.GetFileNameWithoutExtension(x.Name) != saveFileName && !x.Name.StartsWith("Autosave")).Take(50).Select(x =>
                    {
                        var path = Path.GetFileNameWithoutExtension(x.FullName);

                        var comp = GetCompFromSave(path);
                        if (comp == null)
                            return null;
                        var group = new TradeRulesGroup(Path.GetFileNameWithoutExtension(x.Name), null);
                        group.AddRange(comp.Select(x => x.search));

                        return group;
                    }).Where(x => x != null).ToList();
                stopwatch.Stop();
                Log.Message($"Took {stopwatch.Elapsed.TotalSeconds} seconds to load saves");
            }, "LoadingSaves", false, null);
        }

        internal static TradeRulesGroup GetCompFromSave(string path)
        {
            var search = GrabNode(path);

            if (search == string.Empty)
                return null;

            var cleanedMap = Regex.Replace(search, "<searchMaps>.*</searchMaps>", "<searchMaps />",
                RegexOptions.Singleline);
            var cleanedMapType = Regex.Replace(cleanedMap, "<mapType>\\w*</mapType>", "");
            var tempDoc =
                $@"<TD_Find_Lib.QuerySearch>
<saveable Class=""MGAutoSell.Filter.TradeRulesGroup"">
{cleanedMapType}
</saveable>
</TD_Find_Lib.QuerySearch>";

            var newGroup = ScribeXmlFromString.LoadFromString<TradeRulesGroup>(tempDoc);
            return newGroup;
        }

        public static string GrabNode(string saveNameNoExt)
        {
            var path = GenFilePaths.FilePathForSavedGame(saveNameNoExt);
            var text = File.OpenText(path);
            var capture = false;

            var stringBuilder = new StringBuilder();
            while (!text.EndOfStream)
            {
                var line = text.ReadLine();
                if (line.Contains("<components>"))
                    capture = true;
                else if (capture && line.Contains("</components>"))
                    break;
                if (capture)
                    stringBuilder.Append(line);
            }

            var filtered = stringBuilder.ToString();
            var regex = Regex.Match(filtered, "<li Class=\"MGAutoSell.TradeRulesGameComp\">.*<tradeRules>(.*)<\\/tradeRules>.*?<\\/li>",
                RegexOptions.Singleline);

            if (!regex.Success)
                return string.Empty;
            var matches = Regex.Matches(regex.Groups[1].Value, "<li>.*?<search>(.*?)</search>.*?</li>", RegexOptions.Singleline);
            return regex.Groups[1].Value;
        }
    }
}
