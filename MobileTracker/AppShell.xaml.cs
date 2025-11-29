using MobileTracker.Views;

namespace MobileTracker;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
		Routing.RegisterRoute(nameof(TimeTrackerPage), typeof(TimeTrackerPage));
		Routing.RegisterRoute(nameof(ClientPage), typeof(ClientPage));
		Routing.RegisterRoute(nameof(InvoicePage), typeof(InvoicePage));
		Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
	}
}
