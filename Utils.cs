using System.Web;
using Microsoft.AspNetCore.Components;

namespace TravEx;

using Base62;

using MessagePack;

using TravEx.Model;

public static class Utils
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

    public static string? GetStateFromQuery(NavigationManager navigationManager)
    {
        
        var queryString = new Uri(navigationManager.Uri).Query;
        var queryParams = HttpUtility.ParseQueryString(queryString);

        return queryParams["offers"];
    }
}