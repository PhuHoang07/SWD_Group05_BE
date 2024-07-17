using AutoMapper;
using GoodsExchangeAtFUManagement.Repository.DTOs.UserDTOs;
using GoodsExchangeAtFUManagement.Repository.Enums;
using GoodsExchangeAtFUManagement.Service.Services.EmailServices;
using GoodsExchangeAtFUManagement.Service.Ultis;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.RefreshTokenRepositories;
using BusinessObjects.DTOs.UserDTOs;
using System.Linq.Expressions;
using GoodsExchangeAtFUManagement.Repository.Repositories.CoinPackRepositories;

namespace GoodsExchangeAtFUManagement.Service.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICoinPackRepository _coinPackRepository;

        public UserService(IUserRepository userRepository, IMapper mapper, IEmailService emailService, IRefreshTokenRepository refreshTokenRepository, ICoinPackRepository coinPackRepository)
        {
            _mapper = mapper;
            _emailService = emailService;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _coinPackRepository = coinPackRepository;
        }

        public async Task Register(UserRegisterRequestTestingModel request)
        {
            User currentUser = await _userRepository.GetSingle(u => u.Email.Equals(request.Email));
            if (currentUser != null)
            {
                throw new CustomException("User email existed!");
            }

            User newUser = _mapper.Map<User>(request);
            newUser.Id = Guid.NewGuid().ToString();
            var (salt, hash) = PasswordHasher.HashPassword(request.Password);
            newUser.Password = hash;
            newUser.Salt = salt;
            newUser.Status = AccountStatusEnums.Active.ToString();
            newUser.Balance = 0;

            await _userRepository.Insert(newUser);

        }

        public async Task RegisterAccount(UserRegisterRequestModel request)
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

        public async Task<UserLoginResponseModel> Login(UserLoginRequestModel request)
        {
            var user = await _userRepository.GetSingle(u => u.Email.Equals(request.Email));

            if (user == null)
            {
                throw new CustomException("There is no account using this email!");
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

        public async Task ResetPassword(UserResetPasswordRequestModel request)
        {
            var user = await _userRepository.GetSingle(u => u.Email.Equals(request.Email));
            if (user == null)
            {
                throw new CustomException("There is no account using this email!");
            }

            if (!request.NewPassword.Equals(request.ConfirmPassword))
            {
                throw new CustomException("Password and comfirm password must match");
            }

            var (salt, hash) = PasswordHasher.HashPassword(request.NewPassword);
            user.Password = hash;
            user.Salt = salt;

            await _userRepository.Update(user);
        }

        public async Task ChangePassword(UserChangePasswordRequestModel model, string token)
        {
            var userId = JwtGenerator.DecodeToken(token, "userId");
            var user = await _userRepository.GetSingle(u => u.Id.Equals(userId));
            if (user == null)
            {
                throw new CustomException("There is no account using this email!");
            }

            if (!PasswordHasher.VerifyPassword(model.OldPassword, user.Salt, user.Password))
            {
                throw new CustomException("Password incorrect!");
            }

            if (!model.NewPassword.Equals(model.ConfirmPassword))
            {
                throw new CustomException("Password and comfirm password must match");
            }

            if (PasswordHasher.VerifyPassword(model.NewPassword, user.Salt, user.Password))
            {
                throw new CustomException("New password must not match old password");
            }

            var (salt, hash) = PasswordHasher.HashPassword(model.NewPassword);
            user.Password = hash;
            user.Salt = salt;

            await _userRepository.Update(user);
        }

        public async Task<List<ViewUserResponseModel>> GetAllUser(string searchQuery, int pageIndex, int pageSize)
        {
            Expression<Func<User, bool>> searchFilter = u => string.IsNullOrEmpty(searchQuery) ||
                                                               u.Email.Contains(searchQuery) ||
                                                               u.PhoneNumber.Contains(searchQuery) ||
                                                               u.Role.Contains(searchQuery) ||
                                                               u.Status.Contains(searchQuery);

            var users = await _userRepository.Get(searchFilter, pageIndex: pageIndex, pageSize: pageSize);
            var userResponses = _mapper.Map<List<ViewUserResponseModel>>(users);
            return userResponses;
        }

        public async Task<ViewUserResponseModel> GetUserById(string id)
        {
            var user = await _userRepository.GetSingle(u => u.Id.Equals(id));
            if (user == null || user.Status == AccountStatusEnums.Inactive.ToString())
            {
                throw new CustomException("User not found");
            }
            var userResponses = _mapper.Map<ViewUserResponseModel>(user);
            return userResponses;
        }

        public async Task DeleteUser(string id)
        {
            User deleteUser = await _userRepository.GetSingle(c => c.Id.Equals(id));
            if (deleteUser == null)
            {
                throw new CustomException("User not found");
            }
            if (deleteUser.Status == AccountStatusEnums.Inactive.ToString())
            {
                throw new CustomException("User is already be removed");
            }
            deleteUser.Status = AccountStatusEnums.Inactive.ToString();
            await _userRepository.Update(deleteUser);
        }

        public async Task UpdateUser(UpdateUserRequestModel request, string id)
        {
            var user = await _userRepository.GetSingle(u => u.Id.Equals(id));
            if (user == null || user.Status == AccountStatusEnums.Inactive.ToString())
            {
                throw new CustomException("User not found");
            }
            if (!string.IsNullOrEmpty(request.Fullname))
            {
                user.Fullname = request.Fullname;
            }
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                user.PhoneNumber = request.PhoneNumber;
            }
            await _userRepository.Update(user);
        }

        public async Task UpdateUserForAdmin(AdminUpdateUserResponseModel request, string id)
        {
           
            var user = await _userRepository.GetSingle(u => u.Id.Equals(id));
            if (user == null || user.Status == AccountStatusEnums.Inactive.ToString())
            {
                throw new CustomException("User not found");
            }
            if (!string.IsNullOrEmpty(request.Fullname))
                {
                    user.Fullname = request.Fullname;
                }
            if (!string.IsNullOrEmpty(request.Email))
            {
                user.Email = request.Email;
            }
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                user.PhoneNumber = request.PhoneNumber;
            }
            if (!string.IsNullOrEmpty(request.Role))
            {
                user.Role = request.Role;
            }

            await _userRepository.Update(user);
            }

        public async Task AddCoinToUserBalance(string token, string coinPackId)
        {
            var userId = JwtGenerator.DecodeToken(token, "userId");
            var user = await _userRepository.GetSingle(u => u.Id.Equals(userId));
            var coinPack = await _coinPackRepository.GetSingle(c => c.Id.Equals(coinPackId));
            user.Balance += coinPack.CoinAmount;
            await _userRepository.Update(user);
        }
    }

