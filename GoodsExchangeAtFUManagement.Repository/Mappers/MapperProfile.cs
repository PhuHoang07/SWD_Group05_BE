using AutoMapper;
using BusinessObjects.DTOs.CampusDTOs;
using BusinessObjects.DTOs.CategoryDTOs;
using BusinessObjects.DTOs.CoinPackDTOs;
using BusinessObjects.DTOs.PostModeDTOs;
using BusinessObjects.DTOs.ProductPostDTOs;
using BusinessObjects.DTOs.ReportDTOs;
using BusinessObjects.DTOs.UserDTOs;
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
            //User
            CreateMap<UserRegisterRequestModel, User>();
            CreateMap<UserRegisterRequestTestingModel, User>();
            CreateMap<UpdateUserRequestModel, User>(); 
            CreateMap<User, ViewUserResponseModel>();  

            //Campus
            CreateMap<CampusRequestModel, Campus>();
            CreateMap<CampusCreateRequestModel, Campus>();
            CreateMap<Campus, CampusResponseModel>();
            //Category
            CreateMap<CategoryRequestModel, Category>();
            CreateMap<CategoryCreateRequestModel, Category>();
            CreateMap<Category, CategoryResponseModel>();
            //CoinPack
            CreateMap<CoinPackCreateRequestModel, CoinPack>();
            CreateMap<CoinPackUpdateRequestModel, CoinPack>();
            CreateMap<CoinPack, CoinPackResponseModel > ();
            CreateMap<CoinPack, CoinPackManageResponseModel>();
            //ProductPost
            CreateMap<ProductPostCreateRequestModel, ProductPost>();
            //PostMode
            CreateMap<PostModeCreateRequestModel, PostMode>();
            CreateMap<PostModeUpdateRequestModel, PostMode>();
            CreateMap<PostMode, PostModeResponseModel>();
            CreateMap<PostMode, PostModeManageResponseModel>();
            //Report
            CreateMap<ReportRequestModel, Report>();
            CreateMap<CreateReportRequestModel, Report>();
            CreateMap<Report, ReportResponseModel>();



        }
    }
}
