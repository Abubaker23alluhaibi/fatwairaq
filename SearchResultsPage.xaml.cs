using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp;

[QueryProperty(nameof(Keyword), "keyword")]
public partial class SearchResultsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly FatwaSelectionService _selectionService;
    private readonly BackNavigationService _backNav;
    private readonly SearchResultsCacheService _searchCache;
    private string _keyword = string.Empty;

    public string Keyword
    {
        get => _keyword;
        set
        {
            _keyword = value ?? string.Empty;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SearchKeywordLabel.Text = $"نتائج البحث: {_keyword}";
            });
            LoadSearchResults();
        }
    }

    public SearchResultsPage(ApiService apiService, FatwaSelectionService selectionService, BackNavigationService backNav, SearchResultsCacheService searchCache)
    {
        InitializeComponent();
        _apiService = apiService;
        _selectionService = selectionService;
        _backNav = backNav;
        _searchCache = searchCache;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!string.IsNullOrEmpty(_keyword) && ResultsCollectionView.ItemsSource == null && _searchCache.TryGet(_keyword, out var cached))
            MainThread.BeginInvokeOnMainThread(() => ApplyResults(cached!));
    }

    private async void LoadSearchResults()
    {
        if (string.IsNullOrEmpty(_keyword))
            return;

        if (_searchCache.TryGet(_keyword, out var cached))
        {
            MainThread.BeginInvokeOnMainThread(() => ApplyResults(cached!));
            return;
        }

        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadingIndicator.IsVisible = true;
                ResultsCollectionView.IsVisible = false;
                NoResultsLabel.IsVisible = false;
            });

            var results = await _apiService.SearchFatwasAsync(_keyword) ?? new List<Fatwa>();
            _searchCache.Set(_keyword, results);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    LoadingIndicator.IsVisible = false;
                    ResultsRefreshView.IsRefreshing = false;
                    ApplyResults(results);
                }
                catch (Exception ex)
                {
                    DisplayAlert("خطأ", $"حدث خطأ أثناء البحث: {ex.Message}", "موافق");
                }
            });
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadingIndicator.IsVisible = false;
                ResultsRefreshView.IsRefreshing = false;
                DisplayAlert("خطأ", $"حدث خطأ أثناء البحث: {ex.Message}", "موافق");
            });
        }
    }

    private void ApplyResults(List<Fatwa> results)
    {
        LoadingIndicator.IsVisible = false;
        ResultsRefreshView.IsRefreshing = false;
        if (results.Count > 0)
        {
            ResultsCollectionView.ItemsSource = results;
            ResultsCollectionView.IsVisible = true;
            NoResultsLabel.IsVisible = false;
        }
        else
        {
            ResultsCollectionView.IsVisible = false;
            NoResultsLabel.IsVisible = true;
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

    private async void OnFatwaSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0 || e.CurrentSelection[0] is not Fatwa selectedFatwa)
            return;
        ResultsCollectionView.SelectedItem = null;
        _selectionService.SelectedFatwa = selectedFatwa;
        // حفظ مسار الرجوع مع كلمة البحث لظهور النتائج من الذاكرة المؤقتة
        _backNav.SetReturnRoute($"//SearchResultsPage?keyword={Uri.EscapeDataString(_keyword)}");
        await Shell.Current.GoToAsync($"//FatwaDetailsPage?fatwaId={selectedFatwa.Id}");
    }

    private void OnRefresh(object? sender, EventArgs e)
    {
        LoadSearchResults();
    }
}

