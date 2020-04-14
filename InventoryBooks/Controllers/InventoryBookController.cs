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
    [Authorize(Roles="librarian")]
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class InventoryBookController : ControllerBase
    {
        private readonly IInventoryServices _inventoryServices;
        public InventoryBookController(IInventoryServices inventoryServices)
        {
            this._inventoryServices = inventoryServices;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]SearchParam fipso)
        {
            SecurityModel securityModel = (SecurityModel)(HttpContext.Items["SecurityModel"]);
            ResponseModel<List<BookDetails>> response = new ResponseModel<List<BookDetails>>();
            try
            {
                response = await _inventoryServices.GetAsync(fipso, securityModel);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            SecurityModel securityModel = (SecurityModel)(HttpContext.Items["SecurityModel"]);
            ResponseModel<BookDetails> response = new ResponseModel<BookDetails>();
            try
            {
                response = await _inventoryServices.GetAsync(id);
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
        public async Task<IActionResult> Post(BookDetails entity)
        {
            SecurityModel securityModel = (SecurityModel)(HttpContext.Items["SecurityModel"]);
            ResponseModel<BookDetails> response = new ResponseModel<BookDetails>();
            try
            {
                response = await _inventoryServices.CreateAsync(entity, securityModel);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, BookDetails entity)
        {
            SecurityModel securityModel = (SecurityModel)(HttpContext.Items["SecurityModel"]);
            ResponseModel<BookDetails> response = new ResponseModel<BookDetails>();
            try
            {
                entity.Id = id;
                response = await _inventoryServices.UpdateAsync(entity, securityModel);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            SecurityModel securityModel = (SecurityModel)(HttpContext.Items["SecurityModel"]);
            ResponseModel<BookDetails> response = new ResponseModel<BookDetails>();
            try
            {
                response = await _inventoryServices.SoftDeleteAsync(id, securityModel);
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
        [Route("InventoryReturnEntry")]
        public async Task<IActionResult> InventoryReturnEntry(InverntoryReturnRequest entity)
        {
            SecurityModel securityModel = (SecurityModel)(HttpContext.Items["SecurityModel"]);
            ResponseModel<BookDetails> response = new ResponseModel<BookDetails>();
            try
            {               
                response = await _inventoryServices.InventoryReturnEntry(entity, securityModel);
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

        [HttpGet]
        [Route("GetHistoryLog")]
        public async Task<IActionResult> GetHistoryLog([FromQuery]SearchParam fipso)
        {
            SecurityModel securityModel = (SecurityModel)(HttpContext.Items["SecurityModel"]);
            ResponseModel<List<InventoryHistory>> response = new ResponseModel<List<InventoryHistory>>();
            try
            {
                response = await _inventoryServices.GetHistoryLog(fipso, securityModel);
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
