using CommunityToolkit.Mvvm.ComponentModel;
using InnoClinicClient.Models;
using System.Collections.ObjectModel;

namespace InnoClinicClient.ViewModel
{
    public partial class AppointmentsViewModel : BaseViewModel
    {

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotEditing))]
        private bool _isEditing;
        public bool IsNotEditing => !IsEditing;

        [ObservableProperty]
        private ObservableCollection<Appointment> _appointments;
        public AppointmentsViewModel()
        {
            Appointments = new();
            Title = "Appointments";
        }
    }
}
