using InnoClinicClient.Interfaces;
using InnoClinicClient.Models;

namespace InnoClinicClient.Services
{
    public class LocalPatientsService : IPatientsService
    {
        public Task<Patient> GetPatient()
        {
            return Task.FromResult<Patient>(new()
            {
                FirstName = "Tim",
                MiddleName = "Novikov",
                LastName = "Robilko",
                DateOfBirth = DateTime.Today,
                Id = Guid.NewGuid()
            });
        }
    }
}
