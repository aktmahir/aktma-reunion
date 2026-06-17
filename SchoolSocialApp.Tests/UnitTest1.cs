using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchoolSocialApp.Data;
using SchoolSocialApp.Models;

namespace SchoolSocialApp.Tests;

public sealed class CriticalPathTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection;
    private readonly TestAppFactory _factory;

    public CriticalPathTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _factory = new TestAppFactory(_connection);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task DevelopmentStartupSeedsRolesUsersClassSettingsAndPosts()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var admin = await userManager.FindByEmailAsync("admin@schoolapp.local");
        var student = await userManager.FindByEmailAsync("student@schoolapp.local");

        Assert.NotNull(admin);
        Assert.NotNull(student);
        Assert.True(await userManager.IsInRoleAsync(admin, SeedData.AdminRole));
        Assert.True(await userManager.IsInRoleAsync(student, SeedData.StudentRole));
        Assert.True(await context.SchoolClasses.AnyAsync());
        Assert.True(await context.ClassSettings.AnyAsync());
        Assert.True(await context.Posts.AnyAsync());
    }

    [Fact]
    public async Task StudentCannotUpdateAnotherClassSettings()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var studentClass = await context.SchoolClasses.SingleAsync();

        var otherClass = new SchoolClass
        {
            Name = "10B",
            Description = "Class 10B - Private school classmates"
        };
        context.SchoolClasses.Add(otherClass);
        await context.SaveChangesAsync();

        var otherSetting = new ClassSetting
        {
            ClassId = otherClass.Id,
            IsPostingAllowed = true,
            CanShareResources = true,
            Description = "Original 10B setting"
        };
        context.ClassSettings.Add(otherSetting);
        await context.SaveChangesAsync();

        var client = await CreateAuthenticatedClientAsync(_factory, "student@schoolapp.local");
        var settingsResponse = await client.GetAsync("/Classes/Settings");
        settingsResponse.EnsureSuccessStatusCode();

        var html = await settingsResponse.Content.ReadAsStringAsync();
        var token = ExtractAntiForgeryToken(html);

        var response = await client.PostAsync("/Classes/Settings", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Id"] = otherClass.Id.ToString(),
            ["ClassId"] = studentClass.Id.ToString(),
            ["IsPostingAllowed"] = "false",
            ["CanShareResources"] = "false",
            ["Description"] = "Hacked setting",
            ["__RequestVerificationToken"] = token
        }));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var unchangedSetting = await context.ClassSettings.SingleAsync(s => s.Id == otherSetting.Id);
        Assert.Equal("Original 10B setting", unchangedSetting.Description);
        Assert.True(unchangedSetting.IsPostingAllowed);
        Assert.True(unchangedSetting.CanShareResources);
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        WebApplicationFactory<Program> factory,
        string email)
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        await using var scope = factory.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var signInManager = serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var originalHttpContext = httpContextAccessor.HttpContext;
        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        httpContextAccessor.HttpContext = httpContext;

        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);
        await signInManager.SignInAsync(user, isPersistent: false);

        httpContextAccessor.HttpContext = originalHttpContext;

        var setCookie = httpContext.Response.Headers.SetCookie.ToString();
        Assert.NotEmpty(setCookie);
        client.DefaultRequestHeaders.Add("Cookie", setCookie.Split(';')[0]);

        return client;
    }

    private static string ExtractAntiForgeryToken(string html)
    {
        var match = Regex.Match(
            html,
            @"name=""__RequestVerificationToken""[^>]*value=""([^""]+)""|value=""([^""]+)""[^>]*name=""__RequestVerificationToken""");

        Assert.True(match.Success, "Antiforgery token not found.");
        return match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
    }

    private sealed class TestAppFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection;

        public TestAppFactory(SqliteConnection connection)
        {
            _connection = connection;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["SeedData:AdminPassword"] = "AdminPass123!",
                    ["SeedData:StudentPassword"] = "StudentPass123!"
                });
            });
            builder.ConfigureServices(services =>
            {
                services.AddHttpContextAccessor();

                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(_connection));
            });
        }
    }
}
