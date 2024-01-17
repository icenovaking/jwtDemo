using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace jwtAuthDemo.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions dbContextOptions)
    : base(dbContextOptions)
        {
        }
        public DbSet<LoginModel>? LoginModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginModel>().HasData(new LoginModel
            {
                Id = 1,
                UserName = "Roy",
                Password = "Roy@123"
            });
        }
    }
}
