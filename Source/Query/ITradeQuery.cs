using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGAutoSell.Query
{
    public interface ITradeQuery
    {
        public bool AppliesDirectlyTo(Tradeable tradeable);

    }
}
