using CommunityToolkit.Mvvm.ComponentModel;
using InnoClinicClient.Interfaces;
using InnoClinicClient.Models;

namespace InnoClinicClient.ViewModel
{
    public partial class PatientProfileViewModel : BaseViewModel
    {
        private readonly IPatientsService _patientsService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotEditing))]
        private bool _isEditing;
        public bool IsNotEditing => !IsEditing;

        [ObservableProperty]
        private Patient _patient;
        public PatientProfileViewModel(IPatientsService patientsService)
        {
            Title = "Patient profile";
            _patientsService = patientsService;

            Task.Run(GetProfileAsync);
        }

        public async Task SaveProfileAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                await _patientsService.SavePatient(Patient);
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

        //[RelayCommand]
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
    }
}
