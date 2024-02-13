namespace InnoClinicClient.Helpers
{
    public class JwtHandlerHelper
    {
        private const string TokenKey = "jwt_token";

        public static async Task SaveTokenAsync(string token)
        {
            await SecureStorage.SetAsync(TokenKey, token);
        }
        public static async Task<string?> GetTokenAsync()
        {
            return await SecureStorage.GetAsync(TokenKey);
        }
    }
}
