namespace MyMauiApp.Services;

public class SettingsService
{
    private const string FontSizeKey = "fontSize";
    private const string ThemeKey = "theme";

    public async Task<string> GetFontSizeAsync()
    {
        return await SecureStorage.GetAsync(FontSizeKey) ?? "Medium";
    }

    public async Task SetFontSizeAsync(string fontSize)
    {
        await SecureStorage.SetAsync(FontSizeKey, fontSize);
    }

    public async Task<string> GetThemeAsync()
    {
        return await SecureStorage.GetAsync(ThemeKey) ?? "Light";
    }

    public async Task SetThemeAsync(string theme)
    {
        await SecureStorage.SetAsync(ThemeKey, theme);
    }
}

