using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using InventoryBooks.Application.Intefaces;
using InventoryBooks.Infrastructure;
using InventoryBooks.Infrastructure.Entities;
using InventoryBooks.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Application.Services
{
    public class ReminderServices : IReminderServices
    {     
        private readonly IDynamoDbContext<InventoryHistory> _dynamoDbHistoryContex;
        private readonly IMailService _mailService;
        public ReminderServices(IDynamoDbContext<InventoryHistory> dynamoDbHistoryContex, IMailService mailService)
        {            
            this._dynamoDbHistoryContex = dynamoDbHistoryContex;
            this._mailService = mailService;
        }
        public async Task<ResponseModel<List<InventoryHistory>>> SendReminderEmail()
        {
            ResponseModel<List<InventoryHistory>> response = new ResponseModel<List<InventoryHistory>>();
            try
            {
                ScanFilter filter = new ScanFilter();
                filter.AddCondition("ReturnStatus", ScanOperator.Equal, BookReturnStatus.Borrow);
                filter.AddCondition("RenewOn", ScanOperator.LessThan, DateTime.Now);
                filter.AddCondition("IsReminder", ScanOperator.Equal, false);

                var inventoryHistory = _dynamoDbHistoryContex.GetAsync(filter).Result;
                if (inventoryHistory.Any())
                {
                    List<InventoryHistory> userList = new List<InventoryHistory>();
                    foreach (var item in inventoryHistory)
                    {

                        await _mailService.SendMail("", item.Email, "Reminder", "Hi '" + item.FirstName + "Please kindly return the book on time.");
                        item.IsReminder = true;
                        userList.Add(item);
                        await _dynamoDbHistoryContex.SaveAsync(item);
                    }
                    response.Entity = userList;
                    response.Status = true;
                    response.ReturnMessage.Add("Email send successfully");
                }
                else
                {
                    response.Entity = new List<InventoryHistory>();
                    response.Status = true;
                    response.ReturnMessage.Add("Inventory Data not found.");
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
    }
}
