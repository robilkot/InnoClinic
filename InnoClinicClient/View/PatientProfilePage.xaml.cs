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

    protected override void OnAppearing()
    {
        Window.MinimumHeight = 600;
        Window.MinimumWidth = 500;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        Task.Run(_viewModel.GetProfileAsync);
    }
}