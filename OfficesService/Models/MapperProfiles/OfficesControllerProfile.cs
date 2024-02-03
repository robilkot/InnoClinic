using AutoMapper;
using CommonData.Messages;
using OfficesService.Data.Models;

namespace OfficesService.Models.MapperProfiles
{
    public class OfficesControllerProfile : Profile
    {
        public OfficesControllerProfile()
        {
            CreateMap<DbOfficeModel, ClientOfficeModel>().ReverseMap();
            CreateMap<DbOfficeModel, OfficeUpdate>().ReverseMap();
            CreateMap<DbOfficeModel, OfficeRequest>();
        }
    }
}
