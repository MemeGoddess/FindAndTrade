using System;
using System.Collections.Generic;
using System.Linq;
using MGAutoSell.Filter;
using UnityEngine;
using Verse;

namespace MGAutoSell.Records;

public record ItemsToSell(
    List<SellRecord> Items,
    List<PotentialItem> PotentialItems,

    ItemAndLabel<float> TotalSilver,
    TraderRecord Trader,
    Dictionary<TradeRule, RuleUsage> Rules);

public record SellRecord(ThingDef Item, int Count, ItemAndLabel<float> Total, ItemAndLabel<float> Price);

public record TraderRecord(Pawn Pawn, string Name, Func<Texture> Icon, string ImprovementLabel, float Improvement, bool IsLeader);

public record RuleRecord(ThingDef Item, int Count);

public record PotentialItem(ThingDef Item, string Rule);
public record ItemAndLabel<T>(T Value, string Label);

public record RuleUsage
{
    public RuleUsage(List<RuleRecord> Items, (ItemAndLabel<int> min, ItemAndLabel<int> max) Range)
    {
        this.Items = Items;
        this.Range = Range;
        ItemsList = Items.Select(x => $"{x.Item.LabelCap} x{x.Count}").ToLineList("- ");
    }

    public List<RuleRecord> Items { get; init; }
    public (ItemAndLabel<int> min, ItemAndLabel<int> max) Range { get; init; }
    public string ItemsList { get; init; }

    public void Deconstruct(out List<RuleRecord> Items, out (ItemAndLabel<int> min, ItemAndLabel<int> max) Range)
    {
        Items = this.Items;
        Range = this.Range;
    }
}