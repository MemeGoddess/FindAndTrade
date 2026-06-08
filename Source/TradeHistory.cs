using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace MGAutoSell
{
    public class TradeHistory : IExposable
    {
        public string TraderName;
        public Faction Faction;

        public List<TradedItem> Sold = new();
        public List<TradedItem> Bought = new();

        public TradeHistory()
        {
        }

        public TradeHistory(ITrader trader, Faction faction, List<TradedItem> buy, List<TradedItem> sell)
        {
            TraderName = trader.TraderName ?? faction.Name;
            Faction = faction;
            Sold = sell;
            Bought = buy;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref TraderName, nameof(TraderName));
            Scribe_References.Look(ref Faction, nameof(Faction));
            Scribe_Collections.Look(ref Sold, nameof(Sold), LookMode.Deep);
            Scribe_Collections.Look(ref Bought, nameof(Bought), LookMode.Deep);
            Sold ??= [];
        }
    }

    public class TradedItem : IExposable
    {
        public ThingDef ThingDef;
        public float PricePer;
        public int Count;

        public TradedItem()
        {
        }

        public TradedItem(ThingDef thingDef, float pricePer, int count)
        {
            ThingDef = thingDef;
            PricePer = pricePer;
            Count = count;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref ThingDef, nameof(ThingDef));
            Scribe_Values.Look(ref PricePer, nameof(PricePer));
            Scribe_Values.Look(ref Count, nameof(Count));
        }
    }
}
