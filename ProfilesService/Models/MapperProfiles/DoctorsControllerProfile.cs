using AutoMapper;
using ProfilesService.Models;

namespace ProfilesService.Models.MapperProfiles
{
    public class DoctorsControllerProfile : Profile
    {
        public DoctorsControllerProfile()
        {
            CreateMap<DbDoctorModel, ClientDoctorModel>().ReverseMap();
        }
    }
}
