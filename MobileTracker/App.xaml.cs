using MobileTracker.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MobileTracker;


public partial class App : Application
{
       public static IServiceProvider Services { get; private set; }

	       public App()
	       {
		       InitializeComponent();
		       // Set Services after MauiContext is available
		       this.HandlerChanged += (s, e) =>
		       {
			       if (this.Handler?.MauiContext?.Services != null)
				       Services = this.Handler.MauiContext.Services;
		       };
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
}