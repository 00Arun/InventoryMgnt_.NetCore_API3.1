using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Infrastructure.Entities
{
    public class Roles
    {
        public Roles()
        {

        }
        [DynamoDBHashKey]
        public string Id { get; set; }
        [DynamoDBProperty]
        public string RomeName { get; set; }
        [DynamoDBProperty]
        public string Description { get; set; }

    }
}
