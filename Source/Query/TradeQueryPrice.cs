using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace MGAutoSell.Query
{
    public class TradeQueryPrice : TradeQueryWithOption<PriceType>
    {
        public override bool AppliesDirectlyTo(Tradeable tradeable)
        {
            tradeable.PriceTypeFor()
        }

        public override bool AppliesDirectlyTo(Thing thing)
        {
            return true;
        }
    }
}
