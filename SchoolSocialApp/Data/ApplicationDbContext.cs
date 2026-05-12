using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolSocialApp.Models;

namespace SchoolSocialApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SchoolClass> SchoolClasses => Set<SchoolClass>();
        public DbSet<ClassSetting> ClassSettings => Set<ClassSetting>();
        public DbSet<Post> Posts => Set<Post>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SchoolClass>()
                .HasMany(c => c.Members)
                .WithOne(u => u.SchoolClass)
                .HasForeignKey(u => u.SchoolClassId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<SchoolClass>()
                .HasOne(c => c.ClassSetting)
                .WithOne(s => s.Class)
                .HasForeignKey<ClassSetting>(s => s.ClassId);
        }
    }
}
