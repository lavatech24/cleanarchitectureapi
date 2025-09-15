using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Models.SystemModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<ApiUser, ApiUserDto>().ReverseMap();
            CreateMap<ApiUser, ApiUserCreateDto>().ReverseMap();
            CreateMap<ApiUser, ApiUserUpdateDto>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<ApiUser, ApiUserDeleteDto>().ReverseMap();
            CreateMap<ApiUser, ApiUserStatusDto>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<ApiUser, ApiUserRefreshTokenDto>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<ApiUser, ApiUserResponseDto>()
                .ForMember(dest => dest.Company, act => act.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.Role, act => act.MapFrom(src => src.Role.RoleName))
                .ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            //CreateMap<Company, CompanyResponseDto>().ForMember(dest => dest.list, act => act.MapFrom(src => src.ProgramCompanies.Select(p => new { p.Company }).ToList())).ReverseMap();
        }
    }
}
