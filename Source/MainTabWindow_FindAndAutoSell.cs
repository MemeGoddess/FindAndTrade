using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using TD_Find_Lib;
using UnityEngine;
using Verse;

namespace MGAutoSell
{
    public class MainTabWindow_FindAndAutoSell : MainTabWindow
    {
        private TradeRulesListDrawer drawer;
        private Vector2 _scroll = Vector2.zero;

        private TradeRulesGameComp comp;

        public MainTabWindow_FindAndAutoSell()
        {
            preventCameraMotion = false;
            doCloseX = true;
        }

        public override void PreOpen()
        {
            base.PreOpen();
            comp = Current.Game.GetComponent<TradeRulesGameComp>();
            drawer = new TradeRulesListDrawer(comp.tradeRules, this);
        }

        public override void Close(bool doCloseSound = true)
        {
            editor?.PostClose();
            editor = null;
            SelectedTradeRule = null;
            base.Close(doCloseSound);
        }

        Vector2 listerScroll = Vector2.zero;
        Vector2 BuyingSize = Text.CalcSize("Buying"), SellingSize = Text.CalcSize("Selling");
        private long previousRenderTime = 0;
        public override void DoWindowContents(Rect inRect)
        {
            var timestamp = Stopwatch.GetTimestamp();

            var listing = new Listing_Standard();
            listing.Begin(inRect);

            var drawerListing = new Listing_StandardIndent();
            var height = 300f;

            var topRect = listing.GetRect(height);

            var controlsRect = topRect.LeftHalf().BottomPartPixels(Text.LineHeight);
            var controls = new WidgetRow(controlsRect.x, controlsRect.y);
            if (controls.ButtonIcon(FindTex.GreyPlus))
                CreateRule();

            var rect = topRect.LeftHalf();
            var header = rect.TopPartPixels(30).LeftPartPixels(rect.width - 16);
            var body = rect.MiddlePartPixels(rect.width, rect.height - 60);
            Widgets.Label(header.LeftHalf(), $"Find and (Auto) Sell ({previousRenderTime}ts)");

            var middle = 30;
            var left = header.RightHalf().LeftHalf();
            left.x += middle - (SellingSize.x / 2);
            Widgets.Label(left, "Selling");
            Widgets.Label(header.RightPartPixels(middle + BuyingSize.x / 2), "Buying");

            drawerListing.BeginScrollView(body, ref listerScroll, body.LeftPartPixels(body.width - 16).TopPartPixels(comp.tradeRules.Count * 30).AtZero());
            drawer.DrawQuerySearchList(drawerListing);
            drawerListing.EndScrollView(ref height);
            listing.GapLine();

            if (editor != null)
            {
                var editRect = topRect.RightHalf();
                
                editor.DoWindowContents(editRect);
            }
            listing.End();
            previousRenderTime = Stopwatch.GetTimestamp() - timestamp;
        }

        public TradeRule SelectedTradeRule;
        public void DoEdit(TradeRule tradeRule)
        {
            editor?.PostClose();

            if (SelectedTradeRule == tradeRule)
            {
                editor = null;
                SelectedTradeRule = null;
            }
            else
            {
                editor = new TradeRuleEditor(tradeRule);
                SelectedTradeRule = tradeRule;
            }
        }

        public void CreateRule()
        {
            Find.WindowStack.Add(new Dialog_Name("TD.NewAlert".Translate(), n =>
                {
                    TradeRule tradeRule = new(n);
                    comp.tradeRules.Add(tradeRule);

                    EditRule(tradeRule);
                },
                "TD.NameForNewAlert".Translate(),
                name => comp.tradeRules.Any(x => name == x.Search.name)));
        }

        private TradeRuleEditor editor;
        public void EditRule(TradeRule tradeRule)
        {
            editor = new TradeRuleEditor(tradeRule);

            //Find.WindowStack.Add(editor);
            //editor.windowRect.x = Window.StandardMargin;
            //editor.windowRect.y = windowRect.yMin / 3;
            //editor.windowRect.yMax = windowRect.yMin;
        }
    }
}
