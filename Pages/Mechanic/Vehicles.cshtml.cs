using AutoGarageManager.Data;
using AutoGarageManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Mechanic;

[Authorize(Roles = "Mechanic")]
public class VehiclesModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public VehiclesModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public IList<ServiceHistory> LastServices { get; set; } = new List<ServiceHistory>();

    public async Task OnGetAsync()
    {
        Vehicles = await _context.Vehicles
            .Include(v => v.Customer)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();

        // Get last service for each vehicle
        var vehicleIds = Vehicles.Select(v => v.Id).ToList();
        LastServices = await _context.ServiceHistories
            .Where(sh => vehicleIds.Contains(sh.VehicleId))
            .GroupBy(sh => sh.VehicleId)
            .Select(g => g.OrderByDescending(sh => sh.ServiceDate).First())
            .ToListAsync();
    }
}
