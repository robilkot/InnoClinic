using InnoClinicClient.Models;

namespace InnoClinicClient.Interfaces
{
    public interface IPatientsService
    {
        Task<Patient> GetPatient();
        Task SavePatient(Patient patient);
    }
}