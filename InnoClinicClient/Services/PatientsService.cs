using InnoClinicClient.Helpers;
using InnoClinicClient.Interfaces;
using InnoClinicClient.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static InnoClinicClient.Constants.Urls;

namespace InnoClinicClient.Services
{
    public class PatientsService : IPatientsService
    {
        private readonly HttpClient _httpClient;
        public PatientsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(4);
        }
        public async Task<Patient> GetPatient()
        {
            var token = await JwtHandlerHelper.GetTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(BaseUrl + Patients);

            if (response.IsSuccessStatusCode)
            {
                return (Patient)await response.Content.ReadFromJsonAsync(typeof(Patient));
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }
    }
}
