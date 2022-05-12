using AtakDomain.Repository;
using AtakDomain.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Application
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddTransient<HotelService>();

            //services.AddHttpContextAccessor();

            return services;
        }
    }
}