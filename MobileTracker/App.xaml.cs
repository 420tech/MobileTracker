using MobileTracker.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;

namespace MobileTracker;


public partial class App : Application
{
	public static IServiceProvider? Services { get; private set; }

	       public App()
	       {
		       InitializeComponent();
		       // Set Services after MauiContext is available
		       this.HandlerChanged += (s, e) =>
		       {
			       if (this.Handler?.MauiContext?.Services != null)
				       Services = this.Handler.MauiContext.Services;
		       };

			   // deep links handled by overriding OnAppLinkRequestReceived
	       }

	protected override Window CreateWindow(IActivationState? activationState)
	{
	       var services = App.Services;
	       var authService = services.GetService<IAuthService>();
	       if (authService != null && authService.IsAuthenticated)
	       {
		       return new Window(new AppShell());
	       }
	       else
	       {
		       var loginPage = services.GetService<MobileTracker.Views.LoginPage>();
		       return new Window(new NavigationPage(loginPage));
	       }
	}

	protected override void OnAppLinkRequestReceived(Uri uri)
	{
		base.OnAppLinkRequestReceived(uri);

		// Handle Firebase password reset links
		if (uri.ToString().Contains("mode=resetPassword") || uri.ToString().Contains("oobCode"))
		{
			// Extract the oobCode (reset token) from the URL
			var query = uri.Query.TrimStart('?');
			var oobCode = string.Empty;

			foreach (var param in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
			{
				var parts = param.Split('=');
				if (parts.Length == 2 && parts[0] == "oobCode")
				{
					oobCode = Uri.UnescapeDataString(parts[1]);
					break;
				}
			}

			if (!string.IsNullOrEmpty(oobCode))
			{
				// Navigate to password reset page with the token
				var passwordResetPage = Services?.GetService<MobileTracker.Views.PasswordResetPage>();
				var passwordResetViewModel = Services?.GetService<MobileTracker.ViewModels.PasswordResetViewModel>();

				if (passwordResetViewModel != null)
				{
					passwordResetViewModel.SetResetToken(oobCode);
				}

				if (passwordResetPage != null)
				{
					MainThread.BeginInvokeOnMainThread(async () =>
					{
						await MainPage.Navigation.PushAsync(passwordResetPage);
					});
				}
			}
		}
	}
}