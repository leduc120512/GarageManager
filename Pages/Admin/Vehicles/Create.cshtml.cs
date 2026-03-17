using AutoGarageManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Admin.Vehicles;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Vehicle Vehicle { get; set; } = new Vehicle();

    public SelectList CustomerOptions { get; set; } = null!;

    public async Task OnGetAsync()
    {
        CustomerOptions = new SelectList(await _context.Customers.ToListAsync(), "Id", "FullName");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            CustomerOptions = new SelectList(await _context.Customers.ToListAsync(), "Id", "FullName");
            return Page();
        }

        _context.Vehicles.Add(Vehicle);
        await _context.SaveChangesAsync();

        return RedirectToPage("../Vehicles");
    }
}
