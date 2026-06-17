using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolSocialApp.Models;
using System.Security.Cryptography;

namespace SchoolSocialApp.Data
{
    public static class SeedData
    {
        public const string AdminRole = "Admin";
        public const string StudentRole = "Student";

        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(SeedData));

            if (!configuration.GetValue<bool>("SeedData:CreateSeedUsers", true))
            {
                logger.LogInformation("Seed user creation is disabled.");
                return;
            }

            using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await EnsureRoleAsync(roleManager, AdminRole);
            await EnsureRoleAsync(roleManager, StudentRole);

            var adminEmail = configuration["SeedData:AdminEmail"] ?? "admin@schoolapp.local";
            var studentEmail = configuration["SeedData:StudentEmail"] ?? "student@schoolapp.local";
            var adminPassword = GetSeedPassword(configuration, AdminRole, adminEmail, logger);
            var studentPassword = GetSeedPassword(configuration, StudentRole, studentEmail, logger);

            var adminConfiguredPassword = GetConfiguredSeedPassword(configuration, AdminRole);
            var studentConfiguredPassword = GetConfiguredSeedPassword(configuration, StudentRole);

            await EnsureUserAsync(userManager, adminEmail, "School Admin", adminPassword, adminConfiguredPassword, AdminRole, classId: null);
            await EnsureUserAsync(userManager, studentEmail, "Student Member", studentPassword, studentConfiguredPassword, StudentRole, classId: null);

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

                var student = await userManager.FindByEmailAsync(studentEmail);
                if (student != null)
                {
                    student.SchoolClassId = classA.Id;
                    await userManager.UpdateAsync(student);
                }

                context.Posts.Add(new Post
                {
                    AuthorId = student?.Id ?? string.Empty,
                    ClassId = classA.Id,
                    Message = "Welcome to our private class social app!",
                    CreatedAt = DateTime.UtcNow
                });

                await context.SaveChangesAsync();
            }

            logger.LogInformation("Development seed data is ready.");
        }

        private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        private static async Task EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string fullName,
            string password,
            string? configuredPassword,
            string roleName,
            int? classId)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    SchoolClassId = classId,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create user '{email}': {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                user.FullName = fullName;
                user.Email = email;
                user.EmailConfirmed = true;

                if (!string.IsNullOrWhiteSpace(configuredPassword) && await userManager.HasPasswordAsync(user))
                {
                    var removePasswordResult = await userManager.RemovePasswordAsync(user);
                    if (!removePasswordResult.Succeeded)
                    {
                        throw new InvalidOperationException($"Failed to rotate password for '{email}': {string.Join(", ", removePasswordResult.Errors.Select(e => e.Description))}");
                    }

                    var addPasswordResult = await userManager.AddPasswordAsync(user, configuredPassword);
                    if (!addPasswordResult.Succeeded)
                    {
                        throw new InvalidOperationException($"Failed to rotate password for '{email}': {string.Join(", ", addPasswordResult.Errors.Select(e => e.Description))}");
                    }
                }

                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to update user '{email}': {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                var roleResult = await userManager.AddToRoleAsync(user, roleName);
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to assign role '{roleName}' to '{email}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
        }

        private static string GetSeedPassword(IConfiguration configuration, string roleName, string email, ILogger logger)
        {
            var configuredPassword = configuration[$"SeedData:{roleName}Password"];
            if (!string.IsNullOrWhiteSpace(configuredPassword))
            {
                return configuredPassword;
            }

            var generatedPassword = GenerateStrongPassword();
            logger.LogInformation(
                "No SeedData:{RoleName}Password configured for {Email}. Generated development password: {Password}",
                roleName,
                email,
                generatedPassword);

            return generatedPassword;
        }

        private static string? GetConfiguredSeedPassword(IConfiguration configuration, string roleName)
        {
            return configuration[$"SeedData:{roleName}Password"];
        }

        private static string GenerateStrongPassword()
        {
            var randomPart = Convert.ToHexString(RandomNumberGenerator.GetBytes(12)).ToLowerInvariant();
            return $"SchoolApp-{randomPart}-Aa1!";
        }
    }
}
