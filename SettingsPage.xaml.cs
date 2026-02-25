using MyMauiApp.Services;

namespace MyMauiApp;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsService _settingsService;
    private readonly BackNavigationService _backNav;

    public SettingsPage(SettingsService settingsService, BackNavigationService backNav)
    {
        InitializeComponent();
        _settingsService = settingsService;
        _backNav = backNav;
        LoadSettings();
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        try
        {
            var route = _backNav.GetAndClearReturnRoute();
            await Shell.Current.GoToAsync(!string.IsNullOrEmpty(route) ? route : "//MainPage");
        }
        catch { await Shell.Current.GoToAsync("//MainPage"); }
    }

    private async void LoadSettings()
    {
        var fontSize = await _settingsService.GetFontSizeAsync();
        var theme = await _settingsService.GetThemeAsync();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            UpdateFontSizeButtons(fontSize);
            UpdateThemeButtons(theme);
        });
    }

    private void UpdateFontSizeButtons(string fontSize)
    {
        SmallFontButton.BackgroundColor = fontSize == "Small" ? Color.FromArgb("#0D4435") : Color.FromArgb("#E0E0E0");
        SmallFontButton.TextColor = fontSize == "Small" ? Colors.White : Color.FromArgb("#333");
        
        MediumFontButton.BackgroundColor = fontSize == "Medium" ? Color.FromArgb("#0D4435") : Color.FromArgb("#E0E0E0");
        MediumFontButton.TextColor = fontSize == "Medium" ? Colors.White : Color.FromArgb("#333");
        
        LargeFontButton.BackgroundColor = fontSize == "Large" ? Color.FromArgb("#0D4435") : Color.FromArgb("#E0E0E0");
        LargeFontButton.TextColor = fontSize == "Large" ? Colors.White : Color.FromArgb("#333");
    }

    private void UpdateThemeButtons(string theme)
    {
        LightThemeButton.BackgroundColor = theme == "Light" ? Color.FromArgb("#0D4435") : Color.FromArgb("#E0E0E0");
        LightThemeButton.TextColor = theme == "Light" ? Colors.White : Color.FromArgb("#333");
        
        DarkThemeButton.BackgroundColor = theme == "Dark" ? Color.FromArgb("#0D4435") : Color.FromArgb("#E0E0E0");
        DarkThemeButton.TextColor = theme == "Dark" ? Colors.White : Color.FromArgb("#333");
    }

    private async void OnFontSizeChanged(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            string fontSize = button.Text switch
            {
                "صغير" => "Small",
                "متوسط" => "Medium",
                "كبير" => "Large",
                _ => "Medium"
            };

            await _settingsService.SetFontSizeAsync(fontSize);
            UpdateFontSizeButtons(fontSize);
            await DisplayAlert("تم", "تم حفظ حجم الخط", "موافق");
        }
    }

    private async void OnThemeChanged(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            string theme = button.Text switch
            {
                "فاتح" => "Light",
                "داكن" => "Dark",
                _ => "Light"
            };

            await _settingsService.SetThemeAsync(theme);
            UpdateThemeButtons(theme);
            await DisplayAlert("تم", "تم حفظ المظهر. يرجى إعادة تشغيل التطبيق لتطبيق التغييرات", "موافق");
        }
    }

    private async void OnVisitWebsite(object? sender, EventArgs e)
    {
        try
        {
            await Launcher.Default.OpenAsync(new Uri("https://fatwairaq.com"));
        }
        catch (Exception ex)
        {
            await DisplayAlert("خطأ", $"لا يمكن فتح الموقع: {ex.Message}", "موافق");
        }
    }
}

