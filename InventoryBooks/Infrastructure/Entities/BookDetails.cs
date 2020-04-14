using Amazon.DynamoDBv2.DataModel;
using InventoryBooks.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Infrastructure.Entities
{
    public class BookDetails : UserAction
    {
        public BookDetails()
        {
            UserDetails = new List<UserViewModel>();
            librarian = new librarian();
            Currency = "Rs";
        }
        [DynamoDBHashKey]
        public string Id { get; set; }

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
        public string BookStatus { get; set; } //publish, Draft
        [DynamoDBProperty]
        public long Stock { get; set; }
        [DynamoDBProperty]
        public List<UserViewModel> UserDetails { get; set; }
        [DynamoDBProperty]
        public long RemaningStock { get; set; }
        [DynamoDBProperty]
        public string Currency { get; set; }

    }
}
