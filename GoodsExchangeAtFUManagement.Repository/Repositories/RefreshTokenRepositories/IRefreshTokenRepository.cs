using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Repository.Repositories.RefreshTokenRepositories
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
    }
}
