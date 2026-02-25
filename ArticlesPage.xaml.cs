using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp;

public partial class ArticlesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly BackNavigationService _backNav;

    public ArticlesPage(ApiService apiService, BackNavigationService backNav)
    {
        InitializeComponent();
        _apiService = apiService;
        _backNav = backNav;
        LoadArticles();
    }

    private async void LoadArticles()
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadingIndicator.IsVisible = true;
                ArticlesCollectionView.IsVisible = false;
                NoResultsLabel.IsVisible = false;
            });

            var articles = await _apiService.GetArticlesAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    LoadingIndicator.IsVisible = false;
                    if (articles.Count > 0)
                    {
                        ArticlesCollectionView.ItemsSource = articles;
                        ArticlesCollectionView.IsVisible = true;
                    }
                    else
                    {
                        NoResultsLabel.IsVisible = true;
                    }
                }
                catch (Exception ex)
                {
                    DisplayAlert("خطأ", $"حدث خطأ أثناء تحميل المقالات: {ex.Message}", "موافق");
                }
            });
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadingIndicator.IsVisible = false;
                DisplayAlert("خطأ", $"حدث خطأ أثناء تحميل المقالات: {ex.Message}", "موافق");
            });
        }
    }

    private async void OnArticleSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0 && e.CurrentSelection[0] is Article selectedArticle)
        {
            ArticlesCollectionView.SelectedItem = null;
            _backNav.SetReturnRoute(Shell.Current.CurrentState.Location.ToString());
            await Shell.Current.GoToAsync($"//ArticleDetailsPage?articleId={selectedArticle.Id}");
        }
    }

    private async void OnRefresh(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        LoadArticles();
        RefreshView.IsRefreshing = false;
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

