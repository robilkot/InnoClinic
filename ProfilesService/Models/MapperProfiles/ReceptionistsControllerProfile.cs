using AutoMapper;

namespace ProfilesService.Models.MapperProfiles
{
    public class ReceptionistsControllerProfile : Profile
    {
        public ReceptionistsControllerProfile()
        {
            CreateMap<DbReceptionistModel, ClientReceptionistModel>().ReverseMap();
        }
    }
}
