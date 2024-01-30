using AppointmentsService.Data.Models;
using AutoMapper;

namespace AppointmentsService.Models.MapperProfiles
{
    public class AppointmentsControllerProfile : Profile
    {
        public AppointmentsControllerProfile()
        {
            CreateMap<ClientAppointment, DbAppointment>().ReverseMap();
        }
    }
}
