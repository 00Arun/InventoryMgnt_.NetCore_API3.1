using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Infrastructure.Entities
{
    public class UserServicesItem
    {
        public UserServicesItem()
        {
            RequestStatus = "Pending";
            IsReserved = true;
        }
        [DynamoDBHashKey]
        public string Id { get; set; }
        [DynamoDBProperty]
        public string UserId { get; set; }
        [DynamoDBProperty]
        public string BookId { get; set; }
        [DynamoDBProperty]
        public string RequestStatus { get; set; }
        [DynamoDBProperty]
        public string BookName { get; set; }
        [DynamoDBProperty]
        public string BookAuthor { get; set; }
        [DynamoDBProperty]
        public string BookType { get; set; }
        [DynamoDBProperty]
        public decimal BookPrice { get; set; }
        [DynamoDBProperty]
        public long Stock { get; set; }
        [DynamoDBProperty]
        public string UserName { get; set; }
        [DynamoDBProperty]
        public string Email { get; set; }
        [DynamoDBProperty]
        public string FirstName { get; set; }
        [DynamoDBProperty]
        public string LastName { get; set; }
        [DynamoDBProperty]
        public DateTime? SubmittedOn { get; set; }
        [DynamoDBProperty]
        public bool IsReserved { get; set; }
    }
}
