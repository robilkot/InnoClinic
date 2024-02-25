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

            builder.Services.AddTransient<HttpClient>();
            builder.Services.AddSingleton<IAuthService, LocalAuthService>();
            builder.Services.AddSingleton<IPatientsService, LocalPatientsService>();

            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<PatientProfileViewModel>();
            builder.Services.AddSingleton<AppointmentsViewModel>();

            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<PatientProfilePage>();
            builder.Services.AddSingleton<AppointmentsPage>();

            return builder.Build();
        }
    }
}
