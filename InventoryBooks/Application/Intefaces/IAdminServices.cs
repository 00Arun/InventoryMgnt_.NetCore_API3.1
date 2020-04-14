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
    public interface IAdminServices
    {
        Task<ResponseModel<List<UserServicesItem>>> GetAsync(SearchParam fipso, SecurityModel securityModel);
        Task<ResponseModel<string>> ApproveUserRequest(RequestApproveModel entity, SecurityModel securityModel);
    }
}
