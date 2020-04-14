using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.ViewModels
{
    public class RequestApproveModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string BookId { get; set; }
        public string Status { get; set; }
    }
}
