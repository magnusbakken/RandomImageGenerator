using Microsoft.Extensions.Options;
using System.Net;

namespace RandomImageGenerator.SafeList;

public class SafeListMiddleware : IMiddleware
{
    private IList<byte[]> _safeAddresses;

    public SafeListMiddleware(IOptionsMonitor<SafeListOptions> options)
    {
        _safeAddresses = GetAddressesAsByteLists(options.CurrentValue);
        options.OnChange(o => _safeAddresses = GetAddressesAsByteLists(o));
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var ip = context.Connection.RemoteIpAddress?.GetAddressBytes();
        if (ip != null && _safeAddresses.Any(safe => safe.SequenceEqual(ip)))
            await next(context);
        else
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
    }

    private static IList<byte[]> GetAddressesAsByteLists(SafeListOptions options)
    {
        return options.Addresses.Select(address => IPAddress.Parse(address).GetAddressBytes()).ToList();
    }
}
