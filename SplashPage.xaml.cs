namespace MyMauiApp;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
        
        // الانتقال التلقائي إلى الشاشة الرئيسية بعد 3 ثواني
        NavigateToMainPage();
    }
    
    private async void NavigateToMainPage()
    {
        try
        {
            // انتظار 3 ثواني
            await Task.Delay(3000);

            // الانتقال إلى الشاشة الرئيسية على خيط الواجهة (مهم على أندرويد)
            if (Application.Current?.Windows.Count > 0)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        Application.Current!.Windows[0].Page = new AppShell();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Splash navigation error: {ex}");
                    }
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Splash error: {ex}");
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Application.Current?.Windows.Count > 0)
                    Application.Current.Windows[0].Page = new AppShell();
            });
        }
    }
}

