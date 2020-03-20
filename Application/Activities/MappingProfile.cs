using AutoMapper;
using Domain;

namespace Application.Activities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Activity, ActivityDto>();
            CreateMap<UserActivity, AttendeeDto>()
                .ForMember(ad => ad.UserName, opt => opt.MapFrom(ua => ua.AppUser.UserName))
                .ForMember(ad => ad.DisplayName, opt => opt.MapFrom(ua => ua.AppUser.DisplayName));
        }
    }
}