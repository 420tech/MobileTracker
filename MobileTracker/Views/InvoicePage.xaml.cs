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

        // Navigation buttons removed for clean UI
    }
}