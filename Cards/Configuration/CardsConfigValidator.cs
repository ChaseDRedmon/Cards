using System;
using Cards.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace Cards.Configuration
{
    public class OpenDndConfigValidator : IStartupFilter
    {
        private readonly CardsConfig _config;

        public OpenDndConfigValidator(IOptions<CardsConfig> config)
        {
            _config = config.Value;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            if (string.IsNullOrWhiteSpace(_config.DbConnection))
            {
                Log.Fatal("The DbConnection string was not set - this is fatal! Check the config");
            }

            if (string.IsNullOrWhiteSpace(_config.SentryIOToken))
            {
                Log.Warning("The SentryIOToken was not set. SentryIO logging disabled. Check the config.");
            }

            return next;
        }
    }
}
