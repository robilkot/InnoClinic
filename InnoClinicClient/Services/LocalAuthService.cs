using InnoClinicClient.Interfaces;

namespace InnoClinicClient.Services
{
    public class LocalAuthService : IAuthService
    {
        private bool _isAuthenticated = false;
        public async Task<bool> IsAuthenticated()
        {
            await Task.Delay(500);
            return _isAuthenticated;
        }

        public async Task<bool> LoginAsync(string username, string password, bool rememberLogin)
        {
            await Task.Delay(500);
            _isAuthenticated = true;
            return true;
        }

        public async Task<bool> LogoutAsync()
        {
            await Task.Delay(500);
            _isAuthenticated = false;
            return true;
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            await Task.Delay(500);
            return true;
        }
    }
}
