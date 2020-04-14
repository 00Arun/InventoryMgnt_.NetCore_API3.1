using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryBooks.ActionFilters;
using InventoryBooks.Application.Intefaces;
using InventoryBooks.Helper;
using InventoryBooks.Infrastructure.Entities;
using InventoryBooks.Models.Common;
using InventoryBooks.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryBooks.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountServices;
        public AccountController(IAccountServices accountServices)
        {
            this._accountServices = accountServices;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Credentials model)
        {
            ResponseModel<LoginUserViewModel> response = new ResponseModel<LoginUserViewModel>();
            try
            {
                response = await _accountServices.LoginAsync(model);
                if (response.Status)
                {
                    var user = response.Entity;
                    string tokenString = TokenManager.CreateToken(user.Id, user.FirstName,user.Email, user.Roles.RomeName);
                  
                    response.Entity.Token = tokenString;
                    response.Entity.IsAuthenticated = true;
                    response.IsAuthenticated = true;                   
                    response.Token = tokenString;
                    return Ok(response);
                }
                else
                {
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

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserSignUpModel entity)
        {           
            ResponseModel<IdentityViewModel> response = new ResponseModel<IdentityViewModel>();
            try
            {
                response = await _accountServices.Register(entity);             
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

        [HttpPost("GenerateAdminUsers")]
        public async Task<IActionResult> GenerateAdminUsers()
        {           
            ResponseModel<IdentityViewModel> response = new ResponseModel<IdentityViewModel>();
            try
            {
                response = await _accountServices.GenerateAdminUsers();             
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