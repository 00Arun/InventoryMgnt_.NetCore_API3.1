using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.ViewModels
{
    public class IdentityViewModel
    {
        public IdentityViewModel()
        {
            IsDeleted = false;
            IsActive = true;
            AddedOn = DateTime.UtcNow;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Mobilenumber { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public int State { get; set; }
        public int PostCode { get; set; }     
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime AddedOn { get; set; }
        public string AddedBy { get; set; }
        public string UserType { get; set; }
    }
}
