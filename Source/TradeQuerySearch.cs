using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGAutoSell.Query;
using RimWorld;
using TD_Find_Lib;
using Verse;

namespace MGAutoSell
{
    public class TradeQuerySearch : QuerySearch
    {
        public List<ThingDef> AllItems;
        public ITradeQuery[] TradeQueries = [];

        public TradeQuerySearch()
        {
            TradeQueries = children.queries
                .OfType<ITradeQuery>()
                .ToArray();
        }

        public override void Changed()
        {
            base.Changed();
            AllItems = this.GetPossibleItems();
            TradeQueries = children.queries
                .OfType<ITradeQuery>()
                .ToArray();
        }

        public bool AppliesTo(Tradeable tradeable, TradeAction action)
        {
            return !MatchAllQueries
                ? TradeQueries.AnyX(x => (x as ThingQuery)?.Enabled is not false && x.AppliesDirectlyTo(tradeable, action),
                    Children.anyMin)
                : TradeQueries.All(x => x is ThingQuery { Enabled: false } || x.AppliesDirectlyTo(tradeable, action));
        }
    }
}
