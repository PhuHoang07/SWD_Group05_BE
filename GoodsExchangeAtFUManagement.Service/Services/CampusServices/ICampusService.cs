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
        Task CreateCampus(CampusCreateRequestModel  request, string token);
        Task UpdateCampus(CampusRequestModel request, string token);
        Task DeleteCampus(string id, string token);


    }
}
