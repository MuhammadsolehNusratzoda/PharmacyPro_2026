using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PharmacyPro.Domain.Constants;
using PharmacyPro.Domain.Entities;
using PharmacyPro.Infrastructure.Identity;

namespace PharmacyPro.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var db = sp.GetRequiredService<AppDbContext>();
        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var config = sp.GetRequiredService<IConfiguration>();

        await db.Database.MigrateAsync();

        // Roles
        foreach (var role in new[] { Roles.Admin, Roles.Pharmacist })
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        // Default admin
        var email = config["SeedAdmin:Email"] ?? "admin@pharmacypro.tj";
        var password = config["SeedAdmin:Password"] ?? "Admin@12345";

        if (await userManager.FindByEmailAsync(email) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = "Administrator",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, Roles.Admin);
        }

        // Default expense categories
        if (!await db.ExpenseCategories.AnyAsync())
        {
            db.ExpenseCategories.AddRange(
                new ExpenseCategory { Name = "Свет" },
                new ExpenseCategory { Name = "Аренда" },
                new ExpenseCategory { Name = "Маош" },
                new ExpenseCategory { Name = "Барқ" });
            await db.SaveChangesAsync();
        }
    }
}