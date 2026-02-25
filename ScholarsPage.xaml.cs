using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp;

public partial class ScholarsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly BackNavigationService _backNav;

    public ScholarsPage(ApiService apiService, BackNavigationService backNav)
    {
        InitializeComponent();
        _apiService = apiService;
        _backNav = backNav;
        LoadScholars();
    }

    private async void LoadScholars()
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadingIndicator.IsVisible = true;
                ScholarsCollectionView.IsVisible = false;
                NoResultsLabel.IsVisible = false;
            });

            var scholars = await _apiService.GetScholarsAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    LoadingIndicator.IsVisible = false;
                    if (scholars.Count > 0)
                    {
                        ScholarsCollectionView.ItemsSource = scholars;
                        ScholarsCollectionView.IsVisible = true;
                    }
                    else
                    {
                        NoResultsLabel.IsVisible = true;
                    }
                }
                catch (Exception ex)
                {
                    DisplayAlert("خطأ", $"حدث خطأ أثناء تحميل العلماء: {ex.Message}", "موافق");
                }
            });
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadingIndicator.IsVisible = false;
                DisplayAlert("خطأ", $"حدث خطأ أثناء تحميل العلماء: {ex.Message}", "موافق");
            });
        }
    }

    private async void OnScholarSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0 && e.CurrentSelection[0] is Scholar selectedScholar)
        {
            ScholarsCollectionView.SelectedItem = null;
            var name = Uri.EscapeDataString(selectedScholar.Name ?? "");
            _backNav.SetReturnRoute(Shell.Current.CurrentState.Location.ToString());
            await Shell.Current.GoToAsync($"//FatwasListPage?scholarId={selectedScholar.Id}&scholarName={name}");
        }
    }

    private async void OnRefresh(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        LoadScholars();
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

