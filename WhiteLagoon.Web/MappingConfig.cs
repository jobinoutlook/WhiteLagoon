using WhiteLagoon.Domain.Entities;
using AutoMapper;

namespace WhiteLagoon.Web
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Villa, Villa>();
            // .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name));
        }


    }
}
