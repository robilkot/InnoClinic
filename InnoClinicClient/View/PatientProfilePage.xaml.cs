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
}