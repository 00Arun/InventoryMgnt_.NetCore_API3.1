using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.ViewModels
{
    public class UserSignUpModel
    {
        public UserSignUpModel()
        {

        }
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public int State { get; set; }
        public int PostCode { get; set; }
        public string Country { get; set; }
        public string Password { get; set; }
    }
}
