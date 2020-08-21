using Microsoft.EntityFrameworkCore;
using src.Api.Domain.Entities;
using src.Api.Data.Mapping;
using System;

namespace src.Api.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserEntity> User { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserEntity>(new UserMap().Configure);
            modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Administrador",
                    Email = "furious@gmail.com",
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                }
            );
        }

    }
}
