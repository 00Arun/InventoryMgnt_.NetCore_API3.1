using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryBooks.ActionFilters;
using InventoryBooks.Application.Intefaces;
using InventoryBooks.Infrastructure.Entities;
using InventoryBooks.Models;
using InventoryBooks.Models.Common;
using InventoryBooks.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryBooks.Controllers
{
    [ServiceFilter(typeof(SecurityFilter))]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminServices _adminServices;

        public AdminController(IAdminServices adminServices)
        {
            this._adminServices = adminServices;
        }
        [HttpGet]
        [Route("GetAllBookRequest")]
        public async Task<IActionResult> GetAllBookRequest([FromQuery]SearchParam fipso)
        {
            SecurityModel securityModel = (SecurityModel)(HttpContext.Items["SecurityModel"]);
            ResponseModel<List<UserServicesItem>> response = new ResponseModel<List<UserServicesItem>>();
            try
            {
                response = await _adminServices.GetAsync(fipso, securityModel);
                response.Token = securityModel.Token;
                if (!response.Status)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ReturnMessage.Add(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpPost]
        [Route("ApproveDisApprovedUserRequest")]
        public async Task<IActionResult> ApproveDisApprovedUserRequest(RequestApproveModel model)
        {
            SecurityModel securityModel = (SecurityModel)(HttpContext.Items["SecurityModel"]);
            ResponseModel<string> response = new ResponseModel<string>();
            try
            {
                response = await _adminServices.ApproveUserRequest(model, securityModel);
                response.Token = securityModel.Token;
                if (!response.Status)
                {
                    return BadRequest(response);
                }
                return Ok(response);
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