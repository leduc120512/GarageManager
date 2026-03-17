using AutoGarageManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Customer;

[Authorize(Roles = "Customer")]
public class ServiceHistoryModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ServiceHistoryModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public int? SelectedVehicleId { get; set; }
    public IList<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public IList<ServiceHistory> ServiceHistories { get; set; } = new List<ServiceHistory>();

    public async Task OnGetAsync(int? vehicleId)
    {
        SelectedVehicleId = vehicleId;

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

        Vehicles = customer.Vehicles.OrderByDescending(v => v.CreatedAt).ToList();

        var vehicleIds = Vehicles.Select(v => v.Id).ToList();

        var query = _context.ServiceHistories
            .Include(sh => sh.Vehicle)
            .Include(sh => sh.Mechanic)
            .Where(sh => vehicleIds.Contains(sh.VehicleId));

        if (vehicleId.HasValue)
        {
            query = query.Where(sh => sh.VehicleId == vehicleId.Value);
        }

        ServiceHistories = await query
            .OrderByDescending(sh => sh.ServiceDate)
            .ToListAsync();
    }
}
