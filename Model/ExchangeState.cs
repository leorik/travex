namespace TravEx.Model;

using JetBrains.Annotations;

using MessagePack;

[PublicAPI]
[MessagePackObject(true)]
public class ExchangeState
{
    public int RngSeed { get; set; }
    
    public int PlayerBrokerSkill { get; set; }

    public bool IsBlackMarket { get; set; }

    public int LocalLawLevel { get; set; } = 5;
    
    public int ContractorBrokerSkill { get; set; } = 2;
    
    public int WorldPopulation { get; set; } = 5;

    public ISet<string> WorldTags { get; set; } = new HashSet<string>();

    public override string ToString()
    {
        return $"{nameof(RngSeed)}: {RngSeed}, {nameof(PlayerBrokerSkill)}: {PlayerBrokerSkill}, {nameof(WorldPopulation)}: {WorldPopulation}, {nameof(IsBlackMarket)}: {IsBlackMarket}, {nameof(LocalLawLevel)}: {LocalLawLevel}, {nameof(ContractorBrokerSkill)}: {ContractorBrokerSkill}, {nameof(WorldTags)}: {WorldTags}";
    }
}