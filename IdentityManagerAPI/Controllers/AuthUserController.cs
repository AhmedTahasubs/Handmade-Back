﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Auth;
using Models;
using System.Net;
using DataAcess.Repos.IRepos;
using Models.Domain;
using IdentityManager.Services.ControllerService.IControllerService;

namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUserController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthUserController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var result = await _authService.LoginAsync(loginRequestDTO);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            var result = await _authService.RegisterAdminAsync(registerRequestDTO);
            return Ok(result);
        }

        [HttpPost("register/seller")]
        public async Task<IActionResult> RegisterSeller([FromBody] SellerRegisterDto sellerRegisterDto)
        {
            var result = await _authService.RegisterSellerAsync(sellerRegisterDto);
            return Ok(result);
        }

        [HttpPost("register/customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegisterDto customerRegisterDto)
        {
            var result = await _authService.RegisterCustomerAsync(customerRegisterDto);
            return Ok(result);
        }
    }
}
