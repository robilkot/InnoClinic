using AutoMapper;
using ServicesService.Domain.Entities;

namespace ServicesService.Presentation.Models.Mappers
{
    public class ControllerProfile : Profile
    {
        public ControllerProfile()
        {
            CreateMap<Service, ClientServiceModel>().ReverseMap();
            CreateMap<Specialization, ClientSpecializationModel>().ReverseMap();
        }
    }
}
