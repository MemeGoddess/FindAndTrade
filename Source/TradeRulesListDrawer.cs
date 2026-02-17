using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TD_Find_Lib;
using UnityEngine;
using Verse;

namespace MGAutoSell
{
    public class TradeRulesListDrawer : SearchGroupDrawerBase<TradeRulesGroup,TradeRule>
    {
        private readonly MainTabWindow_FindAndAutoSell _parent;
        private TradeRulesGameComp comp;

        private Color altBackground = new(0.3f, 0.3f, 0.3f, 0.5f);

        public TradeRulesListDrawer(TradeRulesGroup list, MainTabWindow_FindAndAutoSell parent) : base(list)
        {
            _parent = parent;
            comp = Current.Game.GetComponent<TradeRulesGameComp>();
        }

        private string editThiSearch = "TD.EditThisSearch".Translate();
        public override void DrawRowButtons(WidgetRow row, TradeRule item, int i)
        {
            if(row.Checkbox(ref item.Enabled))
                item.search.Changed();

            if (row.ButtonIcon(FindTex.Edit, editThiSearch))
                _parent.DoEdit(item);

            if (row.ButtonIcon(FindTex.Trash))
                comp.tradeRules.Remove(item);
        }

        public override void DrawExtraRowRect(Rect rowRect, TradeRule item, int i)
        {
            var color = GUI.color;
            var fadedColor = new Color(1, 1, 1, 0.4f);

            if (item == _parent.SelectedTradeRule)
                Widgets.DrawHighlightSelected(rowRect);

            var rowSell = new WidgetRow(rowRect.xMax, rowRect.y, UIDirection.LeftThenDown);

            var alignment = Text.CurTextFieldStyle.alignment;
            Text.CurTextFieldStyle.alignment = TextAnchor.MiddleCenter;

            var sellDownToRect = rowSell.GetRect(60);
            sellDownToRect.height -= 4;
            sellDownToRect.y += 3;

            var prevSellDown = item.SellDownTo;
            string sellDownToBuffer = null;
            if (!item.AllowSell)
                GUI.color = fadedColor;
            Widgets.TextFieldNumeric(sellDownToRect, ref item.SellDownTo, ref sellDownToBuffer);
            if (string.IsNullOrWhiteSpace(sellDownToBuffer))
                item.SellDownTo = 0;
            GUI.color = color;
            if (item.SellDownTo != prevSellDown)
                item.search.changed = true;

            var rowBuyRect = rowRect.RightHalf();
            rowBuyRect = rowBuyRect.RightPartPixels(rowBuyRect.width - 20);

            if ((_parent.sellCache?.Rules?.TryGetValue(item, out var entry) ?? false) && entry.Any())
            {
                var iconRect = rowBuyRect.MiddlePartPixels(Text.LineHeight, Text.LineHeight);
                GUI.color = fadedColor;
                var length = entry.Count;
                var index = (int)(DateTimeOffset.Now.ToUnixTimeSeconds() % length);
                GUI.DrawTexture(iconRect, entry[index].Item.uiIcon);
                GUI.color = color;
            }

            var rowBuy = new WidgetRow(rowBuyRect.x, rowBuyRect.y, UIDirection.RightThenDown);

            var buyUpToRect = rowBuy.GetRect(60);
            buyUpToRect.height -= 4;
            buyUpToRect.y += 3;

            string buyUpToBuffer = null;
            if (!item.AllowBuy)
            {
                GUI.color = fadedColor;
            }
            Widgets.TextFieldNumeric(buyUpToRect, ref item.BuyUpTo, ref buyUpToBuffer);
            if(string.IsNullOrWhiteSpace(buyUpToBuffer))
                item.BuyUpTo = 0;
            GUI.color = color;
            Text.CurTextFieldStyle.alignment = alignment;
        }

        public override string Name => "TD.ActiveSearches".Translate();
    }
}
