using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Repository.Repositories.PaymentRepositories
{
    public class PaymentRepository : GenericDAO<Payment>, IPaymentRepository
    {
    }
}
