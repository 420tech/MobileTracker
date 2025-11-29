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

        // Navigation buttons removed for clean UI
    }
}