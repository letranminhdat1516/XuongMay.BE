using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Repositories.UOW;

namespace XuongMay.Services
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRepositories();
        }
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
