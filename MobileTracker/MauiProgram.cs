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
	// Try loading a .env file (if present) into environment variables so
	// things like FIREBASE_API_KEY are available at startup in dev/test scenarios.
	Services.EnvLoader.LoadDotEnvIfExists();
		var builder = MauiApp.CreateBuilder();
		builder
			   .UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

			   builder.Services.AddTransient<DashboardViewModel>();
			   builder.Services.AddTransient<TimeTrackerViewModel>();
			   builder.Services.AddTransient<ClientViewModel>();
			   builder.Services.AddTransient<InvoiceViewModel>();
			   builder.Services.AddTransient<SettingsViewModel>();
			   builder.Services.AddTransient<LoginViewModel>();
			   builder.Services.AddTransient<RegistrationViewModel>();
			   builder.Services.AddTransient<PasswordResetViewModel>();

			   // Register Views
			   builder.Services.AddTransient<DashboardPage>();
			   builder.Services.AddTransient<TimeTrackerPage>();
			   builder.Services.AddTransient<ClientPage>();
			   builder.Services.AddTransient<InvoicePage>();
			   builder.Services.AddTransient<SettingsPage>();
			   builder.Services.AddTransient<LoginPage>();
			   builder.Services.AddTransient<RegistrationPage>();
			   builder.Services.AddTransient<PasswordResetPage>();

			   // Register a singleton HttpClient and Auth Service (pass ILogger from DI).
			   // Keep singletons so auth state and HttpClient are preserved across app lifetime.
			   builder.Services.AddSingleton<System.Net.Http.HttpClient>();
			   builder.Services.AddSingleton<IAuthService>(sp =>
				   new FirebaseAuthService(sp.GetRequiredService<System.Net.Http.HttpClient>(), sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<FirebaseAuthService>>()));

			// Register a navigation abstraction backed by Shell for platform runtime
			builder.Services.AddSingleton<INavigationService, ShellNavigationService>();

			// Register Realtime Database service which will use the FIREBASE_DATABASE_URL env var and the current authenticated user's id token
			// Use the non-generic overload to avoid ambiguity with toolkit's AddSingleton<TView,TViewModel>
			builder.Services.AddSingleton(typeof(MobileTracker.Services.IFirebaseDatabaseService), typeof(MobileTracker.Services.FirebaseRealtimeDatabaseService));


	// Add Console logs so when running with `dotnet run` or in CI we see log output
	builder.Logging.AddConsole();
#if DEBUG
	// Show debug-level logs locally so request-level Debug messages are visible during development.
	builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
	builder.Logging.AddDebug();
#endif

		var app = builder.Build();

		// Try to log whether we have a Firebase API key available at startup.
		try
		{
			var loggerFactory = app.Services.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();
			var logger = loggerFactory?.CreateLogger("Startup");
			var hasKey = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("FIREBASE_API_KEY"));
			logger?.LogInformation("FIREBASE_API_KEY present: {HasKey}", hasKey);
		}
		catch
		{
			// best-effort logging — don't crash startup if logging isn't available yet
		}

		return app;
	}
}
