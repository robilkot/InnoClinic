using InnoClinicClient.Helpers;
using InnoClinicClient.Interfaces;
using static InnoClinicClient.Constants.Urls;

namespace InnoClinicClient.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(4);
        }

        public async Task<bool> LoginAsync(string username, string password, bool rememberLogin)
        {
            var response = await _httpClient.PostAsync($"{IdentityUrl}/account/",
                new StringContent($"{{\"username\":\"{username}\",\"password\":\"{password}\",\"rememberlogin\":\"{rememberLogin}\"}}"));

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();

                await JwtHandlerHelper.SaveTokenAsync(token);

                return true;
            }
            else
            {
                return false;
            }
        }

        public Task<bool> LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterAsync(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
