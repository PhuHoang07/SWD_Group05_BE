using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.CoinPackDTOs
{
    public class CoinPackCreateRequestModel
    {
        [Required(ErrorMessage = "Please input amount of coin")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please input a number")]
        public int CoinAmount { get; set; }

        [Required(ErrorMessage = "Please input price of coin")]
        [RegularExpression(@"^\d{1,3}(.\d{3})*$", ErrorMessage = "Please input a valid price in Vietnamese Dong. E.g. 'XXX.XXX' ")]
        public string Price { get; set; } = null!;

    }
}
