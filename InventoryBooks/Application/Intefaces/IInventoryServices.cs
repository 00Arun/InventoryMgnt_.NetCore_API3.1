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
    public interface IInventoryServices
    {
        Task<ResponseModel<BookDetails>> CreateAsync(BookDetails item, SecurityModel security);
        Task<ResponseModel<List<BookDetails>>> GetAsync(SearchParam fipso, SecurityModel security);
        Task<ResponseModel<BookDetails>> GetAsync(string id);
        Task<ResponseModel<BookDetails>> DeleteAsync(string id);
        Task<ResponseModel<BookDetails>> UpdateAsync(BookDetails entity, SecurityModel security);
        Task<ResponseModel<BookDetails>> SoftDeleteAsync(string id, SecurityModel security);
        Task<ResponseModel<BookDetails>> InventoryReturnEntry(InverntoryReturnRequest entity, SecurityModel securityModel);
        Task<ResponseModel<List<InventoryHistory>>> GetHistoryLog(SearchParam fipso, SecurityModel securityModel);
    }
}
