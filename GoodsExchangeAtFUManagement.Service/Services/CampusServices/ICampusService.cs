using BusinessObjects.DTOs.CampusDTOs;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.CampusServices
{
    public interface ICampusService
    {
        Task<List<CampusResponseModel>> GetAllCampus(string searchQuery, int pageIndex, int pageSize);
        Task<CampusResponseModel> GetCampusById(string id);
        Task CreateCampus(CampusCreateRequestModel  request);
        Task UpdateCampus(CampusRequestModel request);
        Task DeleteCampus(string id);


    }
}
