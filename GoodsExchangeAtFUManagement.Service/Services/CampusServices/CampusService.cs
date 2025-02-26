﻿using AutoMapper;
using BusinessObjects.DTOs.CampusDTOs;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.CampusRepositories;
using GoodsExchangeAtFUManagement.Service.Ultis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task CreateCampus(CampusCreateRequestModel request)
        {
            Campus currentCampus = await _campusRepository.GetSingle(c => c.Name.Equals(request.Name));
            if (currentCampus != null)
            {
                throw new CustomException("Campus Name has existed");
            }
            Campus newCampus = _mapper.Map<Campus>(request);
            newCampus.Id = Guid.NewGuid().ToString();
            newCampus.Status = true;
            await _campusRepository.Insert(newCampus);
        }

        public async Task DeleteCampus(string id)
        {
            Campus deleteCampus = await _campusRepository.GetSingle(c => c.Id.Equals(id));
            if (deleteCampus == null)
            {
                throw new CustomException("Campus not found");
            }
            if (deleteCampus.Status == false)
            {
                throw new CustomException("Campus is already be removed");
            }
            deleteCampus.Status = false;
            await _campusRepository.Update(deleteCampus);
        }

        public async Task<List<CampusResponseModel>> GetAllCampus(string searchQuery, int pageIndex, int pageSize)
        {
            Expression<Func<Campus, bool>> searchFilter = c => c.Status == true && (string.IsNullOrEmpty(searchQuery) || c.Name.Contains(searchQuery)); ;
            var campuses = await _campusRepository.Get(searchFilter, pageIndex: pageIndex, pageSize: pageSize);
            var campusResponses = _mapper.Map<List<CampusResponseModel>>(campuses);
            return campusResponses;
        }

        public async Task<CampusResponseModel> GetCampusById(string id)
        {         
            var campus = await _campusRepository.GetSingle(c => c.Id.Equals(id));
            if (campus == null)
            {
                throw new CustomException("Campus not found");
            }
            var campusResponse = _mapper.Map<CampusResponseModel>(campus);
            return campusResponse;
        }

        public async Task UpdateCampus(CampusRequestModel request)
        {
            var campus = await _campusRepository.GetSingle(c => c.Id.Equals(request.Id));
            if (campus == null|| campus.Status == false)
            {
                throw new CustomException("Campus not found");
            }
            if (!string.IsNullOrEmpty(request.Name))
            {
               campus.Name = request.Name;
            }

            await _campusRepository.Update(campus);
        }

    }
}
