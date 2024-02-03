using AutoMapper;
using CommonData.Messages;
using ProfilesService.Data.Models;

namespace ProfilesService.Models.MapperProfiles
{
    public class ProfilesProfile : Profile
    {
        public ProfilesProfile()
        {
            CreateMap<DbDoctorModel, ClientDoctorModel>().ReverseMap();
            CreateMap<DbDoctorModel, DoctorUpdate>().ReverseMap();
            CreateMap<DbDoctorModel, DoctorRequest>();
            
            CreateMap<DbReceptionistModel, ClientReceptionistModel>().ReverseMap();

            CreateMap<DbPatientModel, ClientPatientModel>().ReverseMap();
            CreateMap<DbPatientModel, PatientUpdate>().ReverseMap();
            CreateMap<DbPatientModel, PatientRequest>();
        }
    }
}
