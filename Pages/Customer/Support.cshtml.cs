using AutoGarageManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AutoGarageManager.Pages.Customer;

[Authorize(Roles = "Customer")]
public class SupportModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public SupportModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public SupportInputModel Input { get; set; } = new SupportInputModel();

    public string? StatusMessage { get; set; }
    public IList<SupportTicket> Tickets { get; set; } = new List<SupportTicket>();

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            Tickets = await _context.SupportTickets
                .Where(t => t.CustomerId == user.Id)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        // Create support ticket
        var ticket = new SupportTicket
        {
            CustomerId = user.Id,
            Subject = Input.Subject!,
            Message = Input.Message!,
            CreatedAt = DateTime.Now,
            Status = "Pending",
            Priority = "Normal"
        };

        _context.SupportTickets.Add(ticket);
        await _context.SaveChangesAsync();

        StatusMessage = "Yêu cầu hỗ trợ của bạn đã được gửi thành công. Chúng tôi sẽ liên hệ lại sớm nhất có thể.";

        // Reset form
        Input = new SupportInputModel();

        // Reload tickets
        Tickets = await _context.SupportTickets
            .Where(t => t.CustomerId == user.Id)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return Page();
    }
}

public class SupportInputModel
{
    [Required(ErrorMessage = "Vui lòng chọn chủ đề.")]
    [Display(Name = "Chủ đề")]
    public string? Subject { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập nội dung.")]
    [Display(Name = "Nội dung")]
    [StringLength(1000, ErrorMessage = "Nội dung không được vượt quá 1000 ký tự.")]
    public string? Message { get; set; }
}