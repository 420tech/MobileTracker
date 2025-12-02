using System;
using MobileTracker.ViewModels;
using MobileTracker.Services;

namespace MobileTracker.Views;

public partial class PasswordResetPage : ContentPage
{
    public PasswordResetPage(PasswordResetViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Parameterless for Shell/DI fallback — use DI only
    public PasswordResetPage() : this(
        App.Services?.GetService<PasswordResetViewModel>()
        ?? throw new InvalidOperationException("PasswordResetViewModel not registered in DI. Ensure MauiProgram registers PasswordResetViewModel in services.")
    ) {}
}