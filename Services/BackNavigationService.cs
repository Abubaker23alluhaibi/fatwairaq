namespace MyMauiApp.Services;

/// <summary>
/// يحفظ المسار (Route) للصفحة السابقة حتى يعمل زر الرجوع مع Shell.
/// نطبّع المسار إلى //RouteName لتجنب إغلاق التطبيق أو أخطاء التنقل.
/// </summary>
public class BackNavigationService
{
    private string? _returnRoute;

    private static readonly string[] KnownRoutes = { "MainPage", "FatwasListPage", "SearchResultsPage", "FatwaDetailsPage", "ArticleDetailsPage", "ScholarsPage", "ArticlesPage", "FavoritesPage", "SettingsPage" };

    public void SetReturnRoute(string location)
    {
        if (string.IsNullOrWhiteSpace(location)) return;
        var parts = location.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var lastPart = parts.Length > 0 ? parts[^1] : null;
        var routeName = lastPart?.Split('?')[0];
        if (string.IsNullOrEmpty(routeName) || !KnownRoutes.Contains(routeName))
        {
            _returnRoute = "//MainPage";
            return;
        }
        // الإبقاء على الاستعلام (مثل keyword) لصفحة نتائج البحث
        if (lastPart?.Contains('?') == true)
            _returnRoute = "//" + lastPart;
        else
            _returnRoute = "//" + routeName;
    }

    /// <summary>
    /// تعيين مسار الرجوع بشكل صريح (مثلاً مع categoryId) حتى يعود المستخدم للصفحة نفسها بنفس السياق.
    /// </summary>
    public void SetExplicitReturnRoute(string fullRoute)
    {
        if (string.IsNullOrWhiteSpace(fullRoute)) return;
        _returnRoute = fullRoute.StartsWith("//") ? fullRoute : "//" + fullRoute;
    }

    public string? GetAndClearReturnRoute()
    {
        var r = _returnRoute;
        _returnRoute = null;
        return r;
    }
}
