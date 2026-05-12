using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolSocialApp.Models;

namespace SchoolSocialApp.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = new[] { "Admin", "Student" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@schoolapp.local";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "School Admin",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            if (!context.SchoolClasses.Any())
            {
                var classA = new SchoolClass
                {
                    Name = "10A",
                    Description = "Class 10A - Private school classmates"
                };
                context.SchoolClasses.Add(classA);
                await context.SaveChangesAsync();

                context.ClassSettings.Add(new ClassSetting
                {
                    ClassId = classA.Id,
                    IsPostingAllowed = true,
                    CanShareResources = true,
                    Description = "Only students from this class can access the feed."
                });

                var student = new ApplicationUser
                {
                    UserName = "student@schoolapp.local",
                    Email = "student@schoolapp.local",
                    FullName = "Student Member",
                    SchoolClassId = classA.Id,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(student, "Student123!");
                await userManager.AddToRoleAsync(student, "Student");
                context.Posts.Add(new Post
                {
                    Author = student,
                    ClassId = classA.Id,
                    Message = "Welcome to our private class social app!",
                    CreatedAt = DateTime.UtcNow
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
