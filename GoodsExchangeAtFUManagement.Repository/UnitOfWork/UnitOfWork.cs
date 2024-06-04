using GoodsExchangeAtFUManagement.Repository.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GoodsExchangeAtFuContext _context;
        private readonly IUserRepository _userRepository;
        
        public UnitOfWork(GoodsExchangeAtFuContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public IUserRepository UserRepository => _userRepository;
    }
}
