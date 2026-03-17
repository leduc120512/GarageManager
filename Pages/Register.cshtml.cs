using System.ComponentModel.DataAnnotations;
using AutoGarageManager.Models;
using CustomerModel = AutoGarageManager.Models.Customer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoGarageManager.Pages;

public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [BindProperty]
    [Required]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu nhập lại không khớp.")]
    [Display(Name = "Nhập lại mật khẩu")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Email,
            Email = Email,
            FullName = FullName
        };

        var result = await _userManager.CreateAsync(user, Password);
        if (!result.Succeeded)
        {
            ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            return Page();
        }

        // Gán role mặc định cho user mới (Customer)
        await _userManager.AddToRoleAsync(user, "Customer");

        // Tạo profile khách hàng nếu chưa có
        if (!_context.Customers.Any(c => c.Email == Email))
        {
            var customer = new CustomerModel
            {
                Name = FullName,
                FullName = FullName,
                Email = Email,
                Phone = string.Empty,
                Address = string.Empty
            };

            // Ensure required string fields are not null so inserts succeed
            customer.Name ??= string.Empty;
            customer.Email ??= string.Empty;
            customer.Phone ??= string.Empty;
            customer.Address ??= string.Empty;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        // Đăng nhập tự động sau khi đăng ký
        await _signInManager.SignInAsync(user, isPersistent: false);

        // Chuyển vào trang giao diện khách hàng
        return RedirectToPage("/Customer/Index");
    }
}

