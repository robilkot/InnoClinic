namespace InnoClinicClient.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string username, string password, bool rememberLogin);
        Task<bool> LogoutAsync();
        Task<bool> RegisterAsync(string username, string password);
    }
}