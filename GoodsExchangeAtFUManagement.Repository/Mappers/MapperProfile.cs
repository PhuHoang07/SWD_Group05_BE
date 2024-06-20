using AutoMapper;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Repository.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserRegisterRequestModel, User>();
            CreateMap<UserRegisterRequestTestingModel, User>();
        }
    }
}
