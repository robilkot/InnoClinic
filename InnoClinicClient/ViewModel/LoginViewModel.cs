using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InnoClinicClient.Interfaces;

namespace InnoClinicClient.ViewModel
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string _login = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _rememberLogin;
        public LoginViewModel(IAuthService authService)
        {
            Title = "Login";
            _authService = authService;
        }

        [RelayCommand]
        async Task LoginAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (await _authService.LoginAsync(Login, Password, RememberLogin))
                {
                    await Shell.Current.GoToAsync($"//MainPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error!", "Login failed. Please, check credentials", "Ok");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
