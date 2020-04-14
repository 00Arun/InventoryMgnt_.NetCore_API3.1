using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Infrastructure.Entities
{
    public class UserAction
    {
        public UserAction()
        {
            IsActive = true;
            IsDeleted = false;
            AddedOn = DateTime.UtcNow;
        }
        [DynamoDBProperty]
        public DateTime? AddedOn { get; set; }
        [DynamoDBProperty]
        public DateTime? UpdatedOn { get; set; }
        [DynamoDBProperty]
        public DateTime? DeletedOn { get; set; }
        [DynamoDBProperty]
        public bool IsActive { get; set; }
        [DynamoDBProperty]
        public bool IsDeleted { get; set; }
        [DynamoDBProperty]
        public String AddedBy { get; set; }
        [DynamoDBProperty]
        public String DeletedBy { get; set; }
        [DynamoDBProperty]
        public string UpdatedBy { get; set; }
    }
}
