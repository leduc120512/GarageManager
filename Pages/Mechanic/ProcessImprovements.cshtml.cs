using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoGarageManager.Pages.Mechanic;

[Authorize(Roles = "Mechanic")]
public class ProcessImprovementsModel : PageModel
{
    public IList<ImprovementSuggestion> Suggestions { get; set; } = new List<ImprovementSuggestion>();

    public void OnGet()
    {
        Suggestions = new List<ImprovementSuggestion>
        {
            new ImprovementSuggestion { Title = "Chuẩn hóa quy trình check-in", Description = "Thêm bước kiểm tra sơ bộ trước khi bắt đầu sửa để giảm sai sót." },
            new ImprovementSuggestion { Title = "Ghi nhận thiệt hại", Description = "Tạo danh mục nhanh các hư hỏng thường gặp để báo cáo dễ dàng." },
            new ImprovementSuggestion { Title = "Tự động nhắc bảo dưỡng", Description = "Tích hợp chức năng nhắc bảo dưỡng theo lịch trình để chủ động liên hệ khách." }
        };
    }
}

public class ImprovementSuggestion
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
