using InventoryBooks.Infrastructure.Entities;
using InventoryBooks.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Application.Intefaces
{
    public interface IReminderServices
    {
        Task<ResponseModel<List<InventoryHistory>>> SendReminderEmail();
    }
}
