using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using TD_Find_Lib;
using Verse;

namespace MGAutoSell.Query
{
    public class TradeQueryPrice : ThingQueryIntRange, ITradeQuery
    {
        public override int Min => (int)PriceType.VeryCheap;
        public override int Max => (int)PriceType.Exorbitant;
        //public override Func<int, string> Writer => x => "PriceType"+((PriceType)x);

        public bool AppliesDirectlyTo(Tradeable tradeable, TradeAction action)
        {
            var priceType = tradeable.PriceTypeFor(action);
            if (priceType == PriceType.Undefined)
                return false;
            return sel.Includes((byte)tradeable.PriceTypeFor(action));
        }

        public override bool AppliesDirectlyTo(Thing thing)
        {
            return true;
        }
    }
}
