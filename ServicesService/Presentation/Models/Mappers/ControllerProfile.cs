using AutoMapper;
using CommonData.Messages;
using ServicesService.Domain.Entities;

namespace ServicesService.Presentation.Models.Mappers
{
    public class ControllerProfile : Profile
    {
        public ControllerProfile()
        {
            CreateMap<Service, ClientServiceModel>().ReverseMap();
            CreateMap<Specialization, ClientSpecializationModel>().ReverseMap();
            CreateMap<Category, ClientCategoryModel>().ReverseMap();

            CreateMap<Specialization, SpecializationUpdate>().ReverseMap();
            CreateMap<Service, ServiceUpdate>().ReverseMap();
        }
    }
}
