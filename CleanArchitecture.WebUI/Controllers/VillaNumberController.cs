﻿using CleanArchitecture.WebUI.Models;
using CleanArchitecture.WebUI.Models.DTOs;
using CleanArchitecture.WebUI.Models.ViewModel;
using CleanArchitecture.WebUI.Services.Interfaces;
using CleanArchitecture.WebUI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Security.Claims;

namespace CleanArchitecture.WebUI.Controllers
{
    [Authorize(Roles = Constants.Role_Admin + "," + Constants.Role_Customer + "," + Constants.Role_Manager)]
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        public VillaNumberController(IVillaNumberService villaNumberService, IVillaService villaService)
        {
            _villaNumberService = villaNumberService;
            _villaService = villaService;
        }
        public async Task<IActionResult> Index(int? page)
        {
            QueryParameter query = new QueryParameter
            {
                PageSize = 7,
                PageNumber = (page == null || page < 0) ? 1 : page.Value
            };
            ViewBag.PageNumber = page ?? 1;
            string? userId = null;
            if (User.IsInRole(Constants.Role_Customer))
            {
                userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            ResponseDTO? response = await _villaNumberService.GetAllVillaNumber(query, userId);
            List<VillaNumber> villaNumberList = null;
            PageResult<VillaNumber>? pageResult = new PageResult<VillaNumber>();
            if (response != null && response.IsSuccess)
            {
                pageResult = JsonConvert.DeserializeObject<PageResult<VillaNumber>>(Convert.ToString(response.Result));
                villaNumberList = pageResult.Items;
                ViewBag.TotalCount = (int)Math.Ceiling((double)pageResult.TotalCount / query.PageSize);
            }
            else
            {
                TempData["error"] = response?.Message;
                return RedirectToAction("Index", "Home");
            }
            return View(villaNumberList);
        }
        public async Task<IActionResult> Create()
        {
            string? userId = null;
            if (User.IsInRole(Constants.Role_Customer))
            {
                userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            ResponseDTO? response = await _villaService.GetAllVilla(userId);
            IEnumerable<Villa> listVilla = new List<Villa>();
            if (response != null && response.IsSuccess)
            {
                listVilla = JsonConvert.DeserializeObject<IEnumerable<Villa>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            VillaNumberVM villaNumberVM = new VillaNumberVM
            {
                VillaList = listVilla.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(villaNumberVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(VillaNumberVM villaNumberVM)
        {
            ResponseDTO? response = await _villaNumberService.CreateVillaNumber(villaNumberVM.VillaNumber);
            if (response.Result != null && response.IsSuccess)
            {
                TempData["success"] = "The villa Number has been created successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
                string? userId = null;
                if (User.IsInRole(Constants.Role_Customer))
                {
                    userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                response = await _villaService.GetAllVilla(userId);
                IEnumerable<Villa> listVillaNumber = new List<Villa>();
                if (response != null && response.IsSuccess)
                {
                    listVillaNumber = JsonConvert.DeserializeObject<IEnumerable<Villa>>(Convert.ToString(response.Result));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
                villaNumberVM.VillaList = listVillaNumber.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
            }
            return View(villaNumberVM);
        }
        public async Task<IActionResult> Update(int villaNumberId)
        {
            string? userId = null;
            if (User.IsInRole(Constants.Role_Customer))
            {
                userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            ResponseDTO? response1 = await _villaService.GetAllVilla(userId);
            IEnumerable<Villa> listVilla = new List<Villa>();
            VillaNumber villaNumber = new VillaNumber();
            ResponseDTO? response2 = await _villaNumberService.GetVillaNumberById(villaNumberId);
            if ((response1 != null && response1.IsSuccess) && (response2 != null && response2.IsSuccess))
            {
                listVilla = JsonConvert.DeserializeObject<IEnumerable<Villa>>(Convert.ToString(response1.Result));
                villaNumber = JsonConvert.DeserializeObject<VillaNumber>(Convert.ToString(response2.Result));
            }
            else
            {
                TempData["error"] = response1?.Message;
                TempData["error"] = response2?.Message;
            }
            VillaNumberVM villaNumberVM = new VillaNumberVM
            {
                VillaList = listVilla.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                VillaNumber = villaNumber
            };
            return View(villaNumberVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(VillaNumberVM villaNumberVM)
        {
            ResponseDTO? response = await _villaNumberService.UpdateVillaNumber(villaNumberVM.VillaNumber);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "The villa Number has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
                string? userId = null;
                if (User.IsInRole(Constants.Role_Customer))
                {
                    userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                ResponseDTO? response1 = await _villaService.GetAllVilla(userId);
                IEnumerable<Villa> listVilla = new List<Villa>();
                if (response1 != null && response1.IsSuccess)
                {
                    listVilla = JsonConvert.DeserializeObject<IEnumerable<Villa>>(Convert.ToString(response1.Result));
                }
                else
                {
                    TempData["error"] = response1?.Message;
                }
                villaNumberVM.VillaList = listVilla.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
            }
            return View(villaNumberVM);
        }

        public async Task<IActionResult> Delete(int villaNumberId)
        {
            string? userId = null;
            if (User.IsInRole(Constants.Role_Customer))
            {
                userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            ResponseDTO? response1 = await _villaService.GetAllVilla(userId);
            IEnumerable<Villa> listVilla = new List<Villa>();
            VillaNumber villaNumber = new VillaNumber();
            ResponseDTO? response2 = await _villaNumberService.GetVillaNumberById(villaNumberId);
            if ((response1 != null && response1.IsSuccess) && (response2 != null && response2.IsSuccess))
            {
                listVilla = JsonConvert.DeserializeObject<IEnumerable<Villa>>(Convert.ToString(response1.Result));
                villaNumber = JsonConvert.DeserializeObject<VillaNumber>(Convert.ToString(response2.Result));
            }
            else
            {
                TempData["error"] = (response1?.Message ?? "") + " " + (response2?.Message ?? "");
            }
            VillaNumberVM villaNumberVM = new VillaNumberVM
            {
                VillaList = listVilla.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                VillaNumber = villaNumber
            };
            return View(villaNumberVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(VillaNumberVM villaNumberVM)
        {
            ResponseDTO? response = await _villaNumberService.DeleteVillaNumber(villaNumberVM.VillaNumber.Villa_Number);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "The villa Number has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(villaNumberVM);
        }
    }
}
