using BusinessObjects.DTOs.CampusDTOs;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.CampusServices
{
    public interface ICampusService
    {
        Task<List<CampusResponseModel>> GetAllCampus();
        Task<CampusResponseModel> GetCampusById(string id);
        Task CreateCampus(CampusRequestModel  request);
        Task UpdateCampus(CampusRequestModel request);
        Task DeleteCampus(string id);


    }
}
