using AutoMapper;
using InventoryBooks.Application.Intefaces;
using InventoryBooks.Helper;
using InventoryBooks.Infrastructure;
using InventoryBooks.Infrastructure.Entities;
using InventoryBooks.Models.Common;
using InventoryBooks.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Application.Services
{
    public class ProfileServices : IProfileServices
    {
        private readonly IDynamoDbContext<UserDetails> _dynamodbUserContext;
        private readonly IMapper _mapper;
        public ProfileServices(IDynamoDbContext<UserDetails> dynamodbUserContext, IMapper mapper)
        {
            this._dynamodbUserContext = dynamodbUserContext;
            this._mapper = mapper;
        }
        public ResponseModel<UserViewModel> ChangePassword(ChangePasswordViewModel model)
        {
            ResponseModel<UserViewModel> response = new ResponseModel<UserViewModel>();
            try
            {
                var user = _dynamodbUserContext.GetByIdAsync(model.UserId).Result;
                var oldPassword = Hasher.GenerateHash(model.OldPassword + user.Salt);
                if (user.Password != oldPassword)
                {
                    response.ReturnMessage.Add("Password is incorrect");
                    response.Status = false;
                    return response;
                }
                // New Password
                var salt = Hasher.GetSalt();
                string hashedPassword = Hasher.GenerateHash(model.NewPassword + salt);
                user.Salt = salt;
                user.Password = hashedPassword;
                _dynamodbUserContext.SaveAsync(user);
                response.Entity = _mapper.Map<UserViewModel>(user);
                response.Status = true;
                response.ReturnMessage.Add("Password changed successfully.");
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
        public async Task<ResponseModel<UserViewModel>> GetByIdAsync(string userId, SecurityModel securityModel)
        {
            ResponseModel<UserViewModel> response = new ResponseModel<UserViewModel>();
            try
            {
                var user = await _dynamodbUserContext.GetByIdAsync(userId);
                response.Entity = _mapper.Map<UserViewModel>(user);
                response.Status = true;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }

        public async Task<ResponseModel<UserViewModel>> UpdateAsync(UserSignUpModel entity, SecurityModel securityModel)
        {
            ResponseModel<UserViewModel> response = new ResponseModel<UserViewModel>();
            try
            {
                var user = await _dynamodbUserContext.GetByIdAsync(entity.Id);
                user.FirstName = entity.FirstName;
                user.LastName = entity.LastName;
                user.MobileNumber = entity.MobileNumber;
                user.Suburb = entity.Suburb;
                user.State = entity.State;
                user.PostCode = entity.PostCode;
                user.Country = entity.Country;
                user.UpdatedBy = securityModel.EmailAddress;
                user.UpdatedOn = DateTime.UtcNow;
                await _dynamodbUserContext.SaveAsync(user);
                response.Entity = _mapper.Map<UserViewModel>(user);
                response.Status = true;
                response.ReturnMessage.Add("User has been updated sucessfully.");
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ReturnMessage.Add(ex.Message);
                throw new Exception($"Amazon error in  table operation! Error: {ex}");
            }
            return response;
        }
    }
}
