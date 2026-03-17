using AutoGarageManager.Models;
using Microsoft.AspNetCore.Identity;

namespace AutoGarageManager.Data;

public static class SeedData
{
    public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Admin", "Mechanic", "Accountant", "Customer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await EnsureUserAsync(userManager, "admin@garage.com", "Admin123!", "Admin", "Administrator");
        await EnsureUserAsync(userManager, "mechanic@garage.com", "Mech123!", "Mechanic", "Mechanic User");
        await EnsureUserAsync(userManager, "accountant@garage.com", "Acc123!", "Accountant", "Accountant User");
        await EnsureUserAsync(userManager, "customer@garage.com", "Cust123!", "Customer", "Customer User");
    }

    private static async Task EnsureUserAsync(UserManager<ApplicationUser> userManager, string email, string password, string role, string fullName)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName
            };

            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role);
        }
        else
        {
            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
