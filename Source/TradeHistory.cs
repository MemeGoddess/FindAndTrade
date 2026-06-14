using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace MGAutoSell
{
    public class TradeHistory : IExposable
    {
        public string TraderName;
        public Faction Faction;
        public int Tick;

        public List<TradedItem> Sold = new();
        public List<TradedItem> Bought = new();
        public float Silver;

        [CanBeNull] private string silverLabel;
        public string SilverLabel => silverLabel ??= Mathf.Abs(Silver).ToStringMoney().Colorize(Silver >= 0 ? ColoredText.FactionColor_Ally : ColoredText.FactionColor_Hostile);

        private float? silverSize;
        public float SilverSize => silverSize ??= Text.CalcSize(SilverLabel).x;
        public TradeHistory()
        {
        }

        public TradeHistory(ITrader trader, Faction faction, float silver, List<TradedItem> buy, List<TradedItem> sell)
        {
            TraderName = trader is TradeShip ship ? ship.def.LabelCap : trader.TraderName ?? faction?.Name ?? "Someone";
            Faction = faction;
            Silver = silver;
            Sold = sell;
            Bought = buy;
            Tick = Find.TickManager.TicksGame;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref TraderName, nameof(TraderName));
            Scribe_References.Look(ref Faction, nameof(Faction));
            Scribe_Values.Look(ref Tick, nameof(Tick));
            Scribe_Values.Look(ref Silver, nameof(Silver));
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
