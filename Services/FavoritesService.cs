using System.Text.Json;
using MyMauiApp.Models;

namespace MyMauiApp.Services;

public class FavoritesService
{
    private const string FavoritesKey = "favorites";

    public async Task<List<Fatwa>> GetFavoritesAsync()
    {
        try
        {
            var favoritesJson = await SecureStorage.GetAsync(FavoritesKey);
            if (string.IsNullOrEmpty(favoritesJson))
                return new List<Fatwa>();

            var favorites = JsonSerializer.Deserialize<List<Fatwa>>(favoritesJson);
            return favorites ?? new List<Fatwa>();
        }
        catch
        {
            return new List<Fatwa>();
        }
    }

    public async Task AddToFavoritesAsync(Fatwa fatwa)
    {
        try
        {
            var favorites = await GetFavoritesAsync();
            
            // التحقق من عدم وجود الفتوى مسبقاً
            if (!favorites.Any(f => f.Id == fatwa.Id))
            {
                favorites.Add(fatwa);
                var favoritesJson = JsonSerializer.Serialize(favorites);
                await SecureStorage.SetAsync(FavoritesKey, favoritesJson);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding to favorites: {ex.Message}");
        }
    }

    public async Task RemoveFromFavoritesAsync(int fatwaId)
    {
        try
        {
            var favorites = await GetFavoritesAsync();
            favorites.RemoveAll(f => f.Id == fatwaId);
            
            var favoritesJson = JsonSerializer.Serialize(favorites);
            await SecureStorage.SetAsync(FavoritesKey, favoritesJson);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error removing from favorites: {ex.Message}");
        }
    }

    public async Task<bool> IsFavoriteAsync(int fatwaId)
    {
        try
        {
            var favorites = await GetFavoritesAsync();
            return favorites.Any(f => f.Id == fatwaId);
        }
        catch
        {
            return false;
        }
    }
}

