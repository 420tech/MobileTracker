using Microsoft.Maui.Controls;
using MobileTracker.ViewModels;

namespace MobileTracker.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage(SettingsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        // Navigation buttons removed for clean UI
    }
}