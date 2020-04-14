using InventoryBooks.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.ViewModels
{
    public class LoginUserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public Roles Roles { get; set; }
        public List<BookDetails> BookDetails { get; set; }
        public DateTime LastLogin { get; set; }
        public string RoleName
        {
            get
            {
                return Roles.RomeName;
            }
            set
            {
                RoleName = value;
            }
        }
        public string Suburb { get; set; }
        public string City { get; set; }
        public int State { get; set; }
        public int PostCode { get; set; }
        public string Token { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Country { get; set; }
    }
}
