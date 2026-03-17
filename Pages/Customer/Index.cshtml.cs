using AutoGarageManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Customer;

[Authorize(Roles = "Customer")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext context, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public string CustomerName { get; set; } = "Khách hàng";
    public string Email { get; set; } = string.Empty;
    public int VehicleCount { get; set; }
    public int ReminderCount { get; set; }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            CustomerName = user.FullName ?? user.UserName ?? "Khách hàng";
            Email = user.Email ?? string.Empty;

            var customer = await _context.Customers
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.Email == Email);

            if (customer != null)
            {
                VehicleCount = customer.Vehicles?.Count ?? 0;

                var vehicleIds = customer.Vehicles?.Select(v => v.Id).ToList() ?? new List<int>();
                ReminderCount = await _context.MaintenanceReminders
                    .CountAsync(r => vehicleIds.Contains(r.VehicleId) && !r.IsCompleted);
            }
        }
    }
}
