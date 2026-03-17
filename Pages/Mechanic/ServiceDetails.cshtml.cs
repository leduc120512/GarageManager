using AutoGarageManager.Data;
using AutoGarageManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Mechanic;

[Authorize(Roles = "Mechanic")]
public class ServiceDetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ServiceDetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public ServiceHistory? ServiceHistory { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        ServiceHistory = await _context.ServiceHistories
            .Include(sh => sh.Mechanic)
            .Include(sh => sh.ServicesPerformed)
                .ThenInclude(shs => shs.Service)
            .Include(sh => sh.PartsUsed)
                .ThenInclude(shp => shp.Part)
            .FirstOrDefaultAsync(sh => sh.Id == id);

        if (ServiceHistory == null)
        {
            return NotFound();
        }

        return Page();
    }
}
