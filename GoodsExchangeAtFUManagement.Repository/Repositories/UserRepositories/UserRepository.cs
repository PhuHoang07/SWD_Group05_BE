using AutoMapper;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.DAO;
using GoodsExchangeAtFUManagement.Repository.Repositories.GenericRepositories;

namespace GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(GoodsExchangeAtFuContext context) : base(context)
        {
        }
    }
}
