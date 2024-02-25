using CommunityToolkit.Mvvm.ComponentModel;

namespace InnoClinicClient.ViewModel
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool _isBusy;
        [ObservableProperty]
        private string _title = string.Empty;
        public bool IsNotBusy => !IsBusy;
    }
}
