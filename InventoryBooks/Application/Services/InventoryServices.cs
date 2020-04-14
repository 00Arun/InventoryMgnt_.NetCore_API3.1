using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using InventoryBooks.Application.Intefaces;
using InventoryBooks.Infrastructure;
using InventoryBooks.Infrastructure.Entities;
using InventoryBooks.Models;
using InventoryBooks.Models.Common;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using InventoryBooks.Helper;
using InventoryBooks.ViewModels;

namespace InventoryBooks.Application.Services
{
    public class InventoryServices : IInventoryServices
    {
        private readonly IDynamoDbContext<BookDetails> _dynamoDbBookContext;
        private readonly IDynamoDbContext<InventoryHistory> _dynamoDbHistoryContex;
        private readonly IDynamoDbContext<UserServicesItem> _dynamouserServices;
        public InventoryServices(IDynamoDbContext<UserServicesItem> dynamouserService,IDynamoDbContext<BookDetails> dynamoDbContext, IDynamoDbContext<InventoryHistory> dynamoDbHistoryContex)
        {
            this._dynamoDbBookContext = dynamoDbContext;
            this._dynamoDbHistoryContex = dynamoDbHistoryContex;
            this._dynamouserServices = dynamouserService;
        }
        public async Task<ResponseModel<BookDetails>> CreateAsync(BookDetails item, SecurityModel security)
        {
            ResponseModel<BookDetails> response = new ResponseModel<BookDetails>();
            try
            {
                if (string.IsNullOrEmpty(item.Id))
                {
                    item.Id = Guid.NewGuid().ToString();
                }
                item.librarian = new librarian
                {
                    BookId = item.Id,
                    IssieOn = DateTime.UtcNow,
                    IssueDescription = "",
                    RenewOn = DateTime.UtcNow.AddDays(6),
                    Status = "Pending",
                    FirstName = security.FirstName,
                    Email = security.EmailAddress,
                    UserId = security.UserId

                };
                item.RemaningStock = item.Stock;
                item.AddedBy = security.EmailAddress;
                item.AddedOn = DateTime.UtcNow;
                item.IsActive = true;
                item.IsDeleted = false;
                await _dynamoDbBookContext.SaveAsync(item);
                response.ReturnMessage.Add("Book added successfully.");
                response.Entity = item;
                response.Status = true;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
        public async Task<ResponseModel<BookDetails>> DeleteAsync(string id)
        {
            ResponseModel<BookDetails> response = new ResponseModel<BookDetails>();
            try
            {
                var item = await _dynamoDbBookContext.GetByIdAsync(id);
                await _dynamoDbBookContext.DeleteByIdAsync(item);
                response.Status = true;
                response.ReturnMessage.Add("Book has been deleted successfully.");
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
        public async Task<ResponseModel<List<BookDetails>>> GetAsync(SearchParam fipso, SecurityModel security)
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
                switch (fipso.Status)
                {
                    case "Publish":
                        filter.AddCondition("BookStatus", ScanOperator.Equal, "Publish");
                        break;
                    case "Draft":
                        filter.AddCondition("BookStatus", ScanOperator.Equal, "Draft");
                        break;
                    default:
                        filter.AddCondition("BookStatus", ScanOperator.Equal, "Publish");
                        break;
                }
                if (string.IsNullOrEmpty(fipso.SortExpression))
                {
                    fipso.SortExpression = "AddedOn";
                }
                if (paging.SortDirection != string.Empty)
                    fipso.SortExpression = fipso.SortExpression + " " + paging.SortDirection;
                int rows = 0;
                IEnumerable<BookDetails> books = await _dynamoDbBookContext.GetAsync(filter);

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
        public async Task<ResponseModel<BookDetails>> GetAsync(string id)
        {
            ResponseModel<BookDetails> response = new ResponseModel<BookDetails>();
            try
            {
                BookDetails data = await _dynamoDbBookContext.GetByIdAsync(id);
                if (data == null)
                {
                    response.Status = false;
                    response.ReturnMessage.Add("Book not found.");
                }
                else
                {
                    response.Entity = data;
                    response.Status = true;
                }
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

        public async Task<ResponseModel<List<InventoryHistory>>> GetHistoryLog(SearchParam fipso, SecurityModel securityModel)
        {
            ResponseModel<List<InventoryHistory>> response = new ResponseModel<List<InventoryHistory>>();
            try
            {
                PagingInfo paging = new PagingInfo();
                paging.CurrentPage = fipso.CurrentPage;
                paging.PageSize = fipso.PageSize;
                paging.SortDirection = fipso.SortDirection;
                paging.SortExpression = fipso.SortExpression;

                ScanFilter filter = new ScanFilter();

                if (!string.IsNullOrEmpty(fipso.Status))
                {
                    filter.AddCondition("ReturnStatus", ScanOperator.Equal, fipso.Status);
                }

                if (string.IsNullOrEmpty(fipso.SortExpression))
                {
                    fipso.SortExpression = "FirstName";
                }
                if (paging.SortDirection != string.Empty)
                    fipso.SortExpression = fipso.SortExpression + " " + paging.SortDirection;
                int rows = 0;
                IEnumerable<InventoryHistory> bookshistory = await _dynamoDbHistoryContex.GetAsync(filter);

                var filterData = new List<InventoryHistory>();
                if (!string.IsNullOrEmpty(fipso.SearchText))
                {
                    filterData = bookshistory.Where(x => x.BookName.ToLower().Contains(fipso.SearchText.ToLower())
                        || x.BookAuthor.ToLower().Contains(fipso.SearchText)
                        || x.BookPublication.ToLower().Contains(fipso.SearchText)
                        ).ToList();
                }
                else
                {
                    filterData = bookshistory.ToList();
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
        public async Task<ResponseModel<BookDetails>> InventoryReturnEntry(InverntoryReturnRequest entity, SecurityModel securityModel)
        {
            ResponseModel<BookDetails> response = new ResponseModel<BookDetails>();
            try
            {
                ScanFilter filter = new ScanFilter();
                var book = await _dynamoDbBookContext.GetByIdAsync(entity.BookId);
                filter.AddCondition("BookId", ScanOperator.Equal, entity.BookId);
                filter.AddCondition("UserId", ScanOperator.Equal, entity.UserId);
                var inventoryHistory = _dynamoDbHistoryContex.GetAsync(filter).Result.FirstOrDefault();
                if (inventoryHistory != null)
                {
                    inventoryHistory.ReturnStatus = BookReturnStatus.Returned;
                    inventoryHistory.BookReturnDate = DateTime.UtcNow;
                    if (book.librarian.RenewOn < DateTime.UtcNow)
                    {
                        var dateDiff = (DateTime.UtcNow - book.librarian.RenewOn);
                        inventoryHistory.LateBy = dateDiff.Value.TotalDays.ToString();
                    }
                    await _dynamoDbHistoryContex.SaveAsync(inventoryHistory);

                    book.RemaningStock = book.RemaningStock + 1;
                    await _dynamoDbBookContext.SaveAsync(book);

                    var bookUserService = _dynamouserServices.GetAsync(filter).Result.FirstOrDefault();
                    if (bookUserService != null)
                    {
                        bookUserService.IsReserved = false;
                        await _dynamouserServices.SaveAsync(bookUserService);
                    }
                    response.Entity = book;
                    response.Status = true;
                    response.ReturnMessage.Add("Book stock has been updated sucessfully.");
                }
                else
                {
                    response.Entity = book;
                    response.Status = true;
                    response.ReturnMessage.Add("Oops!! book details not found in the history.");
                }


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
        public async Task<ResponseModel<BookDetails>> SoftDeleteAsync(string id, SecurityModel security)
        {
            ResponseModel<BookDetails> response = new ResponseModel<BookDetails>();
            try
            {
                var model = await _dynamoDbBookContext.GetByIdAsync(id);
                if (model != null)
                {
                    model.IsDeleted = true;
                    model.DeletedOn = DateTime.UtcNow;
                    model.DeletedBy = security.EmailAddress;
                    await _dynamoDbBookContext.SaveAsync(model);
                    response.Status = true;
                    response.ReturnMessage.Add("Book has been deleted successfully.");
                }
                else
                {
                    response.Status = true;
                    response.ReturnMessage.Add("OOps ! unable to delete the book");
                }
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
        public async Task<ResponseModel<BookDetails>> UpdateAsync(BookDetails entity, SecurityModel security)
        {
            ResponseModel<BookDetails> response = new ResponseModel<BookDetails>();
            try
            {
                BookDetails data = await _dynamoDbBookContext.GetByIdAsync(entity.Id);
                data.BookName = entity.BookName;
                data.BookAuthor = entity.BookAuthor;
                data.BookPublication = entity.BookPublication;
                data.BookType = entity.BookType;
                data.BookPrice = entity.BookPrice;
                data.BookDescription = entity.BookDescription;
                data.UpdatedOn = DateTime.UtcNow;
                data.UpdatedBy = security.EmailAddress;
                data.Stock = entity.Stock;
                await _dynamoDbBookContext.SaveAsync(data);
                response.Status = true;
                response.ReturnMessage.Add("Book details has been updated sucessfully.");
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
    }
}
