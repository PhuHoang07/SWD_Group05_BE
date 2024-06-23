using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.CoinPackDTOs
{
    public class CoinPackUpdateRequestModel
    {
        [Required(ErrorMessage = "Please input coin pack Id")]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "Please input amount of coin")]
        public int CoinAmount { get; set; }

        [Required(ErrorMessage = "Please input price of coin pack")]
        public string Price { get; set; } = null!;
    }
}
