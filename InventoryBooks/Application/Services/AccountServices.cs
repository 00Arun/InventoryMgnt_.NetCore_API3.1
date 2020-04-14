using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AutoMapper;
using InventoryBooks.Application.Intefaces;
using InventoryBooks.Helper;
using InventoryBooks.Infrastructure;
using InventoryBooks.Infrastructure.Entities;
using InventoryBooks.Models.Common;
using InventoryBooks.ViewModels;
using Mandrill.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace InventoryBooks.Application.Services
{
    public class AccountServices : IAccountServices
    {
        private readonly IDynamoDbContext<UserDetails> _context;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        public AccountServices(IDynamoDbContext<UserDetails> context, IMapper mapper, IMailService mailService)
        {
            this._context = context;
            this._mapper = mapper;
            this._mailService = mailService;
        }
        public async Task<ResponseModel<IdentityViewModel>> GenerateAdminUsers()
        {
            ResponseModel<IdentityViewModel> response = new ResponseModel<IdentityViewModel>();
            try
            {
                var adminUsersList = GetUserList();
                foreach (var model in adminUsersList)
                {
                    ScanFilter filter = new ScanFilter();
                    filter.AddCondition("Email", ScanOperator.Equal, model.Email);
                    filter.AddCondition("IsDeleted", ScanOperator.Equal, false);
                    IEnumerable<UserDetails> UserList = await _context.GetAsync(filter);
                    if (!UserList.Any())
                    {
                        await this._context.SaveAsync(model);
                        response.ReturnMessage.Add("User has been Added successfully.");
                    }
                }
                response.Status = true;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
        public async Task<ResponseModel<LoginUserViewModel>> LoginAsync(Credentials model)
        {
            ResponseModel<LoginUserViewModel> response = new ResponseModel<LoginUserViewModel>();
            try
            {
                ScanFilter filter = new ScanFilter();
                filter.AddCondition("Email", ScanOperator.Equal, model.UserName);
                filter.AddCondition("IsDeleted", ScanOperator.Equal, false);
                filter.AddCondition("IsActive", ScanOperator.Equal, true);
                var user = _context.GetAsync(filter).Result.FirstOrDefault();

                if (user == null)
                {
                    response.Status = false;
                    response.ReturnMessage.Add("User does not exist.");
                    return response;
                }

                if (user.IsActive == false)
                {
                    response.Status = false;
                    response.ReturnMessage.Add("User is deactivated by the admin.");
                    return response;
                }

                string hashedPassword = Hasher.GenerateHash(model.Password + user.Salt);
                if (user.Password != hashedPassword)
                {
                    response.Status = false;
                    response.ReturnMessage.Add("Username and password do not match.");
                    return response;
                }

                if (!user.IsActive || user.IsDeleted)
                {
                    response.Status = false;
                    response.ReturnMessage.Add("Your account has been disabled.");
                }

                var loginUser = _mapper.Map<LoginUserViewModel>(user);

                response.Entity = loginUser;
                response.Status = true;
                if (response.Status)
                {
                    user.LastLogin = DateTime.UtcNow;
                    await this._context.SaveAsync(user);
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
        public async Task<ResponseModel<IdentityViewModel>> Register(UserSignUpModel model)
        {
            ResponseModel<IdentityViewModel> response = new ResponseModel<IdentityViewModel>();
            try
            {
                ScanFilter filter = new ScanFilter();
                filter.AddCondition("Email", ScanOperator.Equal, model.Email);
                filter.AddCondition("IsDeleted", ScanOperator.Equal, false);
                IEnumerable<UserDetails> UserList = await _context.GetAsync(filter);
                if (!UserList.Any())
                {
                    model.Password = CommonHelper.CreatePassword(8);
                    UserDetails userModel = _mapper.Map<UserDetails>(model);
                    userModel.AddedOn = DateTime.UtcNow;
                    userModel.Salt = Hasher.GetSalt();
                    userModel.Password = Hasher.GenerateHash(userModel.Password + userModel.Salt);
                    userModel.UserName = model.Email;
                    userModel.Email = model.Email;
                    userModel.Id = Guid.NewGuid().ToString();
                    //Need to pull dynamicly from db
                    Roles userRole = new Roles();
                    userRole.Description = "";
                    userRole.Id = Guid.NewGuid().ToString();
                    userRole.RomeName = StaticRoles.Student;
                    userModel.Usertype = userRole.RomeName;
                    userModel.Roles = userRole;
                    userModel.IsActive = true;
                    userModel.IsDeleted = false;
                    userModel.IsStatic = false;
                    await this._context.SaveAsync(userModel);
                    response.Entity = _mapper.Map<IdentityViewModel>(userModel);
                    response.ReturnMessage.Add("User has been Added successfully.");
                    response.Status = true;

                    // Sending User Registration Edm
                   // MailHelper _mailServices = new MailHelper();

                    //tem fixes
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UserName :" + userModel.UserName);
                    sb.Append("password :" + model.Password);
                    //_mailServices.SendEMail("", userModel.Email, "Welcome", sb.ToString(), "", "");
                    await _mailService.SendMail("", userModel.Email, "Welcome to inventory management system", sb.ToString());
                }
                else
                {
                    response.Entity = new IdentityViewModel();
                    response.ReturnMessage.Add("User already exist.");
                    response.Status = true;
                }

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
        protected List<UserDetails> GetUserList()
        {
            List<UserDetails> userList = new List<UserDetails>();
            #region Admin User
            userList.Add(new UserDetails
            {
                AddedOn = DateTime.UtcNow,
                Salt = "8e21ef5dab7da010aa663ee6242b1e04",
                Password = "1aa169928841b8aac6fe340d685daa4f3d343ea329924c0291fb32d266241ffd",
                UserName = "admin@yopmail.com",
                Email = "admin@yopmail.com",
                Id = Guid.NewGuid().ToString(),
                Roles = new Roles
                {
                    Description = "Admin role",
                    Id = Guid.NewGuid().ToString(),
                    RomeName = StaticRoles.Admin
                },
                Usertype = StaticRoles.Admin,
                IsStatic = true,
                IsDeleted = false,
                IsActive = true,
                City = "Nepal",
                FirstName = "Arun",
                LastName = "Pandey",
                MobileNumber = "9854343234",
                PostCode = 3212,
                State = 1,
                Suburb = "Kathmandu"

            });
            #endregion
            #region Librarian User
            userList.Add(new UserDetails
            {
                AddedOn = DateTime.UtcNow,
                Salt = "228368c4fd4c400b2e75275a3e6b817a",
                Password = "ea7cde8fbb5136264d9bb3aba94fc5a9335376e457c7cacf5747a270e0cdc836",
                UserName = "librarian@yopmail.com",
                Email = "librarian@yopmail.com",
                Id = Guid.NewGuid().ToString(),
                Roles = new Roles
                {
                    Description = "librarian role",
                    Id = Guid.NewGuid().ToString(),
                    RomeName = StaticRoles.librarian
                },
                Usertype = StaticRoles.librarian,
                IsStatic = true,
                IsDeleted = false,
                IsActive = true,
                City = "Nepal",
                FirstName = "Samikshya",
                LastName = "Pandney",
                MobileNumber = "9854343234",
                PostCode = 3212,
                State = 1,
                Suburb = "Kathmandu"
            });
            #endregion
           
            return userList;
        }
    }
}
