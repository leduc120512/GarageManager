using AutoGarageManager.Data;
using AutoGarageManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Mechanic;

[Authorize(Roles = "Mechanic")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public int TodayServices { get; set; }
    public int TotalServices { get; set; }
    public int PendingReminders { get; set; }
    public IList<ServiceHistory> TodayServiceHistories { get; set; } = new List<ServiceHistory>();
    public IList<MaintenanceReminder> RecentReminders { get; set; } = new List<MaintenanceReminder>();

    public async Task OnGetAsync()
    {
        var today = DateTime.Today;
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return;
        }

        // Services done by this mechanic today
        TodayServices = await _context.ServiceHistories
            .CountAsync(sh => sh.ServiceDate.Date == today && sh.MechanicId == currentUser.Id);

        // Total services by this mechanic
        TotalServices = await _context.ServiceHistories
            .CountAsync(sh => sh.MechanicId == currentUser.Id);

        // Pending reminders
        PendingReminders = await _context.MaintenanceReminders
            .CountAsync(mr => !mr.IsSent);

        // Today's service histories by this mechanic
        TodayServiceHistories = await _context.ServiceHistories
            .Include(sh => sh.Vehicle)
            .Where(sh => sh.ServiceDate.Date == today && sh.MechanicId == currentUser.Id)
            .OrderBy(sh => sh.ServiceDate)
            .Take(5)
            .ToListAsync();

        // Recent reminders
        RecentReminders = await _context.MaintenanceReminders
            .Include(mr => mr.Vehicle)
            .Where(mr => !mr.IsSent)
            .OrderByDescending(mr => mr.CreatedAt)
            .Take(5)
            .ToListAsync();
    }
}
