using Amazon.DynamoDBv2.DataModel;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Infrastructure.Entities
{
    public class InventoryHistory
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        [DynamoDBProperty]
        public string BookId { get; set; }
        [DynamoDBProperty]
        public string BookName { get; set; }
        [DynamoDBProperty]
        public string BookAuthor { get; set; }
        [DynamoDBProperty]
        public string BookPublication { get; set; }
        [DynamoDBProperty]
        public string BookType { get; set; }
        [DynamoDBProperty]
        public decimal BookPrice { get; set; }
        [DynamoDBProperty]
        public string BookDescription { get; set; }
        [DynamoDBProperty]
        public librarian librarian { get; set; }
        [DynamoDBProperty]
        public string UserId { get; set; }
        [DynamoDBProperty]
        public string UserName { get; set; }

        [DynamoDBProperty]
        public string Email { get; set; }
        [DynamoDBProperty]
        public string FirstName { get; set; }
        [DynamoDBProperty]
        public string LastName { get; set; }
        [DynamoDBProperty]
        public string MobileNumber { get; set; }

        [DynamoDBProperty]
        public string Suburb { get; set; }
        [DynamoDBProperty]
        public string City { get; set; }
        [DynamoDBProperty]
        public int State { get; set; }
        [DynamoDBProperty]
        public int PostCode { get; set; }
        [DynamoDBProperty]
        public string ReturnStatus { get; set; } //Borrow, Returned
        [DynamoDBProperty]
        public DateTime? BookReturnDate { get; set; }

        [DynamoDBProperty]
        public string LateBy
        {

            get
            {
                string result = string.Empty;
                if (librarian.RenewOn < DateTime.UtcNow)
                {
                    var dateDiff = (DateTime.UtcNow - librarian.RenewOn);
                    if (dateDiff.Value.Days > 0)
                    {
                        result = dateDiff.Value.Days.ToString();
                    };
                }
                return result;
            }
            set
            {
                LateBy = value;
            }
        }

        [DynamoDBProperty]
        public DateTime? RenewOn { get; set; }
        [DynamoDBProperty]
        public bool IsReminder { get; set; } = false;

    }
}

