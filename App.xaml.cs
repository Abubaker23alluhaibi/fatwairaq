namespace MyMauiApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// التقاط أي خطأ غير معالج حتى لا يغلق التطبيق فجأة (خصوصاً على أندرويد)
		AppDomain.CurrentDomain.UnhandledException += (_, e) =>
		{
			var ex = (Exception)e.ExceptionObject;
			System.Diagnostics.Debug.WriteLine($"Unhandled: {ex}");
		};
		TaskScheduler.UnobservedTaskException += (_, e) =>
		{
			e.SetObserved();
			System.Diagnostics.Debug.WriteLine($"Unobserved task: {e.Exception}");
		};
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		try
		{
			// أول ما يفتح التطبيق: شاشة ترحيب (صورة المجمع + كتابة أنيميشن خضراء) ثم Shell
			return new Window(new WelcomePage());
		}
		catch (Exception ex)
		{
			return new Window(new ContentPage
			{
				BackgroundColor = Color.FromArgb("#F5F6F2"),
				Content = new ScrollView
				{
					Content = new Label
					{
						Text = "خطأ عند فتح التطبيق:\n\n" + ex.Message,
						Margin = new Thickness(20),
						TextColor = Color.FromArgb("#0D4435")
					}
				}
			});
		}
	}
}