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
    public interface IUserServices
    {
        Task<ResponseModel<UserServicesItem>> CreateAsync(BookRequestParam item, SecurityModel security);
        Task<ResponseModel<List<BookDetails>>> GetAsync(SearchParam fipso, SecurityModel securityModel);
    }
}
