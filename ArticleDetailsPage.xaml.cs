using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp;

[QueryProperty(nameof(ArticleId), "articleId")]
public partial class ArticleDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly BackNavigationService _backNav;
    private Article? _article;

    public string? ArticleId
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                LoadArticle(id);
            }
        }
    }

    public ArticleDetailsPage(ApiService apiService, BackNavigationService backNav)
    {
        InitializeComponent();
        _apiService = apiService;
        _backNav = backNav;
    }

    private async void LoadArticle(int articleId)
    {
        try
        {
            var articles = await _apiService.GetArticlesAsync();
            _article = articles.FirstOrDefault(a => a.Id == articleId);

            if (_article == null)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("خطأ", "لم يتم العثور على المقال", "موافق");
                    await Shell.Current.GoToAsync("//MainPage");
                });
                return;
            }

            var article = _article;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    TitleLabel.Text = article.Title ?? "بدون عنوان";
                    ContentLabel.Text = article.Content ?? "لا يوجد محتوى";
                    if (!string.IsNullOrEmpty(article.Author))
                        AuthorLabel.Text = $"الكاتب: {article.Author}";
                    if (article.PublishDate.HasValue)
                        DateLabel.Text = $"تاريخ النشر: {article.PublishDate.Value:yyyy-MM-dd}";
                }
                catch { }
            });
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(() =>
                DisplayAlert("خطأ", $"حدث خطأ أثناء تحميل المقال: {ex.Message}", "موافق"));
        }
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
}

