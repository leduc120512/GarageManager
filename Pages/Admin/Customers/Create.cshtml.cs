using AutoGarageManager.Models;
using CustomerModel = AutoGarageManager.Models.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoGarageManager.Pages.Admin.Customers;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public CustomerModel Customer { get; set; } = new CustomerModel();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Ensure required string fields are not null so inserts succeed
        Customer.Name ??= string.Empty;
        Customer.Email ??= string.Empty;
        Customer.Phone ??= string.Empty;
        Customer.Address ??= string.Empty;

        _context.Customers.Add(Customer);
        await _context.SaveChangesAsync();

        return RedirectToPage("../Customers");
    }
}
