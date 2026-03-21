using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using TD_Find_Lib;
using UnityEngine;
using Verse;

namespace MGAutoSell.Query
{
    public class TradeQueryPrice : ThingQueryWithOption<PriceTypeRange>, ITradeQuery
    {
        public TradeQueryPrice()
        {
            sel = new PriceTypeRange(PriceType.Normal, PriceType.Normal);
        }

        public bool AppliesDirectlyTo(Tradeable tradeable, TradeAction action)
        {
            var priceType = tradeable.PriceTypeFor(action);
            return priceType != PriceType.Undefined && sel.Includes(tradeable.PriceTypeFor(action));
        }

        public override bool AppliesDirectlyTo(Thing thing)
        {
            return true;
        }

        protected override bool DrawMain(Rect rect, bool locked, Rect fullRect)
        {
            base.DrawMain(rect, locked, fullRect);
            return PriceTypeRange.Widget((fullRect.RightHalfClamped(row.FinalX)), this.id, ref _sel);
        }
    }
}
