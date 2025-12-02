using System;
using MobileTracker.ViewModels;
using MobileTracker.Services;

namespace MobileTracker.Views
{
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage(RegistrationViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        // Parameterless constructor for Shell navigation — use DI only (no manual new)
        public RegistrationPage() : this(
            App.Services?.GetService<RegistrationViewModel>()
            ?? throw new InvalidOperationException("RegistrationViewModel not registered in DI. Ensure MauiProgram registers RegistrationViewModel in services.")
        )
        { }
    }
}