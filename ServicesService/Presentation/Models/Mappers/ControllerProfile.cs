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
            CreateMap<Service, ServiceUpdate>().ReverseMap();
            CreateMap<Service, ServiceRequest>();
            
            CreateMap<Specialization, ClientSpecializationModel>().ReverseMap();
            CreateMap<Specialization, SpecializationUpdate>().ReverseMap();
            CreateMap<Specialization, SpecializationRequest>();

            CreateMap<Category, ClientCategoryModel>().ReverseMap();
        }
    }
}
