using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp;

[QueryProperty(nameof(CategoryId), "categoryId")]
[QueryProperty(nameof(ScholarId), "scholarId")]
[QueryProperty(nameof(ScholarName), "scholarName")]
[QueryProperty(nameof(ShowAll), "showAll")]
public partial class FatwasListPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly FatwaSelectionService _selectionService;
    private readonly BackNavigationService _backNav;
    private int? _categoryId;
    private int? _scholarId;
    private string? _scholarName;
    private bool _showAll;
    private int _loadVersion;

    public string? CategoryId
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                _categoryId = id;
                _scholarId = null;
                _scholarName = null;
                _showAll = false;
                LoadFatwas();
            }
        }
    }

    public string? ScholarId
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                _scholarId = id;
                _categoryId = null;
                _showAll = false;
                LoadFatwas();
            }
        }
    }

    public string? ScholarName
    {
        set
        {
            _scholarName = string.IsNullOrEmpty(value) ? null : Uri.UnescapeDataString(value);
            if (_scholarId.HasValue)
                LoadFatwas();
        }
    }

    public string? ShowAll
    {
        set
        {
            if (bool.TryParse(value, out var show))
            {
                _showAll = show;
                _categoryId = null;
                _scholarId = null;
                LoadFatwas();
            }
        }
    }

    public FatwasListPage(ApiService apiService, FatwaSelectionService selectionService, BackNavigationService backNav)
    {
        InitializeComponent();
        _apiService = apiService;
        _selectionService = selectionService;
        _backNav = backNav;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_categoryId.HasValue && !_scholarId.HasValue && !_showAll)
        {
            _showAll = true;
            LoadFatwas();
        }
    }

    private async void LoadFatwas()
    {
        var thisVersion = ++_loadVersion;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            FatwasCollectionView.ItemsSource = null;
            FatwasCollectionView.IsVisible = false;
            NoResultsLabel.IsVisible = false;
            LoadingIndicator.IsVisible = true;
        });

        try
        {
            List<Fatwa> fatwas;

            if (_categoryId.HasValue)
            {
                fatwas = await _apiService.GetFatwasByCategoryAsync(_categoryId.Value);
            }
            else if (_scholarId.HasValue || !string.IsNullOrEmpty(_scholarName))
            {
                if (_scholarId.HasValue)
                    fatwas = await _apiService.GetFatwasByScholarAsync(_scholarId.Value);
                else
                    fatwas = new List<Fatwa>();

                if (fatwas.Count == 0 && !string.IsNullOrWhiteSpace(_scholarName))
                    fatwas = await _apiService.SearchFatwasAsync(_scholarName);
            }
            else if (_showAll)
            {
                fatwas = await _apiService.GetAllFatwasAsync();
            }
            else
            {
                fatwas = new List<Fatwa>();
            }

            var list = fatwas ?? new List<Fatwa>();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (thisVersion != _loadVersion) return;

                try
                {
                    LoadingIndicator.IsVisible = false;

                    if (_categoryId.HasValue)
                        HeaderLabel.Text = "فتاوى حسب الباب";
                    else if (_scholarId.HasValue || !string.IsNullOrEmpty(_scholarName))
                        HeaderLabel.Text = "فتاوى حسب العالم";
                    else if (_showAll)
                        HeaderLabel.Text = "جميع الفتاوى";

                    if (list.Count > 0)
                    {
                        FatwasCollectionView.ItemsSource = list;
                        FatwasCollectionView.IsVisible = true;
                        NoResultsLabel.IsVisible = false;
                    }
                    else
                    {
                        FatwasCollectionView.ItemsSource = null;
                        NoResultsText.Text = "غير متوفر أو هناك خطأ";
                        NoResultsLabel.IsVisible = true;
                    }
                }
                catch
                {
                    LoadingIndicator.IsVisible = false;
                    FatwasCollectionView.ItemsSource = null;
                    FatwasCollectionView.IsVisible = false;
                    NoResultsText.Text = "غير متوفر أو هناك خطأ";
                    NoResultsLabel.IsVisible = true;
                }
            });
        }
        catch
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (thisVersion != _loadVersion) return;
                LoadingIndicator.IsVisible = false;
                FatwasCollectionView.ItemsSource = null;
                FatwasCollectionView.IsVisible = false;
                NoResultsText.Text = "غير متوفر أو هناك خطأ";
                NoResultsLabel.IsVisible = true;
            });
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
        if (e.CurrentSelection.Count > 0 && e.CurrentSelection[0] is Fatwa selectedFatwa)
        {
            FatwasCollectionView.SelectedItem = null;
            _selectionService.SelectedFatwa = selectedFatwa;
            // حفظ مسار الرجوع مع السياق الحالي (باب/عالم/جميع) حتى يعود المستخدم لنفس القائمة
            var returnRoute = BuildCurrentListRoute();
            _backNav.SetExplicitReturnRoute(returnRoute);
            await Shell.Current.GoToAsync($"//FatwaDetailsPage?fatwaId={selectedFatwa.Id}");
        }
    }

    private string BuildCurrentListRoute()
    {
        if (_categoryId.HasValue)
            return $"//FatwasListPage?categoryId={_categoryId.Value}";
        if (_scholarId.HasValue)
        {
            var namePart = !string.IsNullOrEmpty(_scholarName)
                ? $"&scholarName={Uri.EscapeDataString(_scholarName!)}"
                : "";
            return $"//FatwasListPage?scholarId={_scholarId.Value}{namePart}";
        }
        return "//FatwasListPage?showAll=true";
    }

    private async void OnRefresh(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        LoadFatwas();
        RefreshView.IsRefreshing = false;
    }
}