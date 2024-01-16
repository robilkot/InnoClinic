using AutoMapper;

namespace ProfilesService.Models.MapperProfiles
{
    public class PatientsControllerProfile : Profile
    {
        public PatientsControllerProfile()
        {
            CreateMap<DbPatientModel, ClientPatientModel>().ReverseMap();
        }
    }
}
