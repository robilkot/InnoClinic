using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InnoClinicClient.Interfaces;
using InnoClinicClient.Models;

namespace InnoClinicClient.ViewModel
{
    public partial class PatientProfileViewModel : BaseViewModel
    {
        private readonly IPatientsService _patientsService;
        private readonly IAuthService _authService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotEditing))]
        private bool _isEditing;
        public bool IsNotEditing => !IsEditing;

        [ObservableProperty]
        private Patient? _patient = null;

        [ObservableProperty]
        private string _firstNameEntryText = string.Empty;

        [ObservableProperty]
        private string _lastNameEntryText = string.Empty;

        [ObservableProperty]
        private string? _middleNameEntryText = string.Empty;
        public PatientProfileViewModel(IPatientsService patientsService, IAuthService authService)
        {
            Title = "Patient profile";

            _patientsService = patientsService;
            _authService = authService;
        }

        [RelayCommand]
        public async Task SaveProfileAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                Patient!.FirstName = FirstNameEntryText;
                Patient!.MiddleName = MiddleNameEntryText;
                Patient!.LastName = LastNameEntryText;

                await _patientsService.SavePatient(Patient);

                IsEditing = false;
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

        [RelayCommand]
        public async Task EditProfileAsync()
        {
            if (IsEditing || IsBusy)
                return;

            try
            {
                IsBusy = true;
                IsEditing = true;

                FirstNameEntryText = Patient!.FirstName;
                MiddleNameEntryText = Patient!.MiddleName;
                LastNameEntryText = Patient!.LastName;
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
        [RelayCommand]
        public async Task CancelProfileEditAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                IsEditing = false;
            }
        }

        [RelayCommand]
        public async Task GetProfileAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                Patient = await _patientsService.GetPatient();
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

        [RelayCommand]
        public async Task LogoutAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                await _authService.LogoutAsync();

                Patient = null;

                await Shell.Current.GoToAsync($"//LoginPage");
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
