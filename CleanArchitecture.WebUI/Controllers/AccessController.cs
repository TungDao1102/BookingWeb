﻿using CleanArchitecture.WebUI.Models;
using CleanArchitecture.WebUI.Models.DTOs;
using CleanArchitecture.WebUI.Models.ViewModel;
using CleanArchitecture.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CleanArchitecture.WebUI.Controllers
{
    public class AccessController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IRoleService _roleService;
        private readonly ITokenProvider _token;
        public AccessController(IAuthService authService, IRoleService roleService, ITokenProvider token)
        {
            _authService = authService;
            _roleService = roleService;
            _token = token;

        }
        public IActionResult Login(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            LoginVM loginVM = new()
            {
                RedirectUrl = returnUrl
            };
            return View(loginVM);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            ResponseDTO? response = await _authService.Login(loginVM);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Hello " + loginVM.Email;
                AppUserVM appUserVM = JsonConvert.DeserializeObject<AppUserVM>(response.Result.ToString());
                await SignInUser(appUserVM.Token, appUserVM.AppUser);
                _token.SetToken(appUserVM.Token);
                if (string.IsNullOrEmpty(loginVM.RedirectUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return LocalRedirect(loginVM.RedirectUrl);
                }
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(loginVM);
        }
        public async Task<IActionResult> Register(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ResponseDTO? response = await _roleService.GetAllRole();
            List<string>? roleList;
            List<SelectListItem>? roleItems = new List<SelectListItem>();
            if (response != null && response.IsSuccess)
            {
                roleList = JsonConvert.DeserializeObject<List<string>>(Convert.ToString(response.Result));
                roleItems = roleList.Select(r => new SelectListItem { Value = r, Text = r }).ToList();
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            RegisterVM registerVM = new()
            {
                RedirectUrl = returnUrl,
                RoleList = roleItems
            };

            return View(registerVM);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            ResponseDTO? response = await _authService.Register(registerVM);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Register Success " + registerVM.Email;
                if (string.IsNullOrEmpty(registerVM.RedirectUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return LocalRedirect(registerVM.RedirectUrl);
                }

            }
            else
            {
                TempData["error"] = response?.Message;
                response = await _roleService.GetAllRole();
                List<string>? roleList;
                List<SelectListItem>? roleItems = new List<SelectListItem>();
                if (response != null && response.IsSuccess)
                {
                    roleList = JsonConvert.DeserializeObject<List<string>>(Convert.ToString(response.Result));
                    roleItems = roleList.Select(r => new SelectListItem { Value = r, Text = r }).ToList();
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
                registerVM.RoleList = roleItems;
            }            
            return View(registerVM);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _token.ClearToken();
            return RedirectToAction("Index", "Home");
        }
        private async Task SignInUser(string token, AppUser appUser)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwt = handler.ReadJwtToken(token);
            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(x => x.Type == "role").Value));
            identity.AddClaim(new Claim(ClaimTypes.Email, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(ClaimTypes.MobilePhone, appUser.PhoneNumber));
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
