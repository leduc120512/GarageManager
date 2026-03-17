using AutoGarageManager.Data;
using AutoGarageManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Accountant;

[Authorize(Roles = "Accountant")]
public class PartsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public PartsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Part> Parts { get; set; } = new List<Part>();

    public async Task OnGetAsync()
    {
        Parts = await _context.Parts
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}
