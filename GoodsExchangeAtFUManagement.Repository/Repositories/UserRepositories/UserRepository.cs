using AutoMapper;
using GoodsExchangeAtFUManagement.Repository.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.GenericRepositories;
using GoodsExchangeAtFUManagement.Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(GoodsExchangeAtFuContext context) : base(context)
        {
        }
    }
}
