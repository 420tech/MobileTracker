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

        // Parameterless constructor for Shell navigation
        public RegistrationPage() : this(
            App.Services?.GetService<RegistrationViewModel>()
            ?? new RegistrationViewModel(App.Services?.GetService<IAuthService>() ?? new FirebaseAuthService())
        )
        { }
    }
}