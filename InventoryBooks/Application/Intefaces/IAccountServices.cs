using InventoryBooks.Infrastructure.Entities;
using InventoryBooks.Models;
using InventoryBooks.Models.Common;
using InventoryBooks.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Application.Intefaces
{
    public interface IAccountServices
    {
        Task<ResponseModel<IdentityViewModel>> Register(UserSignUpModel identity);
        Task<ResponseModel<LoginUserViewModel>> LoginAsync(Credentials model);
        Task<ResponseModel<IdentityViewModel>> GenerateAdminUsers();
    }
}
