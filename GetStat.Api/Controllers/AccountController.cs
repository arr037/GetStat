using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GetStat.Api.Domain;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models;
using GetStat.Domain.Services;
using GetStat.Domain.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace GetStat.Api.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly AppDbContext _dbContext;
        private readonly UserManager<Account> _userManager;

        public AccountController(AppDbContext dbContext, UserManager<Account> userManager, EmailService emailService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailService = emailService;
        }


        [HttpGet]
        [Route("")]
        public string Check()
        {
            return "Ok";
        }


        [Route("api/register")]
        [HttpPost]
        public async Task<ApiResponse<string>> Register(Account account)
        {
            if (account == null)
                return new ApiResponse<string> {Error = "Сервер не отвечает"};

            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(account, account.PasswordHash);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(account.UserName);
                    var code = (await _userManager.GenerateEmailConfirmationTokenAsync(user)).Base64ForUrlEncode();
                    var emailuri = "https://" + HttpContext.Request.Host.Value +
                                   $"/api/Confirm?id={user.Id}&token={code}";

                   var isSendEmail = await _emailService.SendEmailAsync(account.Email, "Подтвердите email", emailuri);

                   if (!isSendEmail)
                       return new ApiResponse<string>
                       {
                           Error = "При отправке email, произошла ошибка"
                       };

                    return new ApiResponse<string>
                    {
                        Response = account.Id
                    };
                }

                return new ApiResponse<string>
                {
                    Error = string.Join('\n',
                        result.Errors.Select(x => "Код ошибки: " + x.Code + "\n\t" + x.Description))
                };
            }

            var allErrors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage);

            return new ApiResponse<string>
            {
                Error = string.Join('\n', allErrors)
            };
        }

        [Route("api/login")]
        [HttpPost]
        public async Task<ApiResponse<LoginResponse>> Login(LoginViewModel model)
        {
            if (model == null)
            {
                return new ApiResponse<LoginResponse>
                {
                    Error = "Сервер не отвечает"
                };
            }

            if (ModelState.IsValid)
            {
                var err = await IsValidUserNamePasswordAndConfirmedEmail(model);
                if (err.SuccessFul)
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Response = await GenerateToken(model.UserName)
                    };
                }
                
                return new ApiResponse<LoginResponse>
                {
                    Error = err.Message
                };
                
            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage);

            return new ApiResponse<LoginResponse>
            {
                Error = string.Join('\n', allErrors)
            };
        }


        private async Task<BaseError> IsValidUserNamePasswordAndConfirmedEmail(LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            var checkPassword = await _userManager.CheckPasswordAsync(user, model.Password);

            if (checkPassword)
            {
                var confirmed = await _userManager.IsEmailConfirmedAsync(user);
                if (confirmed)
                    return new BaseError();
                return new BaseError
                {
                    Message = "Подтверите свой email"
                };
            }

            return new BaseError
            {
                Message = "Неверный логин или пароль"
            };
        }

        private async Task<LoginResponse> GenerateToken(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var roles = from ur in _dbContext.UserRoles
                join r in _dbContext.Roles on ur.RoleId equals r.Id
                where ur.UserId == user.Id
                select new {ur.UserId, ur.RoleId, r.Name};

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp,
                    new DateTimeOffset(DateTime.Now.AddHours(2)).ToUnixTimeSeconds().ToString()),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var token = new JwtSecurityToken(
                new JwtHeader(
                    new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mySuperPass001-*+")),
                        SecurityAlgorithms.HmacSha256)),
                new JwtPayload(claims));

            return new LoginResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Name = user.Name,
                Surname = user.Surname,
                MiddleName = user.MiddleName
            };
        }

    
}
    
}