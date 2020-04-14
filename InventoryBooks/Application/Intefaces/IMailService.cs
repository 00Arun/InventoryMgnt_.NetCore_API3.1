using Mandrill.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Application.Intefaces
{
    public interface IMailService
    {
        Task SendMail(string senderName, string sendTo, string Subject, string Body);
        Task SendMail(MandrillMessage message, string messageTemplateSlug);
    }
}
