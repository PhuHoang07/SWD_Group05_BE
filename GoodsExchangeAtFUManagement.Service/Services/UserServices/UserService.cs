using AutoMapper;
using GoodsExchangeAtFUManagement.Repository.DTOs.UserDTOs;
using GoodsExchangeAtFUManagement.Repository.Enums;
using GoodsExchangeAtFUManagement.Service.Services.EmailServices;
using GoodsExchangeAtFUManagement.Service.Ultis;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories;

namespace GoodsExchangeAtFUManagement.Service.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, IMapper mapper, IEmailService emailService)
        {
            _mapper = mapper;
            _emailService = emailService;
            _userRepository = userRepository;
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
        
        public async Task TestCreateNewUser(UserRegisterRequestTestingModel request)
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
        
        public async Task CreateNewUser(UserRegisterRequestModel request)
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

            _userRepository.Update(user);
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

            _userRepository.Update(user);
        }
    }
}
