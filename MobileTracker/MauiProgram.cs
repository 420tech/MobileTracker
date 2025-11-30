using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MobileTracker.Views;
using MobileTracker.ViewModels;
using MobileTracker.Services;

namespace MobileTracker;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			   .UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});


			   // Register ViewModels
			   builder.Services.AddTransient<DashboardViewModel>();
			   builder.Services.AddTransient<TimeTrackerViewModel>();
			   builder.Services.AddTransient<ClientViewModel>();
			   builder.Services.AddTransient<InvoiceViewModel>();
			   builder.Services.AddTransient<SettingsViewModel>();
			   builder.Services.AddTransient<LoginViewModel>();
			   builder.Services.AddTransient<RegistrationViewModel>();

			   // Register Views
			   builder.Services.AddTransient<DashboardPage>();
			   builder.Services.AddTransient<TimeTrackerPage>();
			   builder.Services.AddTransient<ClientPage>();
			   builder.Services.AddTransient<InvoicePage>();
			   builder.Services.AddTransient<SettingsPage>();
			   builder.Services.AddTransient<LoginPage>();
			   builder.Services.AddTransient<RegistrationPage>();

			   // Register Auth Service
			   builder.Services.AddSingleton<IAuthService, FirebaseAuthService>();


#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
