namespace TravEx.Model;

public record Ware(string Name, int BasePrice, int TonnageDiceCount, int TonnageMultiplier, int MaxLawLevel, bool IsUniversallyIllegal,
    Dictionary<string, int> BuyTags,
    Dictionary<string, int> SellTags, HashSet<string> AvailabilityTags)
{
    public bool IsLegal(int localLawLevel)
    {
        return !IsUniversallyIllegal && MaxLawLevel <= localLawLevel;
    }
};