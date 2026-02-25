namespace MyMauiApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AppShell init: {ex}");
            throw;
        }
    }

    private async void OnPrivacyPolicyClicked(object? sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri("https://sites.google.com/view/fatawairaqpaicsy/home"));
    }
}
