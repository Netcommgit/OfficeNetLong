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
            CreateMap<DepartmentDto, UsersDepartment>();
            CreateMap<HelpdeskDepartmentDto, HelpdeskDepartmentModel>();
            CreateMap<HelpdeskDepartmentModel, HelpdeskDepartmentDto>();
            CreateMap<HelpdeskCategoryDto, HelpdeskCategoryModel>();
            CreateMap<HelpdeskCategoryModel, HelpdeskCategoryDto>();
            CreateMap<HelpdeskSubcategoryDto, HelpdeskSubcategoryModel>(); 
            CreateMap<HelpdeskSubcategoryModel, HelpdeskSubcategoryDto>();
            CreateMap<HelpDeskDetailDto, HelpDeskDetailModel>().ForMember(dest => dest.AdminUsers, opt => opt.Ignore()); ;
        }
    }
}
