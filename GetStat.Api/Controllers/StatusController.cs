using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GetStat.Domain.Base;
using GetStat.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GetStat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatusController : ControllerBase
    {
        private readonly UserManager<Account> _userManager;
        private string UserId => User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
        public StatusController(UserManager<Account> userManager)
        { 
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<ApiResponse<UserRoleResponse>> GetUserRoles()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null || string.IsNullOrEmpty(UserId))
                return new ApiResponse<UserRoleResponse>
                {
                    Error = "Произошла ошибка. Пользователь не найден!"
                };

            var userRolesAsync = await _userManager.GetRolesAsync(user);
            return new ApiResponse<UserRoleResponse>
            {
                Response = new UserRoleResponse
                {
                    Roles = userRolesAsync
                }
            };

        }
    }
}