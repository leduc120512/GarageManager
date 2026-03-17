using AutoGarageManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Customer;

[Authorize(Roles = "Customer")]
public class VehiclesModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public VehiclesModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IList<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public IList<ServiceHistory> LastServices { get; set; } = new List<ServiceHistory>();

    public async Task OnGetAsync()
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

        Vehicles = await _context.Vehicles
            .Where(v => v.CustomerId == customer.Id)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();

        var vehicleIds = Vehicles.Select(v => v.Id).ToList();
        LastServices = await _context.ServiceHistories
            .Where(sh => vehicleIds.Contains(sh.VehicleId))
            .GroupBy(sh => sh.VehicleId)
            .Select(g => g.OrderByDescending(sh => sh.ServiceDate).First())
            .ToListAsync();
    }
}
