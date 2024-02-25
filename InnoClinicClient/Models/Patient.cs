using CommunityToolkit.Mvvm.ComponentModel;

namespace InnoClinicClient.Models
{
    public partial class Patient : ObservableObject
    {
        public Guid? Id { get; set; }
        [ObservableProperty]
        private string _firstName = string.Empty;
        [ObservableProperty]
        private string _lastName = string.Empty;
        [ObservableProperty]
        private string? _middleName = string.Empty;
        public bool? LinkedToAccount { get; set; }
        [ObservableProperty]
        private DateTime _dateOfBirth;
        public Guid? AccountId { get; set; }
    }
}
