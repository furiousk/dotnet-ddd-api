using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using src.Api.Data.Context;

namespace src.Api.Data.Test
{
    public abstract class BaseTest
    {
        public BaseTest()
        {

        }
    }

    public class DbTest : IDisposable
    {
        private string dataBaseName = $"dbApiTest_{Guid.NewGuid().ToString().Replace("-", string.Empty)}";
        public ServiceProvider ServiceProvider { get; private set; }

        public DbTest()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<ApplicationDbContext>(o =>
                o.UseMySql($"Persist Security Info=True;Server=localhost;Database={dataBaseName};User=root;Password=mudar@123"),
                ServiceLifetime.Transient
            );

            ServiceProvider = serviceCollection.BuildServiceProvider();
            using (var context = ServiceProvider.GetService<ApplicationDbContext>())
            {
                context.Database.EnsureCreated();
            }
        }

        public void Dispose()
        {
            using (var context = ServiceProvider.GetService<ApplicationDbContext>())
            {
                context.Database.EnsureDeleted();
            }
        }
    }
}
