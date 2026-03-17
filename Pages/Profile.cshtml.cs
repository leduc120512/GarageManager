using AutoGarageManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AutoGarageManager.Pages;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ProfileModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [BindProperty]
    public ProfileInputModel Input { get; set; } = new ProfileInputModel();

    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        Input = new ProfileInputModel
        {
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        // Load additional info from Customer if applicable
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == user.Email);
        if (customer != null)
        {
            Input.Address = customer.Address;
        }

        return Page();
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
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        user.FullName = Input.FullName;
        user.PhoneNumber = Input.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            StatusMessage = "Lỗi khi cập nhật thông tin.";
            return Page();
        }

        // Update Customer info if exists
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == user.Email);
        if (customer != null)
        {
            customer.FullName = Input.FullName;
            customer.Phone = Input.PhoneNumber ?? string.Empty;
            customer.Address = Input.Address ?? string.Empty;
            await _context.SaveChangesAsync();
        }

        StatusMessage = "Thông tin đã được cập nhật thành công.";
        return Page();
    }
}

public class ProfileInputModel
{
    [Required]
    [Display(Name = "Họ và tên")]
    public string? FullName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Phone]
    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }
}
