using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Models.Common
{
    public static class StaticRoles
    {
        public static string Student = "Student";
        public static string librarian = "librarian";
        public static string Admin = "Admin";
    }
    public static class BookReturnStatus
    {
        public static string Borrow = "Borrow";
        public static string Returned = "Returned";
    }
}
