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
using AutoMapper;
using System.Text;

namespace InventoryBooks.Application.Services
{
    public class AdminServices : IAdminServices
    {
        private readonly IDynamoDbContext<UserServicesItem> _dynamodbContext;
        private readonly IDynamoDbContext<BookDetails> _dynamoDBBookContext;
        private readonly IDynamoDbContext<UserDetails> _dynamoDbUserContex;
        private readonly IDynamoDbContext<InventoryHistory> _dynamoDbHistoryContex;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;
        public AdminServices(IDynamoDbContext<UserServicesItem> dynamodbContext, IDynamoDbContext<UserDetails> dynamoDbUserContex, IDynamoDbContext<BookDetails> dynamoDBBookContext, IDynamoDbContext<InventoryHistory> dynamoDbHistoryContex, IMailService mailService, IMapper mapper)
        {
            this._dynamodbContext = dynamodbContext;
            this._dynamoDBBookContext = dynamoDBBookContext;
            this._dynamoDbUserContex = dynamoDbUserContex;
            this._dynamoDbHistoryContex = dynamoDbHistoryContex;
            this._mailService = mailService;
            this._mapper = mapper;
        }
        public async Task<ResponseModel<string>> ApproveUserRequest(RequestApproveModel entity, SecurityModel securityModel)
        {
            ResponseModel<string> response = new ResponseModel<string>();
            try
            {
                var bookRequestbyUser = await _dynamodbContext.GetByIdAsync(entity.Id);
                var user = await _dynamoDbUserContex.GetByIdAsync(entity.UserId);
                var book = await _dynamoDBBookContext.GetByIdAsync(entity.BookId);

                bookRequestbyUser.RequestStatus = entity.Status; //Approved, Rejected
                await _dynamodbContext.SaveAsync(bookRequestbyUser);

                if (entity.Status == "Approved")
                {
                    book.RemaningStock = (book.Stock - 1);
                    book.UserDetails.Add(_mapper.Map<UserViewModel>(user));
                    book.librarian.IssieOn = DateTime.UtcNow;
                    book.librarian.RenewOn = DateTime.UtcNow.AddDays(7);
                    await _dynamoDBBookContext.SaveAsync(book);
                    InventoryHistory invHistory = new InventoryHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        BookId = entity.BookId,
                        BookAuthor = book.BookAuthor,
                        BookDescription = book.BookDescription,
                        BookName = book.BookName,
                        BookPrice = book.BookPrice,
                        BookPublication = book.BookPublication,
                        BookType = book.BookType,
                        City = user.City,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        MobileNumber = user.MobileNumber,
                        PostCode = user.PostCode,
                        State = user.State,
                        Suburb = user.Suburb,
                        UserId = entity.UserId,
                        UserName = user.Email,
                        librarian = new librarian
                        {
                            BookId = entity.BookId,
                            IssieOn = book.librarian.IssieOn,
                            IssueDescription = book.librarian.IssueDescription,
                            RenewOn = book.librarian.RenewOn

                        },
                        RenewOn = book.librarian.RenewOn,
                        ReturnStatus = BookReturnStatus.Borrow
                    };
                    await _dynamoDbHistoryContex.SaveAsync(invHistory);
                }
                else
                {
                    bookRequestbyUser.IsReserved = false;
                }
                await _mailService.SendMail("", user.Email, "Request '" + entity.Status + "'", "Hi '" + user.FirstName + "' you request has been '" + entity.Status + "'");
                response.Entity = "success";
                response.Status = true;
                response.ReturnMessage.Add("Request has been " + entity.Status + " sucessfully.");
            }
            catch (AmazonServiceException amazon)
            {
                response.Status = false;
                response.ReturnMessage.Add($"Amazon error in table operation! Error: {amazon.Message}");
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
        public async Task<ResponseModel<List<UserServicesItem>>> GetAsync(SearchParam fipso, SecurityModel securityModel)
        {
            ResponseModel<List<UserServicesItem>> response = new ResponseModel<List<UserServicesItem>>();
            try
            {
                PagingInfo paging = new PagingInfo();
                paging.CurrentPage = fipso.CurrentPage;
                paging.PageSize = fipso.PageSize;
                paging.SortDirection = fipso.SortDirection;
                paging.SortExpression = fipso.SortExpression;

                ScanFilter filter = new ScanFilter();
                switch (fipso.Status)
                {
                    case "Rejected":
                        filter.AddCondition("RequestStatus", ScanOperator.Equal, "Rejected");
                        break;
                    case "Approved":
                        filter.AddCondition("RequestStatus", ScanOperator.Equal, "Approved");
                        break;
                    default:
                        filter.AddCondition("RequestStatus", ScanOperator.Equal, "Requested");
                        break;
                }

                if (string.IsNullOrEmpty(fipso.SortExpression))
                {
                    fipso.SortExpression = "SubmittedOn";
                }
                if (paging.SortDirection != string.Empty)
                    fipso.SortExpression = fipso.SortExpression + " " + paging.SortDirection;
                int rows = 0;
                IEnumerable<UserServicesItem> requestedItem = await _dynamodbContext.GetAsync(filter);

                var filterData = new List<UserServicesItem>();
                if (!string.IsNullOrEmpty(fipso.SearchText))
                {
                    filterData = requestedItem.Where(x => x.BookName.ToLower().Contains(fipso.SearchText.ToLower())
                        || x.BookAuthor.ToLower().Contains(fipso.SearchText)
                        || x.UserName.ToLower().Contains(fipso.SearchText)
                        ).ToList();
                }
                else
                {
                    filterData = requestedItem.ToList();
                }
                var resultList = from p in filterData.AsQueryable() select p;
                rows = resultList.Count();

                var requestedBookList = resultList.OrderBy(fipso.SortExpression).Skip((paging.CurrentPage - 1) * paging.PageSize).Take(paging.PageSize).ToList();
                paging.TotalRows = rows;
                paging.TotalPages = Functions.CalculateTotalPages(rows, paging.PageSize);

                response.Entity = requestedBookList;
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
