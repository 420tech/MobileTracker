using Microsoft.Maui.Controls;
using MobileTracker.ViewModels;

namespace MobileTracker.Views
{
    public partial class ClientPage : ContentPage
    {
        public ClientPage(ClientViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ClientViewModel vm)
            {
                _ = vm.LoadClientsAsync();
            }
        }

        // Navigation buttons removed for clean UI
    }
}