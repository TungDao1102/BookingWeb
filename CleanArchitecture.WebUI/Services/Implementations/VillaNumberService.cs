﻿using CleanArchitecture.WebUI.Models;
using CleanArchitecture.WebUI.Models.DTOs;
using CleanArchitecture.WebUI.Services.Interfaces;
using CleanArchitecture.WebUI.Utilities;

namespace CleanArchitecture.WebUI.Services.Implementations
{
    public class VillaNumberService : IVillaNumberService
    {
        private readonly IBaseService _baseService;
        public VillaNumberService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDTO?> CreateVillaNumber(VillaNumber villaNumber)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                Url = Constants.APIUrlBase + "/api/VillaNumberAPI/CreateVillaNumber",
                Data = villaNumber,
                ApiType = Constants.ApiType.POST
            });
        }

        public async Task<ResponseDTO?> DeleteVillaNumber(int id)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                Url = Constants.APIUrlBase + "/api/VillaNumberAPI/DeleteVillaNumber/" + id,
                ApiType = Constants.ApiType.DELETE
            });
        }

        public async Task<ResponseDTO?> GetAllVillaNumber(QueryParameter queryParameter, string? userId)
        {
            string apiUrl = Constants.APIUrlBase + "/api/VillaNumberAPI/GetAllVillaNumber?PageNumber=" + queryParameter.PageNumber + "&PageSize=" + queryParameter.PageSize;
            if (!string.IsNullOrEmpty(userId))
            {
                apiUrl += $"&userId={userId}";
            }
            return await _baseService.SendAsync(new RequestDTO
            {
                Url = apiUrl,
                ApiType = Constants.ApiType.GET,
 
            });
        }

        public async Task<ResponseDTO?> GetVillaNumberById(int id)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                Url = Constants.APIUrlBase + "/api/VillaNumberAPI/GetVillaNumber/" + id,
                ApiType = Constants.ApiType.GET
            });
        }

        public async Task<ResponseDTO?> UpdateVillaNumber(VillaNumber villaNumber)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                Url = Constants.APIUrlBase + "/api/VillaNumberAPI/UpdateVillaNumber",
                Data = villaNumber,
                ApiType = Constants.ApiType.PUT
            });
        }
    }
}
