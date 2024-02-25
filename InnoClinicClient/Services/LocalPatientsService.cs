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
        public async Task<Patient> GetPatient()
        {
            await Task.Delay(500);

            var json = JsonSerializer.Serialize(_patient);

            var newPatient = JsonSerializer.Deserialize<Patient>(json);

            return await Task.FromResult(newPatient!);
        }
        public async Task SavePatient(Patient patient)
        {
            await Task.Delay(500);

            var json = JsonSerializer.Serialize(patient);

            _patient = JsonSerializer.Deserialize<Patient>(json)!;
        }
    }
}
