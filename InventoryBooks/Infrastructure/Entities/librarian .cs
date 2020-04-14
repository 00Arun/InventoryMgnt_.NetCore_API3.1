using Amazon.DynamoDBv2.DataModel;
using System;

namespace InventoryBooks.Infrastructure.Entities
{
    public class librarian
    {
        public librarian()
        {

        }
        [DynamoDBProperty]
        public string UserId { get; set; }
        [DynamoDBProperty]
        public string BookId { get; set; }
        [DynamoDBProperty]
        public DateTime? IssieOn { get; set; }
        [DynamoDBProperty]
        public DateTime? RenewOn { get; set; }
        [DynamoDBProperty]
        public string IssueDescription { get; set; }
        [DynamoDBProperty]
        public string Status { get; set; } //Panding,Requested,Approved,Rejected
        [DynamoDBProperty]
        public string FirstName { get; set; }
        [DynamoDBProperty]
        public string Email { get; set; }



    }
}
