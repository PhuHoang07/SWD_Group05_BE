using AutoMapper;
using GoodsExchangeAtFUManagement.Repository.DTOs.UserDTOs;
using GoodsExchangeAtFUManagement.Repository.Enums;
using GoodsExchangeAtFUManagement.Repository.Models;
using GoodsExchangeAtFUManagement.Repository.UnitOfWork;
using GoodsExchangeAtFUManagement.Service.Services.EmailServices;
using GoodsExchangeAtFUManagement.Service.Ultis;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task Register(UserRegisterRequestTestingModel request)
        {
            User currentUser = await _unitOfWork.UserRepository.GetSingle(u => u.Email.Equals(request.Email));
            if (currentUser != null)
            {
                throw new CustomException("User email existed!");
            }

            if (!request.Email.EndsWith("@fpt.edu.vn") && !request.Email.EndsWith("@fe.edu.vn"))
            {
                throw new CustomException("Email is not in correct format. Please input @fpt email!");
            }

            if (request.Role.IsNullOrEmpty())
            {
                throw new CustomException("Please enter role");
            }

            if (!Enum.IsDefined(typeof(RoleEnums), request.Role))
            {
                throw new CustomException("Please enter role in correct format");
            }

            if (!Regex.Match(request.PhoneNumber, @"^\d{10,11}$").Success)
            {
                throw new CustomException("Phone number is not in correct format!");
            }

            User newUser = _mapper.Map<User>(request);
            newUser.Id = Guid.NewGuid().ToString();
            var (salt, hash) = PasswordHasher.HashPassword(request.Password);
            newUser.Password = hash;
            newUser.Salt = salt;
            newUser.Status = AccountStatusEnums.Active.ToString();
            newUser.Balance = 0;

            await _unitOfWork.UserRepository.Insert(newUser);
            var result = await _unitOfWork.SaveChangeAsync();

            if (result < 1)
            {
                throw new Exception("Internal Server Error");
            }
        }

        public async Task RegisterAccount(UserRegisterRequestModel request)
        {
            User currentUser = await _unitOfWork.UserRepository.GetSingle(u => u.Email.Equals(request.Email));

            if (currentUser != null)
            {
                throw new CustomException("User email existed!");
            }

            if (!request.Email.EndsWith("@fpt.edu.vn") && !request.Email.EndsWith("@fe.edu.vn"))
            {
                throw new CustomException("Email is not in correct format. Please input @fpt email!");
            }

            if (!Regex.Match(request.PhoneNumber, @"^\d{10,11}$").Success)
            {
                throw new CustomException("Phone number is not in correct format!");
            }

            if (request.Fullname.IsNullOrEmpty())
            {
                throw new CustomException("Please enter your name");
            }

            User newUser = _mapper.Map<User>(request);
            newUser.Id = Guid.NewGuid().ToString();
            newUser.Role = nameof(RoleEnums.User);
            newUser.Status = AccountStatusEnums.Active.ToString();
            newUser.Balance = 0;
            var firstPassword = PasswordHasher.GenerateRandomPassword();
            var htmlBody = $"<h1>Your login information is:</h1><br/><p>Email: {request.Email}<br/>Password: {firstPassword}</p>";
            bool sendEmailSuccess = await _emailService.SendEmail(request.Email, "Login Information", htmlBody);

            if (!sendEmailSuccess)
            {
                throw new CustomException("Error in sending email");
            }

            var (salt, hash) = PasswordHasher.HashPassword(firstPassword);
            newUser.Password = hash;
            newUser.Salt = salt;

            await _unitOfWork.UserRepository.Insert(newUser);
            var result = await _unitOfWork.SaveChangeAsync();

            if (result < 1)
            {
                throw new CustomException("Internal Server Error");
            }
        }

        public async Task<UserLoginResponseModel> Login(UserLoginRequestModel request)
        {
            var user = await _unitOfWork.UserRepository.GetSingle(u => u.Email.Equals(request.Email));

            if (user == null)
            {
                throw new CustomException("User email not exist!");
            }

            if (!PasswordHasher.VerifyPassword(request.Password, user.Salt, user.Password))
            {
                throw new CustomException("Password incorrect!");
            }

            return new UserLoginResponseModel()
            {
                UserInfo = new UserInfo
                {
                    Fullname = user.Fullname,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Balance = user.Balance,
                    Role = user.Role    
                },
                token = JwtGenerator.GenerateJWT(user)
            };
        }
    }
}
