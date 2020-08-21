using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace src.Api.Data.Context
{
    public class ContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Server=localhost;Port=3306;Database=exodus;Uid=root;Pwd=mudar@123";
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseMySql(connectionString);
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
