using AutoMapper;
using Data.Models;
using Data.ViewModel;
using Data.ViewModel.OC;
using Data.ViewModel.Project;
using Data.ViewModel.Task;
using Data.ViewModel.User;
using Service.Helpers;
using System;
using System.Globalization;
using WorkManagement.Dtos;

namespace WorkManagement.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserForRegisterDto>();
            CreateMap<UserForRegisterDto, User>()
                .ForMember(x => x.Role, option => option.Ignore())
                .ForMember(x => x.Email, option => option.Ignore())
                .ForMember(x => x.OCID, option => option.Ignore());
            CreateMap<Task, CreateTaskViewModel>().ForMember(x => x.PIC, option => option.Ignore());
            CreateMap<CreateTaskViewModel, Task>()
                 .ForMember(dest => dest.DueDate,
                opt => opt.MapFrom(src => DateTime.ParseExact(src.Deadline, "dd/MM/yyyy", CultureInfo.InvariantCulture)));

            CreateMap<Task, TreeViewTask>();
                
            CreateMap<TreeViewTask, Task>();

            CreateMap<User, UserViewModel>();

            CreateMap<UserViewModel, User>();

            CreateMap<OC, CreateOCViewModel>();

            CreateMap<CreateOCViewModel, OC>();

            CreateMap<Project, ProjectViewModel>();
            CreateMap<ProjectViewModel, Project>()
                .ForMember(x => x.Managers, option => option.Ignore())
                .ForMember(x => x.TeamMembers, option => option.Ignore());


            CreateMap<TreeViewOC, TreeModel>();
            CreateMap<TreeModel, TreeViewOC>()
                .ForMember(x => x.Level, option => option.Ignore())
                .ForMember(x => x.Name, option => option.Ignore());



            //CreateMap<UserAccount, UserModel>();
            //CreateMap<RegisterModel, UserAccount>();
            //CreateMap<UpdateModel, UserAccount>();
        }
    }
}