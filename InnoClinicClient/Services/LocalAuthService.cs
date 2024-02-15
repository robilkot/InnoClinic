using InnoClinicClient.Interfaces;

namespace InnoClinicClient.Services
{
    public class LocalAuthService : IAuthService
    {
        private readonly bool _isAuthenticated = false;
        public Task<bool> IsAuthenticated()
        {
            return Task.FromResult(_isAuthenticated);
        }

        public Task<bool> LoginAsync(string username, string password, bool rememberLogin)
        {
            return Task.FromResult(true);
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
