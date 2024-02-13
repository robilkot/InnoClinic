using InnoClinicClient.Models;
using InnoClinicClient.ViewModel;

namespace InnoClinicClient.View;

public partial class PatientProfilePage : ContentPage
{
    public PatientProfilePage(PatientProfileViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        //if(BindingContext is  PatientProfileViewModel viewModel)
        //{
        //    await Shell.Current.DisplayAlert("wow", viewModel.Patient.Id.ToString(), "Ok");
        //}
    }
}