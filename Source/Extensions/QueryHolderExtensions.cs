using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGAutoSell.Query;
using RimWorld;
using TD_Find_Lib;

namespace MGAutoSell.Extensions
{
    public static class QueryHolderExtensions
    {
        public static bool AppliesTo(this QueryHolder holder, ITradeQuery[] queries, Tradeable tradeable, TradeAction action)
        {
            return !holder.MatchAllQueries
                ? queries.AnyX(x => (x as ThingQuery)?.Enabled is not false && x.AppliesDirectlyTo(tradeable, action),
                    holder.Children.anyMin)
                : queries.All(x => x is ThingQuery { Enabled: false } || x.AppliesDirectlyTo(tradeable, action));
        }
    }
}
