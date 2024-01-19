using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;
using OfficesService.Data.Models;

namespace OfficesService.Models.MapperProfiles
{
    public class OfficesControllerProfile : Profile
    {
        public OfficesControllerProfile()
        {
            CreateMap<ClientImageModel, DbImageModel>().ReverseMap();
            CreateMap<ClientOfficeModel, DbOfficeModel>().ReverseMap();
        }
    }
}
