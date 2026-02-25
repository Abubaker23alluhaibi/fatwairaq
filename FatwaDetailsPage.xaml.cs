using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp;

[QueryProperty(nameof(FatwaId), "fatwaId")]
public partial class FatwaDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly FavoritesService _favoritesService;
    private readonly FatwaSelectionService _selectionService;
    private readonly BackNavigationService _backNav;
    private Fatwa? _fatwa;
    private int _currentFontSize = 16;

    public string? FatwaId
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                LoadFatwa(id);
            }
        }
    }

    public FatwaDetailsPage(ApiService apiService, FavoritesService favoritesService, FatwaSelectionService selectionService, BackNavigationService backNav)
    {
        InitializeComponent();
        _apiService = apiService;
        _favoritesService = favoritesService;
        _selectionService = selectionService;
        _backNav = backNav;
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

    private async void LoadFatwa(int fatwaId)
    {
        try
        {
            if (_selectionService.SelectedFatwa != null && _selectionService.SelectedFatwa.Id == fatwaId)
            {
                _fatwa = _selectionService.SelectedFatwa;
                _selectionService.SelectedFatwa = null;
            }
            else
            {
                // جلب فتوى واحدة فقط — أسرع بكثير من جلب جميع الفتاوى
                _fatwa = await _apiService.GetFatwaByIdAsync(fatwaId);
                if (_fatwa == null)
                {
                    var allFatwas = await _apiService.GetAllFatwasAsync();
                    _fatwa = allFatwas.FirstOrDefault(f => f.Id == fatwaId);
                }
            }

            if (_fatwa == null)
            {
                _fatwa = new Fatwa
                {
                    Id = fatwaId,
                    Title = "فتوى غير موجودة",
                    Question = "لا توجد معلومات متاحة حالياً",
                    Answer = "عذراً، لم نتمكن من العثور على تفاصيل هذه الفتوى."
                };
            }

            var fatwa = _fatwa;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    QuestionLabel.Text = fatwa.Question ?? "لا يوجد سؤال";
                    AnswerLabel.Text = fatwa.Answer ?? "لا يوجد جواب";
                    ScholarLabel.Text = !string.IsNullOrEmpty(fatwa.ScholarName) ? $"فضيلة الشيخ: {fatwa.ScholarName}" : "المجمع الفقهي العراقي";
                    CategoryLabel.Text = !string.IsNullOrEmpty(fatwa.CategoryName) ? $"التصنيف: {fatwa.CategoryName}" : "عام";
                    if (fatwa.Date.HasValue)
                        DateLabel.Text = $"نُشر بتاريخ: {fatwa.Date.Value:yyyy-MM-dd}";
                }
                catch { }
            });

            await UpdateFavoriteButton();
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(() =>
                DisplayAlert("خطأ", $"حدث خطأ أثناء تحميل الفتوى: {ex.Message}", "موافق"));
        }
    }

    private async Task UpdateFavoriteButton()
    {
        if (_fatwa == null) return;

        var isFavorite = await _favoritesService.IsFavoriteAsync(_fatwa.Id);
        var text = isFavorite ? "إزالة من المفضلة" : "حفظ في المفضلة";
        var bg = isFavorite ? Color.FromArgb("#E57373") : Color.FromArgb("#D4AF37");
        MainThread.BeginInvokeOnMainThread(() =>
        {
            FavoriteButton.Text = text;
            FavoriteButton.BackgroundColor = bg;
        });
    }

    private async void OnFavoriteClicked(object? sender, EventArgs e)
    {
        if (_fatwa == null) return;

        var isFavorite = await _favoritesService.IsFavoriteAsync(_fatwa.Id);
        
        if (isFavorite)
        {
            await _favoritesService.RemoveFromFavoritesAsync(_fatwa.Id);
            await DisplayAlert("تنبيه", "تمت الإزالة من المفضلة", "موافق");
        }
        else
        {
            await _favoritesService.AddToFavoritesAsync(_fatwa);
            await DisplayAlert("نجاح", "تم الحفظ في المفضلة", "موافق");
        }

        await UpdateFavoriteButton();
    }

    private async void OnShareClicked(object? sender, EventArgs e)
    {
        if (_fatwa == null) return;

        var shareText = $"{_fatwa.Title}\n\n" +
                       $"السؤال: {_fatwa.Question}\n\n" +
                       $"الجواب: {_fatwa.Answer}\n\n" +
                       $"المصدر: المجمع الفقهي العراقي";

        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = shareText,
            Title = "مشاركة فتوى شرعية"
        });
    }

    private void OnFontSizeSmall(object? sender, EventArgs e) => SetFontSize(14);
    private void OnFontSizeMedium(object? sender, EventArgs e) => SetFontSize(16);
    private void OnFontSizeLarge(object? sender, EventArgs e) => SetFontSize(22);

    private void SetFontSize(int size)
    {
        _currentFontSize = size;
        QuestionLabel.FontSize = size - 2; // السؤال دائماً أصغر قليلاً
        AnswerLabel.FontSize = size;
    }
}