using InnoClinicClient.Interfaces;
using InnoClinicClient.Services;
using InnoClinicClient.View;
using InnoClinicClient.ViewModel;
using Microsoft.Extensions.Logging;

namespace InnoClinicClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddTransient<HttpClient>();

            builder.Services.AddTransient<IAuthService, LocalAuthService>();
            builder.Services.AddTransient<LoginViewModel>();

            builder.Services.AddTransient<IPatientsService, LocalPatientsService>();

            builder.Services.AddTransient<PatientProfileViewModel>();
            builder.Services.AddTransient<PatientProfilePage>();

            return builder.Build();
        }
    }
}
