using InventoryBooks.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            Roles = new Roles();
        }
        public string Id { get; set; }
        public string UserName { get; set; }
        public bool IsStatic { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public DateTime? LastLogin { get; set; }
        public string Usertype { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public int State { get; set; }
        //public string RoleName
        //{
        //    get
        //    {
        //        return Roles.RomeName;
        //    }
        //    set
        //    {
        //        RoleName = value;
        //    }
        //}
        public int PostCode { get; set; }
        public Roles Roles { get; set; }
        public string Country { get; set; }
    }
}
