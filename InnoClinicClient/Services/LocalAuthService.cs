using InnoClinicClient.Interfaces;

namespace InnoClinicClient.Services
{
    public class LocalAuthService : IAuthService
    {
        public Task<bool> LoginAsync(string username, string password, bool rememberLogin)
        {
            return Task.FromResult(username.Length > 3 && password.Length > 3);
        }

        public Task<bool> LogoutAsync()
        {
            return Task.FromResult(true);
        }

        public Task<bool> RegisterAsync(string username, string password)
        {
            return Task.FromResult(true);
        }
    }
}
