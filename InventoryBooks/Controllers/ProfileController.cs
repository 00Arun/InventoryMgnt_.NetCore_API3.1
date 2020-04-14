using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryBooks.ActionFilters;
using InventoryBooks.Application.Intefaces;
using InventoryBooks.Infrastructure.Entities;
using InventoryBooks.Models.Common;
using InventoryBooks.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryBooks.Controllers
{
    [ServiceFilter(typeof(SecurityFilter))]
    [Authorize(Roles = "Admin,Student,librarian")]
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileServices _profileServices;
        public ProfileController(IProfileServices profileServices)
        {
            this._profileServices = profileServices;
        }

        [HttpPost]
        [Route("ChangePassword")]
        public IActionResult Put([FromBody]ChangePasswordViewModel model)
        {
            ResponseModel<UserViewModel> response = new ResponseModel<UserViewModel>();
            try
            {
                response = _profileServices.ChangePassword(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ReturnMessage.Add(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string userId)
        {
            SecurityModel securityModel = (SecurityModel)(HttpContext.Items["SecurityModel"]);
            ResponseModel<UserViewModel> response = new ResponseModel<UserViewModel>();
            try
            {
                response = await _profileServices.GetByIdAsync(userId, securityModel);
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

        //update profile
        [HttpPut("{UpdateProfile}/{id}")]
        public async Task<IActionResult> Put(string Id, [FromBody] UserSignUpModel entity)
        {
            SecurityModel securityModel = (SecurityModel)(HttpContext.Items["SecurityModel"]);
            ResponseModel<UserViewModel> response = new ResponseModel<UserViewModel>();
            try
            {
                entity.Id = Id;
                response = await _profileServices.UpdateAsync(entity, securityModel);
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