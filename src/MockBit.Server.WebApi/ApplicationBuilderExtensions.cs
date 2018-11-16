using Microsoft.AspNetCore.Builder;
using System;

namespace MockBit.Server.WebApi
{
    internal static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder MapByPort(this IApplicationBuilder app, int port, Action<IApplicationBuilder> configuration)
        {
            return app.MapWhen(p => p.Connection.LocalPort == port, configuration);
        }
    }
}
