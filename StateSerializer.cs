namespace TravEx;

using Base62;

using MessagePack;

using TravEx.Model;

public static class StateSerializer
{
    public static string ToPortableString(this ExchangeState state)
    {
        var binaryState = MessagePackSerializer.Serialize(state);

        return binaryState.ToBase62();
    }

    public static ExchangeState StringToState(string portableString)
    {
        var binaryState = portableString.FromBase62();

        return MessagePackSerializer.Deserialize<ExchangeState>(binaryState);
    }
}