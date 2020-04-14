using AutoMapper;
using InventoryBooks.Infrastructure.Entities;
using InventoryBooks.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserDetails, LoginUserViewModel>()
             .AfterMap((s, d) =>
             {

             });
            CreateMap<UserDetails, IdentityViewModel>()
              .AfterMap((s, d) =>
              {

              });
            CreateMap<IdentityViewModel, UserDetails>()
              .AfterMap((s, d) =>
              {

              });

            CreateMap<UserViewModel, UserDetails>()
             .AfterMap((s, d) =>
             {

             });
            CreateMap<UserDetails, UserViewModel>()
             .AfterMap((s, d) =>
             {

             });
            CreateMap<UserDetails, UserSignUpModel>()
            .AfterMap((s, d) =>
            {

            });
            CreateMap<UserSignUpModel, UserDetails>()
          .AfterMap((s, d) =>
          {

          });

        }

    }
}
