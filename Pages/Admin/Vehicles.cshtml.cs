using AutoGarageManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Admin;

[Authorize(Roles = "Admin")]
public class VehiclesModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public VehiclesModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public async Task OnGetAsync()
    {
        Vehicles = await _context.Vehicles
            .Include(v => v.Customer)
            .ToListAsync();
    }
}
