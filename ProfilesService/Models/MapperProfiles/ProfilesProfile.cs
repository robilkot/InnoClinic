using AutoMapper;
using ProfilesService.Data.Models;

namespace ProfilesService.Models.MapperProfiles
{
    public class ProfilesProfile : Profile
    {
        public ProfilesProfile()
        {
            CreateMap<DbDoctorModel, ClientDoctorModel>().ReverseMap();
            CreateMap<DbReceptionistModel, ClientReceptionistModel>().ReverseMap();
            CreateMap<DbPatientModel, ClientPatientModel>().ReverseMap();
        }
    }
}
