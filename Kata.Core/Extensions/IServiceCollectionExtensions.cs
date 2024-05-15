using Kata.Core.Services;
using Kata.Data.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kata.Core.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddServices();
        }

        private static void AddServices(this IServiceCollection services)
        {
            services
                .AddScoped<IAppointmentService, AppointmentService>()
                .AddScoped<IAppointmentRepository, AppointmentRepository>();

        }
    }
}
