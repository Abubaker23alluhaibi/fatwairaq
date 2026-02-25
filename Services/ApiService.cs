using System.Text.Json;
using MyMauiApp.Models;

namespace MyMauiApp.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://fatwairaq.com";

    private List<Category>? _cachedCategories;
    private DateTime _categoriesCacheTime = DateTime.MinValue;
    private static readonly TimeSpan CategoriesCacheDuration = TimeSpan.FromMinutes(10);

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(25)
        };
    }

    /// <summary>جلب فتوى واحدة بالمعرف — أسرع بكثير من جلب الكل.</summary>
    public async Task<Fatwa?> GetFatwaByIdAsync(int fatwaId)
    {
        try
        {
            var url = $"/Fatwas/DetailsAsJson/{fatwaId}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;
            var json = await response.Content.ReadAsStringAsync();
            var fatwa = JsonSerializer.Deserialize<Fatwa>(json, _jsonOptions);
            return fatwa;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting fatwa by id: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Fatwa>> SearchFatwasAsync(string keyword)
    {
        try
        {
            var url = $"/Fatwas/SearchAsJson?keyword={Uri.EscapeDataString(keyword)}";
            var response = await _httpClient.GetStringAsync(url);
            var fatwas = JsonSerializer.Deserialize<List<Fatwa>>(response, _jsonOptions);
            return fatwas ?? new List<Fatwa>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error searching fatwas: {ex.Message}");
            return new List<Fatwa>();
        }
    }

    public async Task<List<Fatwa>> GetFatwasByCategoryAsync(int categoryId)
    {
        try
        {
            var url = $"/Fatwas/IndexByCategoryAsJson/{categoryId}";
            var response = await _httpClient.GetStringAsync(url);
            var fatwas = JsonSerializer.Deserialize<List<Fatwa>>(response, _jsonOptions);
            return fatwas ?? new List<Fatwa>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting fatwas by category: {ex.Message}");
            return new List<Fatwa>();
        }
    }

    public async Task<List<Fatwa>> GetFatwasByScholarAsync(int scholarId)
    {
        try
        {
            var url = $"/Fatwas/IndexByScholarsAsJson/{scholarId}";
            var response = await _httpClient.GetStringAsync(url);
            var fatwas = JsonSerializer.Deserialize<List<Fatwa>>(response, _jsonOptions);
            return fatwas ?? new List<Fatwa>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting fatwas by scholar: {ex.Message}");
            return new List<Fatwa>();
        }
    }

    public async Task<List<Fatwa>> GetAllFatwasAsync()
    {
        try
        {
            var url = "/Fatwas/IndexAsJson";
            var response = await _httpClient.GetStringAsync(url);
            var fatwas = JsonSerializer.Deserialize<List<Fatwa>>(response, _jsonOptions);
            return fatwas ?? new List<Fatwa>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting all fatwas: {ex.Message}");
            return new List<Fatwa>();
        }
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        if (_cachedCategories != null && DateTime.UtcNow - _categoriesCacheTime < CategoriesCacheDuration)
            return _cachedCategories;
        try
        {
            var url = "/Categories/IndexAsJson";
            var response = await _httpClient.GetStringAsync(url);
            var categories = JsonSerializer.Deserialize<List<Category>>(response, _jsonOptions) ?? new List<Category>();
            _cachedCategories = categories;
            _categoriesCacheTime = DateTime.UtcNow;
            return categories;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting categories: {ex.Message}");
            return _cachedCategories ?? new List<Category>();
        }
    }

    public async Task<List<Scholar>> GetScholarsAsync()
    {
        try
        {
            var url = "/Scholars/IndexAsJson";
            var response = await _httpClient.GetStringAsync(url);
            var scholars = JsonSerializer.Deserialize<List<Scholar>>(response, _jsonOptions);
            return scholars ?? new List<Scholar>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting scholars: {ex.Message}");
            return new List<Scholar>();
        }
    }

    public async Task<List<Article>> GetArticlesAsync()
    {
        try
        {
            var url = "/Articles/ListAsJson";
            var response = await _httpClient.GetStringAsync(url);
            var articles = JsonSerializer.Deserialize<List<Article>>(response, _jsonOptions);
            return articles ?? new List<Article>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting articles: {ex.Message}");
            return new List<Article>();
        }
    }
}
