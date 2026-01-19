using System.Security.Cryptography;
using System.Text;
using gop.Data;
using gop.Data.Entities;
using gop.Enums;
using gop.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gop.Seeds;

public static class AdminAccountSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var hashService = scope.ServiceProvider.GetRequiredService<IHashService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AdminAccountSeeder");

        const string adminEmail = "admin@gop.gov.pk";

        var adminExists = await context.Users.AnyAsync(u => u.Email == adminEmail && !u.IsDeleted);

        if (adminExists)
        {
            logger.LogInformation("Admin account already exists.");
            return;
        }

        var admin = new User
        {
            FullName = "System Administrator",
            Email = adminEmail,
            Ministry = "Government of Pakistan",
            Role = UserRoleEnum.Admin,
            Status = UserStatusEnum.Active,
            HashPassword = hashService.Hash("Secret@1236"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(admin);
        await context.SaveChangesAsync();

        logger.LogInformation("Admin account seeded successfully.");
    }
}