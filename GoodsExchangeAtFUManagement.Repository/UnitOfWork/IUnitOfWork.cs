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
        public Task<int> SaveChangeAsync();
        public IUserRepository UserRepository { get; }
    }
}
