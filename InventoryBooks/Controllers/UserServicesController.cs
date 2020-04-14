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
    [Authorize(Roles = "Student")]
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class UserServicesController : ControllerBase
    {
        private readonly IUserServices _userServices;
        public UserServicesController(IUserServices userServices)
        {
            this._userServices = userServices;
        }
        
        [HttpGet]
        [Route("GetAllBookList")]
        public async Task<IActionResult> GetAllBookList([FromQuery]SearchParam fipso)
        {
            SecurityModel securityModel = (SecurityModel)(HttpContext.Items["SecurityModel"]);
            ResponseModel<List<BookDetails>> response = new ResponseModel<List<BookDetails>>();
            try
            {
                response = await _userServices.GetAsync(fipso, securityModel);
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
        [Route("BookRequest")]
        public async Task<IActionResult> BookRequest(BookRequestParam entity)
        {
            SecurityModel securityModel = (SecurityModel)(HttpContext.Items["SecurityModel"]);
            ResponseModel<UserServicesItem> response = new ResponseModel<UserServicesItem>();
            try
            {
                response = await _userServices.CreateAsync(entity, securityModel);
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