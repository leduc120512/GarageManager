using AutoGarageManager.Data;
using AutoGarageManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Mechanic;

[Authorize(Roles = "Mechanic")]
public class ServiceHistoryModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ServiceHistoryModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public IList<ServiceHistory> ServiceHistories { get; set; } = new List<ServiceHistory>();

    public async Task<IActionResult> OnGetAsync(int vehicleId)
    {
        VehicleId = vehicleId;
        Vehicle = await _context.Vehicles
            .Include(v => v.Customer)
            .FirstOrDefaultAsync(v => v.Id == vehicleId);

        if (Vehicle == null)
        {
            return NotFound();
        }

        ServiceHistories = await _context.ServiceHistories
            .Include(sh => sh.Mechanic)
            .Where(sh => sh.VehicleId == vehicleId)
            .OrderByDescending(sh => sh.ServiceDate)
            .ToListAsync();

        return Page();
    }
}
