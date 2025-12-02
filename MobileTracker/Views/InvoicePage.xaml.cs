using Microsoft.Maui.Controls;
using MobileTracker.ViewModels;

namespace MobileTracker.Views
{
    public partial class InvoicePage : ContentPage
    {
        public InvoicePage(InvoiceViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is InvoiceViewModel vm)
            {
                _ = vm.LoadInvoicesAsync();
            }
        }

        // Navigation buttons removed for clean UI
    }
}