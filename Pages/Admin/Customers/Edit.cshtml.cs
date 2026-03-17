using AutoGarageManager.Models;
using CustomerModel = AutoGarageManager.Models.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Admin.Customers;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public CustomerModel Customer { get; set; } = new CustomerModel();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        Customer = customer;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Ensure the required Name field is populated from FullName (UI binds FullName)
        if (string.IsNullOrWhiteSpace(Customer.Name) && !string.IsNullOrWhiteSpace(Customer.FullName))
        {
            Customer.Name = Customer.FullName;
            ModelState.Remove("Customer.Name");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Customer).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return RedirectToPage("../Customers");
    }
}
