using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp;

public partial class FavoritesPage : ContentPage
{
    private readonly FavoritesService _favoritesService;
    private readonly FatwaSelectionService _selectionService;
    private readonly BackNavigationService _backNav;

    public FavoritesPage(FavoritesService favoritesService, FatwaSelectionService selectionService, BackNavigationService backNav)
    {
        InitializeComponent();
        _favoritesService = favoritesService;
        _selectionService = selectionService;
        _backNav = backNav;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadFavorites();
    }

    private async void LoadFavorites()
    {
        try
        {
            var favorites = await _favoritesService.GetFavoritesAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    if (favorites.Count > 0)
                    {
                        FavoritesCollectionView.ItemsSource = favorites;
                        FavoritesCollectionView.IsVisible = true;
                        NoFavoritesLabel.IsVisible = false;
                    }
                    else
                    {
                        FavoritesCollectionView.IsVisible = false;
                        NoFavoritesLabel.IsVisible = true;
                    }
                }
                catch (Exception ex)
                {
                    DisplayAlert("خطأ", $"حدث خطأ أثناء تحميل المفضلة: {ex.Message}", "موافق");
                }
            });
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(() =>
                DisplayAlert("خطأ", $"حدث خطأ أثناء تحميل المفضلة: {ex.Message}", "موافق"));
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
            FavoritesCollectionView.SelectedItem = null;
            _selectionService.SelectedFatwa = selectedFatwa;
            _backNav.SetReturnRoute(Shell.Current.CurrentState.Location.ToString());
            await Shell.Current.GoToAsync($"//FatwaDetailsPage?fatwaId={selectedFatwa.Id}");
        }
    }

    private async void OnDeleteFavorite(object? sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.BindingContext is Fatwa fatwa)
        {
            await _favoritesService.RemoveFromFavoritesAsync(fatwa.Id);
            LoadFavorites();
        }
    }
}

