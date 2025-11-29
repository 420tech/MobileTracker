using Microsoft.Maui.Controls;
using MobileTracker.ViewModels;

namespace MobileTracker.Views
{
    public partial class TimeTrackerPage : ContentPage
    {
        public TimeTrackerPage(TimeTrackerViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        // Navigation buttons removed for clean UI
    }
}