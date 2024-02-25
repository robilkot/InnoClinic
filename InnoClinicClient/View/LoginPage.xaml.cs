using InnoClinicClient.ViewModel;

namespace InnoClinicClient.View
{
    public partial class LoginPage : ContentPage
    {
        private readonly LoginViewModel _viewModel;
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            Window.MinimumHeight = 500;
            Window.MinimumWidth = 500;
        }

        private void RememberLoginLabelTapped(object sender, TappedEventArgs e)
        {
            _viewModel.RememberLogin = !_viewModel.RememberLogin;
        }

        private void TogglePasswordVisibility(object sender, EventArgs e)
        {
            passwordEntry.IsPassword = !passwordEntry.IsPassword;
        }
    }
}
