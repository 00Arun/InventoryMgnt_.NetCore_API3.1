using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Models.Common
{
    public class ResponseModel<T>
    {
        public ResponseModel()
        {
            ReturnMessage = new List<String>();
            Status = true;
            Errors = new Hashtable();
            TotalPages = 0;
            TotalPages = 0;
            PageSize = 0;
            IsAuthenticated = false;
        }
        public string Token { get; set; }
        public bool Status { get; set; }
        public List<String> ReturnMessage { get; set; }
        public Hashtable Errors { get; set; }
        public int TotalPages { get; set; }
        public int TotalRows { get; set; }
        public int PageSize { get; set; }
        public Boolean IsAuthenticated { get; set; } = false;
        public T Entity { get; set; }
    }
}
