using Microsoft.Extensions.DependencyInjection;

namespace Cards.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add application services to the service collection for use
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddOpenDND(this IServiceCollection services)
        {
            // Core application services
            //services.AddScoped<IAuthorizationService, AuthorizationService>();
            
            // Add additional services here
            return services;
        }
    }
}