using AutoGarageManager.Data;
using AutoGarageManager.Models;
using CustomerModel = AutoGarageManager.Models.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Accountant;

[Authorize(Roles = "Accountant")]
public class CustomersModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CustomersModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<CustomerModel> Customers { get; set; } = new List<CustomerModel>();
    public IList<CustomerTotal> CustomerTotals { get; set; } = new List<CustomerTotal>();

    public async Task OnGetAsync()
    {
        Customers = await _context.Customers
            .Include(c => c.Vehicles)
            .OrderBy(c => c.FullName)
            .ToListAsync();

        // Calculate total spent by each customer
        var customerIds = Customers.Select(c => c.Id).ToList();
        CustomerTotals = await _context.ServiceHistories
            .Where(sh => customerIds.Contains(sh.Vehicle.CustomerId))
            .GroupBy(sh => sh.Vehicle.CustomerId)
            .Select(g => new CustomerTotal
            {
                CustomerId = g.Key,
                Total = g.Sum(sh => sh.TotalCost)
            })
            .ToListAsync();
    }

    public class CustomerTotal
    {
        public int CustomerId { get; set; }
        public decimal Total { get; set; }
    }
}
