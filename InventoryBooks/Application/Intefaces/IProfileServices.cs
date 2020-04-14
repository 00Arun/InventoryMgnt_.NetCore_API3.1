using InventoryBooks.Infrastructure.Entities;
using InventoryBooks.Models.Common;
using InventoryBooks.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Application.Intefaces
{
    public interface IProfileServices
    {
        ResponseModel<UserViewModel> ChangePassword(ChangePasswordViewModel model);
        Task<ResponseModel<UserViewModel>> GetByIdAsync(string userId, SecurityModel securityModel);
        Task<ResponseModel<UserViewModel>> UpdateAsync(UserSignUpModel entity, SecurityModel securityModel);
    }
}
