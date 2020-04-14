using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Models
{
    public class PagingInfo
    {
        public PagingInfo()
        {
            CurrentPage = 1;
            PageSize = 10;
            SortDirection = "DESC";
            SortExpression = string.Empty;
            TotalPages = 0;
            TotalRows = 0;
        }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string SortExpression { get; set; }
        public string SortDirection { get; set; }
        public int TotalRows { get; set; }
        public int TotalPages { get; set; }
    }
}
