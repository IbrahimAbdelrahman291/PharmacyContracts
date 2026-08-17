// src/Host/PharmacyContracts.Api/Hangfire/HangfireDashboardAuthorizationFilter.cs
using System.Text;
using Hangfire.Dashboard;
using Microsoft.Extensions.Configuration;

namespace PharmacyContracts.Api.Hangfire;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly IConfiguration _configuration;
    public HangfireDashboardAuthorizationFilter(IConfiguration configuration) => _configuration = configuration;

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var authHeader = httpContext.Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            httpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return false;
        }

        try
        {
            var encodedCredentials = authHeader["Basic ".Length..].Trim();
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            var separatorIndex = decodedCredentials.IndexOf(':');

            if (separatorIndex < 0)
                return false;

            var username = decodedCredentials[..separatorIndex];
            var password = decodedCredentials[(separatorIndex + 1)..];

            var expectedUsername = _configuration["HangfireDashboard:Username"];
            var expectedPassword = _configuration["HangfireDashboard:Password"];

            return username == expectedUsername && password == expectedPassword;
        }
        catch
        {
            return false;
        }
    }
}