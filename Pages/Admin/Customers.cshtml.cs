using AutoGarageManager.Models;
using CustomerModel = AutoGarageManager.Models.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Admin;

[Authorize(Roles = "Admin")]
public class CustomersModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CustomersModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<CustomerModel> Customers { get; set; } = new List<CustomerModel>();

    public async Task OnGetAsync()
    {
        Customers = await _context.Customers
            .Include(c => c.Vehicles)
            .ToListAsync();
    }
}
