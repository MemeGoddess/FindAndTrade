using System;
using System.Collections.Generic;
using System.Linq;
using MGAutoSell.Caravans;
using MGAutoSell.Filter;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace MGAutoSell
{
    public record SettlementToTradeWith(float Distance, float Silver, float FuelRequirement);
    public class TradeRulesGameComp : GameComponent
    {
        public TradeRulesGroup tradeRules = new();
        public HashSet<ITrader> traders = new();

        public bool autoTrade = false;
        public HashSet<int> autoTraderIDs = new();

        public Dictionary<Map, ItemsToSell> SellCache = [];
        public Dictionary<Map, Pawn> SellerOverride = [];
        private Queue<Map> cacheOrder = new();
        private int tickWait => 3600 / UnityEngine.Mathf.Max(SellCache.Count, 1);
        private int nextTick = -1;

        public TradeRulesGameComp(Game game)
        {
            tradeRules ??= new();
            traders ??= new();
        }

        public override void FinalizeInit()
        {
            LongEventHandler.ExecuteWhenFinished(() => tradeRules.GetPossibleItemsList([]));
            LongEventHandler.ExecuteWhenFinished(() =>
            {
                foreach (var map in Find.Maps)
                {
                    var cache = CacheUtility.Cache(this, map, out _);
                    SellCache[map] = cache;
                    cacheOrder.Enqueue(map);
                }
            });
        }

        public ItemsToSell Fetch(Map map, bool refresh = false)
        {
            if (!refresh && SellCache.TryGetValue(map, out var cache))
                return cache;

            cache = CacheUtility.Cache(this, map, out _);
            SellCache[map] = cache;
            return cache;
        }

        public override void GameComponentTick()
        {
            if (nextTick > Find.TickManager.TicksGame)
                return;

            nextTick = Find.TickManager.TicksGame + tickWait;

            if (!cacheOrder.Any())
                return;

            var map = cacheOrder.Dequeue();

            if (map == null)
                return;

            try
            {
                SellerOverride.TryGetValue(map, out var seller);
                var cache = CacheUtility.Cache(this, map, out _, seller);
                SellCache[map] = cache;

                var comp = Find.World.GetComponent<SettlementTracker>();
                var shuttle = comp.GetBestForMap(map);

                if (shuttle != null && shuttle.FuelLevel >= shuttle.LaunchableComp.Props.minFuelCost)
                {
                    var total = cache.TotalSilver.Value;
                    var fuelWithBuffer = shuttle.FuelLevel - 5;

                    var distances = comp.GetDistances(map, maxDistance: layer =>
                    {
                        var maxDistance = shuttle.LaunchableComp.MaxLaunchDistanceAtFuelLevel(shuttle.FuelLevel, layer);
                        if (shuttle.LaunchableComp.Props.fixedLaunchDistanceMax >= 0)
                            maxDistance = Mathf.Min(maxDistance, shuttle.LaunchableComp.Props.fixedLaunchDistanceMax);
                        return maxDistance;
                    });


                }

            }
            finally
            {
                cacheOrder.Enqueue(map);
            }
        }

        private Settlement GetTradeRoute(Building_PassengerShuttle shuttle, ItemsToSell cache, SettlementTracker comp)
        {
            if (shuttle == null)
                return null;
            var minFuel = shuttle.LaunchableComp.Props.minFuelCost;

            if (shuttle.FuelLevel < minFuel + 5)
                return null;

            var total = cache.TotalSilver.Value;
            if (total < shuttle.LaunchableComp.Props.minFuelCost * 4)
                return null;

            var fuelWithBuffer = shuttle.FuelLevel - 5;

            var distances = comp.GetDistances(shuttle.Map,
                maxDistance: layer =>
                {
                    var maxDistance = shuttle.LaunchableComp.MaxLaunchDistanceAtFuelLevel(fuelWithBuffer, layer);
                    if (shuttle.LaunchableComp.Props.fixedLaunchDistanceMax >= 0)
                        maxDistance = Mathf.Min(maxDistance, shuttle.LaunchableComp.Props.fixedLaunchDistanceMax);
                    return maxDistance;
                },
                settlementPredicate: settlement => settlement.TradeCurrency == TradeCurrency.Favor || settlement.trader.StockListForReading.FirstOrDefault(x => x.def == ThingDefOf.Silver)?.stackCount < 200);

            // Ok, what would they actually buy?

            var sales =
                distances.ToDictionary(x => x.Key, x =>
                {
                    var total = cache.Items.Sum(y => x.Key.trader.TraderKind.WillTrade(y.Item) ? y.Total.Value : 0);
                    var traderSilver = x.Key.trader.StockListForReading.FirstOrDefault(x => x.def == ThingDefOf.Silver)
                        ?.stackCount ?? 0f;
                    return total > traderSilver ? traderSilver : total;
                });

            var fuelCost = distances.ToDictionary(x => x.Key,
                x => shuttle.LaunchableComp.FuelNeededToLaunchAtDist(x.Value, x.Key.Tile.Layer));
            
            // Find best


            return null;
        }

        public override void ExposeData()
        {
            Scribe_Deep.Look(ref tradeRules, nameof(tradeRules));
            Scribe_Collections.Look(ref traders, nameof(traders), LookMode.Reference);

            Scribe_Values.Look(ref autoTrade, nameof(autoTrade));
            Scribe_Collections.Look(ref autoTraderIDs, nameof(autoTraderIDs));

            tradeRules ??= new();
            traders ??= new();
            autoTraderIDs ??= new();
        }
    }
}
