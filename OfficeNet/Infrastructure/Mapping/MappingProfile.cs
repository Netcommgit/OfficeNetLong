using AutoMapper;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;

namespace OfficeNet.Infrastructure.Mapping
{
    public class MappingProfile :Profile
    {
        public MappingProfile() 
        {
            CreateMap<ApplicationUser, UserResponse>();
            CreateMap<ApplicationUser, CurrentUserResponse>();
            CreateMap<UserRegisterRequest, ApplicationUser>();
            //CreateMap<ThoughtSaveModel, ThoughtOfDay>();
            CreateMap<ThoughtOfDay, ThoughtSaveModel>();
        }
    }
}
