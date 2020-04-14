using Amazon.DynamoDBv2.DocumentModel;
using InventoryBooks.Application.Intefaces;
using InventoryBooks.Helper;
using InventoryBooks.Infrastructure;
using InventoryBooks.Infrastructure.Entities;
using InventoryBooks.Models;
using InventoryBooks.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Amazon.Runtime;
using InventoryBooks.ViewModels;

namespace InventoryBooks.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly IDynamoDbContext<UserServicesItem> _dynamodbContext;
        private readonly IDynamoDbContext<BookDetails> _dynamoDBBookContext;
        private readonly IDynamoDbContext<UserDetails> _dynamoDbUserContex;

        public UserServices(IDynamoDbContext<UserServicesItem> dynamodbContext, IDynamoDbContext<UserDetails> dynamoDbUserContex, IDynamoDbContext<BookDetails> dynamoDBBookContext)
        {
            this._dynamodbContext = dynamodbContext;
            this._dynamoDBBookContext = dynamoDBBookContext;
            this._dynamoDbUserContex = dynamoDbUserContex;
        }
        public async Task<ResponseModel<UserServicesItem>> CreateAsync(BookRequestParam item, SecurityModel security)
        {
            ResponseModel<UserServicesItem> response = new ResponseModel<UserServicesItem>();
            try
            {
                UserServicesItem bookRequestInfo = new UserServicesItem();
                ScanFilter filter = new ScanFilter();
                filter.AddCondition("UserId", ScanOperator.Equal, item.UserId);
                filter.AddCondition("BookId", ScanOperator.Equal, item.BookId);
                filter.AddCondition("IsReserved", ScanOperator.Equal, true);
                var userContextItem = _dynamodbContext.GetAsync(filter).Result;
                if (!userContextItem.Any())
                {
                    var user = await _dynamoDbUserContex.GetByIdAsync(item.UserId);
                    var book = await _dynamoDBBookContext.GetByIdAsync(item.BookId);
                    if (string.IsNullOrEmpty(bookRequestInfo.Id))
                    {
                        bookRequestInfo.Id = Guid.NewGuid().ToString();
                    }
                    bookRequestInfo.BookId = item.BookId;
                    bookRequestInfo.UserId = item.UserId;
                    bookRequestInfo.RequestStatus = "Requested";
                    bookRequestInfo.BookName = book.BookName;
                    bookRequestInfo.BookAuthor = book.BookAuthor;
                    bookRequestInfo.BookType = book.BookType;
                    bookRequestInfo.BookPrice = book.BookPrice;
                    bookRequestInfo.Stock = book.Stock;
                    bookRequestInfo.UserName = user.UserName;
                    bookRequestInfo.Email = user.Email;
                    bookRequestInfo.FirstName = user.FirstName;
                    bookRequestInfo.LastName = user.LastName;
                    bookRequestInfo.SubmittedOn = DateTime.UtcNow;
                    bookRequestInfo.IsReserved = true;
                    await _dynamodbContext.SaveAsync(bookRequestInfo);
                    response.Status = true;
                    response.ReturnMessage.Add("Your request has been submitted to admin, Please wait for approbal.");
                }
                else
                {
                    response.Status = true;
                    response.ReturnMessage.Add("Oops!! You have already requested this book.");
                }

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }

        public async Task<ResponseModel<List<BookDetails>>> GetAsync(SearchParam fipso, SecurityModel securityModel)
        {
            ResponseModel<List<BookDetails>> response = new ResponseModel<List<BookDetails>>();
            try
            {
                PagingInfo paging = new PagingInfo();
                paging.CurrentPage = fipso.CurrentPage;
                paging.PageSize = fipso.PageSize;
                paging.SortDirection = fipso.SortDirection;
                paging.SortExpression = fipso.SortExpression;

                ScanFilter filter = new ScanFilter();
                filter.AddCondition("IsActive", ScanOperator.Equal, true);
                filter.AddCondition("IsDeleted", ScanOperator.Equal, false);
                filter.AddCondition("BookStatus", ScanOperator.Equal, "Publish");
                filter.AddCondition("RemaningStock", ScanOperator.GreaterThan, 0);

                if (string.IsNullOrEmpty(fipso.SortExpression))
                {
                    fipso.SortExpression = "AddedOn";
                }
                if (paging.SortDirection != string.Empty)
                    fipso.SortExpression = fipso.SortExpression + " " + paging.SortDirection;
                int rows = 0;
                var books = await _dynamoDBBookContext.GetAsync(filter);

                var filterData = new List<BookDetails>();
                if (!string.IsNullOrEmpty(fipso.SearchText))
                {
                    filterData = books.Where(x => x.BookName.ToLower().Contains(fipso.SearchText.ToLower())
                        || x.BookAuthor.ToLower().Contains(fipso.SearchText)
                        || x.BookPublication.ToLower().Contains(fipso.SearchText)
                        ).ToList();
                }
                else
                {
                    filterData = books.ToList();
                }
                var resultList = from p in filterData.AsQueryable() select p;
                rows = resultList.Count();

                var bookList = resultList.OrderBy(fipso.SortExpression).Skip((paging.CurrentPage - 1) * paging.PageSize).Take(paging.PageSize).ToList();
                paging.TotalRows = rows;
                paging.TotalPages = Functions.CalculateTotalPages(rows, paging.PageSize);

                response.Entity = bookList;
                response.TotalRows = paging.TotalRows;
                response.TotalPages = paging.TotalPages;
                response.Status = true;
            }
            catch (AmazonServiceException amazonEx)
            {
                response.Status = false;
                response.ReturnMessage.Add($"Amazon error in table operation! Error: {amazonEx.Message}");
            }
            catch (System.Exception ex)
            {
                response.Status = false;
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
    }
}
