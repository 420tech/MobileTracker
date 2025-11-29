using Microsoft.Maui.Controls;
using MobileTracker.ViewModels;

namespace MobileTracker.Views
{
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage(DashboardViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        // Navigation buttons removed for clean UI
    }
}