using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks
{
    public class AppSettings
    {
        public string SecretKey { get; set; }
    }
    public class DynamoDbOptions
    {
        public string BookDetails { get; set; }
        public string Roles { get; set; }
        public string UserDetails { get; set; }
        public string IssueBookInventory { get; set; }
        public string InventoryHistory { get; set; }
    }
}
