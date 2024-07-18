using GoodsExchangeAtFUManagement.Repository.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        Task Register(UserRegisterRequestModel request);
        Task<UserLoginResponseModel> Authenticate(UserLoginRequestModel request);
    }
}
