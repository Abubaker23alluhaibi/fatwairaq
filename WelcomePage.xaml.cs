using System.Linq;

namespace MyMauiApp;

public partial class WelcomePage : ContentPage
{
	public WelcomePage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		// تحميل صورة الخلفية من Raw asset (back.png)
		try
		{
			using var stream = await FileSystem.OpenAppPackageFileAsync("back.png");
			if (stream != null)
			{
				using var ms = new MemoryStream();
				await stream.CopyToAsync(ms);
				ms.Position = 0;
				BackgroundImage.Source = ImageSource.FromStream(() => new MemoryStream(ms.ToArray()));
			}
		}
		catch { /* خلفية الصفحة تبقى لون كريمي */ }

		// أنيميشن ظهور النص الترحيبي بالأخضر
		var label = WelcomeLabel;
		label.Opacity = 0;
		label.Scale = 0.8;
		await Task.Delay(400);
		_ = label.FadeTo(1, 800, Easing.CubicOut);
		_ = label.ScaleTo(1, 800, Easing.CubicOut);
		// بعد انتهاء الترحيب الانتقال للتطبيق
		await Task.Delay(2500);
		GoToApp();
	}

	private void GoToApp()
	{
		var window = Application.Current?.Windows?.FirstOrDefault();
		if (window != null)
			window.Page = new AppShell();
	}
}
