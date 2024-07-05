using BusinessObjects.DTOs.CampusDTOs;
using BusinessObjects.DTOs.UserDTOs;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.DTOs.UserDTOs;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.UserServices
{
    public interface IUserService
    {
        Task Register(UserRegisterRequestTestingModel request);
        Task RegisterAccount(UserRegisterRequestModel request);
        Task<UserLoginResponseModel> Login(UserLoginRequestModel request);
        Task ResetPassword(UserResetPasswordRequestModel request);
        Task ChangePassword(UserChangePasswordRequestModel model, string token);
        Task<List<ViewUserResponseModel>> GetAllUser(string searchQuery, int pageIndex, int pageSize);
        Task<ViewUserResponseModel> GetUserById(string id);
        Task DeleteUser(string id);
        Task UpdateUser(UpdateUserRequestModel request);
    }
}
