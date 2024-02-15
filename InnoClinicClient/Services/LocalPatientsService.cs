using InnoClinicClient.Interfaces;
using InnoClinicClient.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InnoClinicClient.Services
{
    public class LocalPatientsService : IPatientsService
    {
        private Patient _patient = new()
        {
            FirstName = "Tim",
            MiddleName = "Novikov",
            LastName = "Robilko",
            DateOfBirth = DateTime.Today,
            Id = Guid.NewGuid()
        };
        public Task<Patient> GetPatient()
        {
            var json = JsonSerializer.Serialize(_patient);

            var newPatient = JsonSerializer.Deserialize<Patient>(json);

            return Task.FromResult(newPatient!);
        }
        public async Task SavePatient(Patient patient)
        {
            await Task.Delay(200);

            var json = JsonSerializer.Serialize(patient);

            _patient = JsonSerializer.Deserialize<Patient>(json)!;
        }
    }
}
