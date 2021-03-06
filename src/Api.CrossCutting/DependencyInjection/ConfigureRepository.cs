using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using src.Api.Data.Context;
using src.Api.Data.Implementation;
using src.Api.Data.Interfaces;
using src.Api.Domain.Interfaces;
using src.Api.Domain.Repository;

namespace src.Api.CrossCutting.DependencyInjection
{
    public class ConfigureRepository
    {
        public static void ConfigureDependenciesRepository(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            serviceCollection.AddScoped<IUserRepository, UserImplementation>();

            if (Environment.GetEnvironmentVariable("DATABASE").ToLower() == "MYSQL".ToLower())
            {
                serviceCollection.AddDbContext<ApplicationDbContext>(
                    options => options.UseMySql(Environment.GetEnvironmentVariable("DB_CONNECTION"))
                );
            }
        }
    }
}
