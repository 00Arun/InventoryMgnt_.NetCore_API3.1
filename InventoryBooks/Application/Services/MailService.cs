using InventoryBooks.Application.Intefaces;
using Mandrill;
using Mandrill.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Application.Services
{
    public class MailService : IMailService
    {
        private readonly string APIKey = "";       
        public MailService()
        {
            
           
        }
        public async Task SendMail(string senderName, string sendTo, string Subject, string Body)
        {
            var api = new MandrillApi(APIKey);
            var message = new MandrillMessage("info@arun.com", sendTo, Subject, Body);
            await api.Messages.SendAsync(message);
        }

        public async Task SendMail(MandrillMessage message, string messageTemplateSlug)
        {
            var api = new MandrillApi(APIKey);
            var result = await api.Messages.SendTemplateAsync(message, messageTemplateSlug);
        }
    }
}
