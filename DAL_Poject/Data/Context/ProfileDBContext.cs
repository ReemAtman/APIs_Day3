using DAL_Poject.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL_Poject.Data.Context
{
    public class ProfileDBContext : IdentityDbContext<Student>
    {
        public ProfileDBContext(DbContextOptions<ProfileDBContext> options)
        : base(options)
        { 

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Student>().ToTable("Students");
            builder.Entity<IdentityUserClaim<string>>().ToTable("StudentsClaims");
        }
    }
}
