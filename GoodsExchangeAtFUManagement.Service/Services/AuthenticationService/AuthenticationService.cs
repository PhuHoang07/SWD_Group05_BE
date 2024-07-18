using AutoMapper;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.DTOs.UserDTOs;
using GoodsExchangeAtFUManagement.Repository.Enums;
using GoodsExchangeAtFUManagement.Repository.Repositories.RefreshTokenRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories;
using GoodsExchangeAtFUManagement.Service.Services.EmailServices;
using GoodsExchangeAtFUManagement.Service.Ultis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;


        public AuthenticationService(IUserRepository userRepository, IMapper mapper, IEmailService emailService, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task Register(UserRegisterRequestModel request)
        {
            User currentUser = await _userRepository.GetSingle(u => u.Email.Equals(request.Email));

            if (currentUser != null)
            {
                throw new CustomException("Email is already used to create account!");
            }

            User newUser = _mapper.Map<User>(request);
            newUser.Id = Guid.NewGuid().ToString();
            newUser.Role = nameof(RoleEnums.User);
            newUser.Status = AccountStatusEnums.Active.ToString();
            newUser.Balance = 0;
            var firstPassword = PasswordHasher.GenerateRandomPassword();
            var htmlBody = HTMLEmail.CreateAccountEmail(request.Fullname, request.Email, firstPassword);
            bool sendEmailSuccess = await _emailService.SendEmail(request.Email, "Login Information", htmlBody);

            if (!sendEmailSuccess)
            {
                throw new CustomException("Error in sending email");
            }

            var (salt, hash) = PasswordHasher.HashPassword(firstPassword);
            newUser.Password = hash;
            newUser.Salt = salt;

            await _userRepository.Insert(newUser);
        }

        public async Task<UserLoginResponseModel> Authenticate(UserLoginRequestModel request)
        {
            var user = await _userRepository.GetSingle(u => u.Email.Equals(request.Email));

            if (user == null)
            {
                throw new CustomException("There is no account using this email!");
            }

            if (user.Status.Equals(AccountStatusEnums.Inactive.ToString()))
            {
                throw new CustomException("This account is banned from system");
            }

            var oldRefreshToken = await _refreshTokenRepository.GetSingle(r => r.UserId == user.Id && r.ExpiredDate > DateTime.Now);
            string token;
            if (oldRefreshToken == null)
            {
                token = JwtGenerator.GenerateRefreshToken();
                var refreshToken = new RefreshToken
                {
                    Id = Guid.NewGuid().ToString(),
                    Token = token,
                    ExpiredDate = DateTime.Now.AddDays(7),
                    UserId = user.Id,
                };
                await _refreshTokenRepository.Insert(refreshToken);
            }
            else
            {
                token = oldRefreshToken.Token;
            }


            if (!PasswordHasher.VerifyPassword(request.Password, user.Salt, user.Password))
            {
                throw new CustomException("Password incorrect!");
            }

            return new UserLoginResponseModel()
            {
                UserInfo = new UserInfo
                {
                    Id = user.Id,
                    Fullname = user.Fullname,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Balance = user.Balance,
                    Role = user.Role
                },
                token = JwtGenerator.GenerateJWT(user),
                refreshToken = token
            };
        }
    }
}
