using MyMauiApp.Models;

namespace MyMauiApp.Services;

/// <summary>
/// يحفظ آخر نتائج البحث مؤقتاً لظهورها فوراً عند الرجوع من صفحة تفاصيل الفتوى.
/// </summary>
public class SearchResultsCacheService
{
    private string? _cachedKeyword;
    private List<Fatwa>? _cachedResults;

    public void Set(string keyword, List<Fatwa> results)
    {
        _cachedKeyword = keyword?.Trim();
        _cachedResults = results == null ? new List<Fatwa>() : new List<Fatwa>(results);
    }

    public bool TryGet(string keyword, out List<Fatwa>? results)
    {
        results = null;
        var key = keyword?.Trim();
        if (string.IsNullOrEmpty(key) || _cachedResults == null || _cachedKeyword == null)
            return false;
        if (!string.Equals(key, _cachedKeyword, StringComparison.Ordinal))
            return false;
        results = new List<Fatwa>(_cachedResults);
        return true;
    }
}
