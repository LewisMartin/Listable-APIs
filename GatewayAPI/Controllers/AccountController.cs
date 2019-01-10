﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using GatewayAPI.Services;
using Listable.UserMicroservice.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GatewayAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult AccountLoginCheck()
        {
            var response = _userService.CheckForUserEntry(GetUserSub()).Result;

            if (response.IsSuccessStatusCode)
                return Ok();

            response = _userService.CreateUser(new UserDetails()
            {
                SubjectId = GetUserSub(),
                DisplayName = GenerateDisplayName()
            }).Result;

            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return Ok();
        }

        private string GetUserSub()
        {
            string sub = "";

            foreach (var identity in User.Identities)
            {
                sub = identity.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
            }

            return sub;
        }

        private string GenerateDisplayName()
        {
            string name = "";
            int size = 0;

            StringLengthAttribute strLenAttr = typeof(UserDetails).GetProperty("DisplayName").GetCustomAttributes(typeof(StringLengthAttribute), false).Cast<StringLengthAttribute>().SingleOrDefault();
            if (strLenAttr != null)
                size = strLenAttr.MaximumLength;

            foreach (var identity in User.Identities)
            {
                name = identity.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").FirstOrDefault().Value;
            }

            name = Regex.Replace(name, @"\s+", "");

            name = name.Length < size-10 ? name : name.Substring(0, size-10);

            name = name + Guid.NewGuid().ToString("n").Substring(0, 10);

            var response = _userService.CheckDisplayName(name).Result;

            if (response.IsSuccessStatusCode)
                return name;
            else
                return GenerateDisplayName();
        }
    }
}
