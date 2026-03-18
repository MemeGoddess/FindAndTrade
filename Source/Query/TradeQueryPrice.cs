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
    public class TradeQueryPrice : ThingQueryDropDown<PriceType>, ITradeQuery
    {
        public bool AppliesDirectlyTo(Tradeable tradeable, TradeAction action)
        {
            return tradeable.PriceTypeFor(action) == sel;
        }

        public override string NameFor(PriceType o)
        {
            return o.ToString().SplitCamelCase();
        }

        public override bool AppliesDirectlyTo(Thing thing)
        {
            return true;
        }
    }
}
