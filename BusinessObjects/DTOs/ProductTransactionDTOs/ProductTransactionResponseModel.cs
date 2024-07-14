using BusinessObjects.DTOs.ProductPostDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.ProductTransactionDTOs
{
    public class ProductTransactionResponseModel
    {
        public string Id { get; set; } = null!;

        public string Price { get; set; } = null!;

        public DateTime TransactAt { get; set; }

        public ProductPostResponseModel? responseModel { get; set; }

    }

    public class ProductTransactionInSellerViewModel
    {
        public string Id { get; set; } = null!;
        public DateTime TransactAt { get; set; }
        public BuyerInfo BuyerInfo { get; set; } = null!;
    }

    public class BuyerInfo
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
