using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp;

public partial class MainPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly BackNavigationService _backNav;

    public MainPage(ApiService apiService, BackNavigationService backNav)
    {
        InitializeComponent();
        _apiService = apiService;
        _backNav = backNav;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadCategories();
    }

    private async void LoadCategories()
    {
        try
        {
            var categories = await _apiService.GetCategoriesAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    if (categories.Count == 0)
                    {
                        var defaultCategories = new List<Category>
                        {
                            new() { Id = 1, Name = "الصلاة" },
                            new() { Id = 2, Name = "الحج والعمرة" },
                            new() { Id = 3, Name = "المعاملات المالية" },
                            new() { Id = 4, Name = "الجنائز" },
                            new() { Id = 5, Name = "الأدعية والأذكار" },
                            new() { Id = 6, Name = "الزكاة" },
                            new() { Id = 7, Name = "الصيام" },
                            new() { Id = 8, Name = "الطهارة" },
                            new() { Id = 9, Name = "الأضحية" },
                            new() { Id = 10, Name = "قرارات المجمع" },
                            new() { Id = 11, Name = "فتاوى عامة" },
                            new() { Id = 12, Name = "الجنايات والعقوبات" },
                            new() { Id = 13, Name = "فقه الأسرة" },
                            new() { Id = 14, Name = "الوصايا والمواريث" },
                            new() { Id = 15, Name = "العقائد" },
                            new() { Id = 16, Name = "النكاح" },
                            new() { Id = 17, Name = "العدة والحضانة" },
                            new() { Id = 18, Name = "الطلاق" },
                            new() { Id = 19, Name = "مجلة الفتوى" },
                            new() { Id = 20, Name = "الربا" },
                            new() { Id = 21, Name = "الأطعمة والأشربة" },
                            new() { Id = 22, Name = "اللباس والزينة" },
                            new() { Id = 23, Name = "النذر والأيمان" },
                            new() { Id = 24, Name = "الأقضية" }
                        };
                        CategoriesCollectionView.ItemsSource = defaultCategories;
                    }
                    else
                    {
                        CategoriesCollectionView.ItemsSource = categories;
                    }
                    AnimateCategoriesIn();
                }
                catch (Exception ex)
                {
                    DisplayAlert("خطأ", $"حدث خطأ أثناء تحميل الأبواب: {ex.Message}", "موافق");
                }
            });
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(() =>
                DisplayAlert("خطأ", $"حدث خطأ أثناء تحميل الأبواب: {ex.Message}", "موافق"));
        }
    }

    private async void OnSearchClicked(object? sender, EventArgs e)
    {
        var keyword = SearchEntry.Text?.Trim();

        if (string.IsNullOrEmpty(keyword))
        {
            await DisplayAlert("تنبيه", "يرجى إدخال كلمة البحث", "موافق");
            return;
        }

        _backNav.SetReturnRoute(Shell.Current.CurrentState.Location.ToString());
        await Shell.Current.GoToAsync($"//SearchResultsPage?keyword={Uri.EscapeDataString(keyword)}");
    }

    private void AnimateCategoriesIn()
    {
        var cv = CategoriesCollectionView;
        cv.TranslationY = 35;
        cv.Opacity = 0;
        cv.Animate("CategoriesDropIn", d =>
        {
            cv.TranslationY = 35 - (35 * d);
            cv.Opacity = d;
        }, length: 350, easing: Easing.CubicOut);
    }

    private async void OnCategorySelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0 && e.CurrentSelection[0] is Category selectedCategory)
        {
            CategoriesCollectionView.SelectedItem = null;
            _backNav.SetReturnRoute(Shell.Current.CurrentState.Location.ToString());
            await Shell.Current.GoToAsync($"//FatwasListPage?categoryId={selectedCategory.Id}");
        }
    }

}