using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;
using OfficesService.Data.Models;
using CommonData.Messages;

namespace OfficesService.Models.MapperProfiles
{
    public class OfficesControllerProfile : Profile
    {
        public OfficesControllerProfile()
        {
            CreateMap<ClientOfficeModel, DbOfficeModel>().ReverseMap();
            CreateMap<OfficeUpdate, DbOfficeModel>().ReverseMap();
        }
    }
}
