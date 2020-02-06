using AutoMapper;
using Data.Models;
using Data.ViewModel.Task;
using WorkManagement.Dtos;

namespace WorkManagement.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserForRegisterDto>();
            CreateMap<UserForRegisterDto, User>()
                .ForMember(x => x.Role, option =>option.Ignore())
                .ForMember(x => x.Email, option => option.Ignore())
                .ForMember(x => x.OCID, option => option.Ignore());
            CreateMap<Task, CreateTaskViewModel>();
            CreateMap<CreateTaskViewModel, Task>();


            //CreateMap<UserAccount, UserModel>();
            //CreateMap<RegisterModel, UserAccount>();
            //CreateMap<UpdateModel, UserAccount>();
        }
    }
}