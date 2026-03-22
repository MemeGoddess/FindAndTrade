using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD_Find_Lib;
using Verse;

namespace MGAutoSell
{
    public class TradeQuerySearch : QuerySearch
    {
        public List<ThingDef> AllItems;
        public override void Changed()
        {
            base.Changed();
            AllItems = this.GetPossibleItems();
        }


    }
}
