using AutoMapper;
using BusinessObjects.DTOs.CampusDTOs;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.CampusRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.CampusServices
{
    public class CampusService : ICampusService
    {
        private readonly IMapper _mapper;
        private readonly ICampusRepository _campusRepository;

        public CampusService(ICampusRepository campusRepository, IMapper mapper)
        {
            _mapper = mapper;
            _campusRepository = campusRepository;
        }

        public async Task CreateCampus(CampusRequestModel request, string token)
        {

            Campus currentCampus = await _campusRepository.GetSingle(c => c.Name.Equals(request.Name));
            if (currentCampus != null)
            {
                throw new Exception("Campus Name has existed");
            }
            Campus newCampus = _mapper.Map<Campus>(request);
            newCampus.Id = Guid.NewGuid().ToString();
            await _campusRepository.Insert(newCampus);
        }

        public async Task DeleteCampus(string id, string token)
        {
            Campus deleteCampus = await _campusRepository.GetSingle(c => c.Id.Equals(id));
            if (deleteCampus == null)
            {
                throw new Exception("Campus not found");
            }
            await _campusRepository.Delete(deleteCampus.Id);
        }


        public async Task<List<CampusResponseModel>> GetAllCampus()
        {
            var campuses = await _campusRepository.Get();
            var campusResponses = _mapper.Map<List<CampusResponseModel>>(campuses);
            return campusResponses;
        }

        public async Task<CampusResponseModel> GetCampusById(string id)
        {
            var campus = await _campusRepository.GetSingle(c => c.Id.Equals(id));
            if (campus == null)
            {
                throw new Exception("Campus not found");
            }
            var campusResponse = _mapper.Map<CampusResponseModel>(campus);
            return campusResponse;
        }

        public async Task UpdateCampus(CampusRequestModel request, string token)
        {
            var campus = await _campusRepository.GetSingle(c => c.Id.Equals(request.Id));
            if (campus == null)
            {
                throw new Exception("Campus not found");
            }
            campus.Name = request.Name;
            await _campusRepository.Update(campus);
        }
    }
}
