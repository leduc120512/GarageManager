using AutoGarageManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Admin.Vehicles;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Vehicle Vehicle { get; set; } = new Vehicle();

    public SelectList CustomerOptions { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null)
        {
            return NotFound();
        }

        Vehicle = vehicle;
        CustomerOptions = new SelectList(await _context.Customers.ToListAsync(), "Id", "FullName", Vehicle.CustomerId);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            CustomerOptions = new SelectList(await _context.Customers.ToListAsync(), "Id", "FullName", Vehicle.CustomerId);
            return Page();
        }

        _context.Attach(Vehicle).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return RedirectToPage("../Vehicles");
    }
}
