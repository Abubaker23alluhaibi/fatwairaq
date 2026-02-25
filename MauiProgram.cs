using Microsoft.Extensions.Logging;
using MyMauiApp.Services;

namespace MyMauiApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("Cairo-Regular.ttf", "CairoRegular");
			});

		// تسجيل الخدمات
		builder.Services.AddSingleton<ApiService>();
		builder.Services.AddSingleton<FavoritesService>();
		builder.Services.AddSingleton<FatwaSelectionService>();
		builder.Services.AddSingleton<BackNavigationService>();
		builder.Services.AddSingleton<SearchResultsCacheService>();
		builder.Services.AddSingleton<SettingsService>();
		
		// تسجيل الصفحات
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddSingleton<SearchResultsPage>();
		builder.Services.AddTransient<FatwasListPage>();
		builder.Services.AddTransient<FatwaDetailsPage>();
		builder.Services.AddTransient<ScholarsPage>();
		builder.Services.AddTransient<ArticlesPage>();
		builder.Services.AddTransient<ArticleDetailsPage>();
		builder.Services.AddTransient<FavoritesPage>();
		builder.Services.AddTransient<SettingsPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
