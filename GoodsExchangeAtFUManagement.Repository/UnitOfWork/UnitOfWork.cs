using GoodsExchangeAtFUManagement.DAO;
using GoodsExchangeAtFUManagement.Repository.Repositories.OTPCodeRepositories;
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
        private readonly IOTPCodeRepository _otpCodeRepository;

        public UnitOfWork(GoodsExchangeAtFuContext context, IUserRepository userRepository, IOTPCodeRepository otpCodeRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _otpCodeRepository = otpCodeRepository;
        }

        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public IUserRepository UserRepository => _userRepository;
        public IOTPCodeRepository OTPCodeRepository => _otpCodeRepository;
    }
}
