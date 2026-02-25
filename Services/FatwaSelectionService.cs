using MyMauiApp.Models;

namespace MyMauiApp.Services;

/// <summary>
/// يحفظ الفتوى المختارة عند الانتقال لصفحة التفاصيل حتى لا تعتمد الصفحة على قائمة "جميع الفتاوى" فقط.
/// </summary>
public class FatwaSelectionService
{
    public Fatwa? SelectedFatwa { get; set; }
}
