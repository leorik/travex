namespace TravEx.Model;

public sealed class Exchange
{
    private readonly ExchangeState _state;

    private readonly Dice _dice;

    public Exchange(ExchangeState state)
    {
        _state = state;
        _dice = new Dice(state.RngSeed);
    }

    public IReadOnlyCollection<Listing> GetMarketListings(IReadOnlyCollection<Ware> allWares, IReadOnlyList<double> buyModifiers,
        IReadOnlyList<double> sellModifiers)
    {
        var sellingWareHits = GetSellingWares(allWares).GroupBy(w => w).ToDictionary(g => g.Key, g => g.Count());

        var listings = new List<Listing>(allWares.Count);

        foreach (var ware in allWares)
        {
            var sellDiscount = CalculateDiscount(ware, sellModifiers, true);
            var sellingPrice = Convert.ToInt32(ware.BasePrice * sellDiscount);
            var listing = new Listing(ware.Name, null, null, sellingPrice, sellDiscount, null);

            if (sellingWareHits.TryGetValue(ware.Name, out var hits))
            {
                var buyDiscount = CalculateDiscount(ware, buyModifiers, false);
                var buyPrice = Convert.ToInt32(ware.BasePrice * buyDiscount);

                var tonnage = 0;
                for (var i = 0; i < hits; i++)
                {
                    tonnage += GetAvailableTonnage(ware);
                }
                
                listing = listing with
                {
                    BuyPrice = buyPrice,
                    BuyPriceFactor = buyDiscount,
                    AvailableTonnage = tonnage
                };
            }

            listings.Add(listing);
        }

        return listings;
    }

    private IEnumerable<string> GetSellingWares(IReadOnlyCollection<Ware> allWares)
    {
        var availableWares = new List<string>();

        List<Ware> rollingWares;
        if (!_state.IsBlackMarket)
        {
            availableWares.AddRange(
                allWares.Where(w => w.IsLegal(_state.LocalLawLevel) && MatchesTradeCode(w)).
                         Select(w => w.Name));

            rollingWares = allWares.Where(w => w.IsLegal(_state.LocalLawLevel) || _state.IsBlackMarket).ToList();
        }
        else
        {
            availableWares.AddRange(
                allWares.Where(w => !w.IsLegal(_state.LocalLawLevel) && MatchesTradeCode(w) && !w.IsUniversallyIllegal).Select(w => w.Name));

            rollingWares = allWares.Where(w => w.IsUniversallyIllegal).ToList();
        }

        if (rollingWares.Any())
        {
            for (var i = 0; i < _state.WorldPopulation; i++)
            {
                availableWares.Add(rollingWares[_dice.RollInRange(rollingWares.Count - 1)].Name);
            }
        }

        return availableWares;
    }

    private int GetAvailableTonnage(Ware ware)
    {
        const int LowPopWorldThreshold = 3;
        const int LowPopWorldAvailabilityMod = -3;
        const int HighPopWorldThreshold = 9;
        const int HighPopAvailabilityMod = 3;

        var availabilityMod = _state.WorldPopulation switch
        {
            <= LowPopWorldThreshold => LowPopWorldAvailabilityMod,
            >= HighPopWorldThreshold => HighPopAvailabilityMod,
            _ => 0
        };

        return int.Max(0, _dice.RollD6s(ware.TonnageDiceCount) + availabilityMod) * ware.TonnageMultiplier;
    }

    private double CalculateDiscount(Ware ware, IReadOnlyList<double> modifiers, bool isSelling)
    {
        const int ModTableBaseline = 3;
        
        var brokerSkillMod = _state.PlayerBrokerSkill - _state.ContractorBrokerSkill;
        var supplyMod = ware.SellTags.Where(kv => _state.WorldTags.Contains(kv.Key)).Aggregate(0, (acc, kv) => acc + kv.Value) * (isSelling ? -1 : 1);
        var demandMod = ware.BuyTags.Where(kv => _state.WorldTags.Contains(kv.Key)).Aggregate(0, (acc, kv) => acc + kv.Value) * (isSelling ? 1 : -1);
        var legalityMod = !ware.IsUniversallyIllegal ? int.Max(0, ware.MaxLawLevel - _state.LocalLawLevel) : 0;

        var rollResult = _dice.RollD6s(3) + brokerSkillMod + legalityMod + demandMod + supplyMod + ModTableBaseline;
        rollResult = int.Max(0, rollResult);
        rollResult = int.Min(modifiers.Count, rollResult);

        return modifiers[rollResult];
    }

    private bool MatchesTradeCode(Ware w) => !w.AvailabilityTags.Any() || w.AvailabilityTags.Overlaps(_state.WorldTags);
}