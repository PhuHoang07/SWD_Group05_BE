using GoodsExchangeAtFUManagement.Repository.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.UserServices
{
    public interface IUserService
    {
        Task RegisterAccount(UserRegisterRequestModel request);
        Task<UserLoginResponseModel> Login(UserLoginRequestModel request);
    }
}
