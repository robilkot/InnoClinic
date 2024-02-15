using InnoClinicClient.View;

namespace InnoClinicClient
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AppointmentsPage), typeof(AppointmentsPage));
            Routing.RegisterRoute(nameof(PatientProfilePage), typeof(PatientProfilePage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        }
    }
}
