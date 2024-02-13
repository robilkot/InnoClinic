using CommunityToolkit.Mvvm.ComponentModel;
using InnoClinicClient.Interfaces;
using InnoClinicClient.Models;

namespace InnoClinicClient.ViewModel
{
    public partial class PatientProfileViewModel : BaseViewModel
    {
        private readonly IPatientsService _patientsService;

        [ObservableProperty]
        private Patient _patient;
        public PatientProfileViewModel(IPatientsService patientsService)
        {
            Title = "Patient profile";
            _patientsService = patientsService;

            Task.Run(GetProfileAsync);
        }

        //[RelayCommand]
        async Task GetProfileAsync()
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
