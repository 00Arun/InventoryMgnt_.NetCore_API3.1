using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Models
{
    public class SearchParam
    {
        public string SearchText { get; set; }     
        public bool IsActive { get; set; } = true;
        public string Status { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string SortDirection { get; set; }
        public string SortExpression { get; set; }
    }
}
