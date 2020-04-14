using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Infrastructure.Entities
{
    public class UserDetails : UserAction
    {
        public UserDetails()
        {
            BookDetails = new List<BookDetails>();
            Roles = new Roles();
            IsStatic = false;
        }
        [DynamoDBHashKey]
        public string Id { get; set; }
        [DynamoDBProperty]
        public string UserName { get; set; }
        [DynamoDBProperty]
        public bool IsStatic { get; set; }
        [DynamoDBProperty]
        public string Email { get; set; }
        [DynamoDBProperty]
        public string FirstName { get; set; }
        [DynamoDBProperty]
        public string LastName { get; set; }
        [DynamoDBProperty]
        public string MobileNumber { get; set; }

        [DynamoDBProperty]
        public string Password { get; set; }
        [DynamoDBProperty]
        public DateTime? LastLogin { get; set; }
        [DynamoDBProperty]
        public string Salt { get; set; }
        [DynamoDBProperty]
        public string Usertype { get; set; }

        [DynamoDBProperty]
        public string Suburb { get; set; }
        [DynamoDBProperty]
        public string City { get; set; }
        [DynamoDBProperty]
        public int State { get; set; }
        [DynamoDBProperty]
        public int PostCode { get; set; }
        [DynamoDBProperty]
        public string Country { get; set; }
        [DynamoDBProperty]
        public Roles Roles { get; set; }

        [DynamoDBProperty]
        public List<BookDetails> BookDetails { get; set; }

    }
}
