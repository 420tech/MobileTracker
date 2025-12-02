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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is TimeTrackerViewModel vm)
            {
                _ = vm.LoadEntriesAsync();
            }
        }

        // Navigation buttons removed for clean UI
    }
}