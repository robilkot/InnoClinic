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
}