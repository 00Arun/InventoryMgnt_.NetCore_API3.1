using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryBooks.Application.Intefaces;
using InventoryBooks.Infrastructure.Entities;
using InventoryBooks.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryBooks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReminderController : ControllerBase
    {
        private readonly IReminderServices _reminderServices;
        public ReminderController(IReminderServices reminderServices)
        {
            this._reminderServices = reminderServices;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            ResponseModel<List<InventoryHistory>> response = new ResponseModel<List<InventoryHistory>>();
            try
            {
                string authToken = HttpContext.Request.Headers["Authorization"];
                if (authToken == "reminder_r!#+pkk4C21kV]r")
                {
                    response = await _reminderServices.SendReminderEmail();
                    if (!response.Status)
                    {
                        return BadRequest(response);
                    }
                    return Ok(response);
                }
                else {
                    response.Token = "";
                    response.Entity = new List<InventoryHistory>();
                    response.ReturnMessage.Add("Error: Unauthorized");
                    response.Status = true;
                    return Ok(response);
                }
              
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ReturnMessage.Add(ex.Message);
                return BadRequest(response);
            }
        }
    }
}