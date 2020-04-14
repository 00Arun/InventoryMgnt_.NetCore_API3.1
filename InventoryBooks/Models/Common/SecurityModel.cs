using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Models.Common
{
    public class SecurityModel
    {
        public string Token { get; set; }
        public string UserId { get; set; }       
        public string  Roles { get; set; }
        public string FirstName { get; set; }       
        public string EmailAddress { get; set; }
        public string System { get; set; }
       
    }
}
