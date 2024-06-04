using GoodsExchangeAtFUManagement.Repository.Repositories.OTPCodeRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Repository.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangeAsync();
        IUserRepository UserRepository { get; }
        IOTPCodeRepository OTPCodeRepository { get; }
    }
}
