using AutoGarageManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Customer;

[Authorize(Roles = "Customer")]
public class RemindersModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public RemindersModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IList<MaintenanceReminder> Reminders { get; set; } = new List<MaintenanceReminder>();

    public async Task OnGetAsync()
    {
        await LoadReminders();
    }

    public async Task<IActionResult> OnPostCompleteAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        var customer = await _context.Customers
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.Email == user.Email);

        if (customer == null)
        {
            return NotFound();
        }

        var vehicleIds = customer.Vehicles.Select(v => v.Id).ToList();

        var reminder = await _context.MaintenanceReminders
            .FirstOrDefaultAsync(r => r.Id == id && vehicleIds.Contains(r.VehicleId));

        if (reminder == null)
        {
            return NotFound();
        }

        reminder.IsCompleted = true;
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    private async Task LoadReminders()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return;
        }

        var customer = await _context.Customers
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.Email == user.Email);

        if (customer == null)
        {
            return;
        }

        var vehicleIds = customer.Vehicles.Select(v => v.Id).ToList();

        Reminders = await _context.MaintenanceReminders
            .Include(r => r.Vehicle)
            .Where(r => vehicleIds.Contains(r.VehicleId))
            .OrderBy(r => r.ReminderDate)
            .ToListAsync();
    }
}
