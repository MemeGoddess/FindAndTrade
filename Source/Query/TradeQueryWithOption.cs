using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using TD_Find_Lib;
using Verse;

// ReSharper disable once CheckNamespace
namespace MGAutoSell.Query
{
    public abstract class TradeQueryWithOption<T> : ThingQueryWithOption<T>, ITradeQuery
    {
        public abstract bool AppliesDirectlyTo(Tradeable tradeable);
    }
}
