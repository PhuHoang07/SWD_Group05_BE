﻿using BusinessObjects.DTOs;
using BusinessObjects.DTOs.CoinTransactionDTOs;
using BusinessObjects.DTOs.VnPayDTOs;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.CoinPackRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.CoinTransactionRepositories;
using GoodsExchangeAtFUManagement.Service.Services.VnPayServices;
using GoodsExchangeAtFUManagement.Service.Ultis;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Service.Services.CoinTransactionServices
{
    public class CoinTransactionService : ICoinTransactionService
    {
        private readonly ICoinTransactionRepository _coinTransactionRepository;
        private readonly ICoinPackRepository _coinPackRepository;
        private readonly IVnPayService _vnPayService;

        private readonly int ItemsPerPage = 10;
        public CoinTransactionService(ICoinTransactionRepository coinTransactionRepository, ICoinPackRepository coinPackRepository, IVnPayService vpnPayService)
        {
            _coinTransactionRepository = coinTransactionRepository;
            _coinPackRepository = coinPackRepository;
            _vnPayService = vpnPayService;
        }

        public async Task<string> CreateCoinTransaction(string coinPackId, string token)
        {
            var userId = JwtGenerator.DecodeToken(token, "userId");
            var coinPack = await _coinPackRepository.GetSingle(c => c.Id.Equals(coinPackId));
            var newCoinTransaction = new CoinTransaction
            {
                Id = Guid.NewGuid().ToString(),
                TransactAt = DateTime.Now,
                Status = CoinTransactionStatus.Pending.ToString(),
                CoinPackId = coinPackId,
                UserId = userId,
                Price = coinPack.Price.Replace(".", "")
            };
            await _coinTransactionRepository.Insert(newCoinTransaction);

            return newCoinTransaction.Id;
        }

        public async Task<string> GetPaymentUrl(HttpContext context, string transactId, string redirectUrl)
        {
            var currentTransact = await _coinTransactionRepository.GetSingle(c => c.Id.Equals(transactId));

            if (currentTransact == null)
            {
                throw new CustomException("Payment does not exist!");
            }

            if (currentTransact.Status.Equals(nameof(CoinTransactionStatus.Success)))
            {
                throw new CustomException("Payment has already been paid!");
            }

            VnPaymentRequestModel vnpay = new VnPaymentRequestModel
            {
                OrderId = currentTransact.CoinPackId,
                PaymentId = currentTransact.Id,
                Amount = decimal.Parse(currentTransact.Price),
                CreatedDate = currentTransact.TransactAt,
                RedirectUrl = redirectUrl
            };

            return _vnPayService.CreatePaymentUrl(context, vnpay);
        }

        public async Task<CoinTransaction> UpdateCoinTransactionStatus(CoinTransactionUpdateReqModel reqModel)
        {
            var coinTransact = await _coinTransactionRepository.GetSingle(c => c.Id.Equals(reqModel.TransactId));
            if (coinTransact == null)
            {
                throw new CustomException("Coin transaction is not existed");
            }
            if (!coinTransact.Status.Equals(CoinTransactionStatus.Pending.ToString()))
            {
                throw new CustomException("This transaction is not in pending status");
            }
            if (!Enum.IsDefined(typeof(CoinTransactionStatus), reqModel.Status))
            {
                throw new CustomException("Please choose valid status");
            }
            coinTransact.Status = reqModel.Status;
            coinTransact.TransactAt = DateTime.Now;
            await _coinTransactionRepository.Update(coinTransact);
            return coinTransact;
        }

        public async Task<Pagination<CoinTransactionResModel>> ViewAllCoinTransaction(int? pageIndex)
        {
            var allList = await _coinTransactionRepository.Get(null, null, includeProperties: "User, CoinPack", pageIndex: pageIndex ?? 1, ItemsPerPage);
            var count = await _coinTransactionRepository.Count();
            var responseList = allList.Select(a => new CoinTransactionResModel
            {
                Id = a.Id,
                User = new UserInfo
                {
                    Id = a.User.Id,
                    Email = a.User.Email,
                    PhoneNumber = a.User.PhoneNumber
                },
                CoinAmount = a.CoinPack.CoinAmount,
                Price = a.CoinPack.Price,
                Status = a.Status,
                TransactAt = a.TransactAt
            }).ToList();
            if (responseList.Count == 0)
            {
                return null;
            }
            return new Pagination<CoinTransactionResModel>
            {
                TotalItems = count,
                CurrentPage = pageIndex ?? 1,
                PageSize = ItemsPerPage,
                Data = responseList
            };
        }

        public async Task<Pagination<MyCoinTransactionResModel>> ViewOwnCoinTransaction(string token, int? pageIndex)
        {
            var userId = JwtGenerator.DecodeToken(token, "userId");
            var allList = await _coinTransactionRepository.Get(c => c.UserId.Equals(userId), null, includeProperties: "CoinPack", pageIndex: pageIndex ?? 1, ItemsPerPage);
            var count = await _coinTransactionRepository.Count(c => c.UserId.Equals(userId));
            var responseList = allList.Select(a => new MyCoinTransactionResModel
            {
                Id = a.Id,
                CoinAmount = a.CoinPack.CoinAmount,
                Price = a.CoinPack.Price,
                Status = a.Status,
                TransactAt = a.TransactAt
            }).ToList();
            if (responseList.Count == 0)
            {
                return null;
            }
            return new Pagination<MyCoinTransactionResModel>
            {
                TotalItems = count,
                CurrentPage = pageIndex ?? 1,
                PageSize = ItemsPerPage,
                Data = responseList
            };
        }
    }
}
