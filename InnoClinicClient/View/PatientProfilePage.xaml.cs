using InnoClinicClient.ViewModel;
using Microsoft.Web.WebView2.Core;

namespace InnoClinicClient.View;

public partial class PatientProfilePage : ContentPage
{
	private PatientProfileViewModel _viewModel;
	public PatientProfilePage(PatientProfileViewModel viewModel)
	{
		InitializeComponent();

		_viewModel = viewModel;
		BindingContext = viewModel;
	}

    private void editProfileButton_Clicked(object sender, EventArgs e)
    {
        firstNameEntry.Text = _viewModel.Patient.FirstName;
        middleNameEntry.Text = _viewModel.Patient.MiddleName;
        lastNameEntry.Text = _viewModel.Patient.LastName;

        _viewModel.IsEditing = true;
    }

    private async void saveProfileEditButton_Clicked(object sender, EventArgs e)
    {
        _viewModel.Patient.FirstName = firstNameEntry.Text;
        _viewModel.Patient.MiddleName = middleNameEntry.Text;
        _viewModel.Patient.LastName = lastNameEntry.Text;

        await _viewModel.SaveProfileAsync();

        await _viewModel.GetProfileAsync();

        _viewModel.IsEditing = false;
    }

    private void discardProfileEditButton_Clicked(object sender, EventArgs e)
    {
        _viewModel.IsEditing = false;
    }
}