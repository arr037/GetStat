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
using Microsoft.AspNetCore.Authorization;
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
        private string UserId => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


        [Route("api/resetpassword")]
        [HttpPost]
        public async Task<ApiResponse<string>> ResetPassword([FromBody]string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return new ApiResponse<string>
                {
                    Error = "Пользователь не найден"
                };

            if (!user.EmailConfirmed)
                return new ApiResponse<string>
                {
                    Error = "Восстановление пароля невозможно."
                };

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            
            var callbackUrl = Url.Action("ResetPassword", "Confirm", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
            var check = await _emailService.SendEmailAsync(user.Email, "Восстановление пароля", $"Для сброса пароля пройдите по ссылке: <a href='{callbackUrl}'>link</a>");
            if (!check)
                return new ApiResponse<string>
                {
                    Error = "При отправке пароля произошла ошибка.Повторите попытку позже"
                };


            var userMail = user.Email.ToCharArray();

            userMail[0] = '*';
            userMail[1] = '*';
            userMail[2] = '*';


            return new ApiResponse<string>
            {
                Response = $"На вашу почту {string.Join("",userMail)} отправлено сообщение.\nПроверьте вашу почту"
            };
        }

        [Authorize]
        [Route("api/changePassword")]
        [HttpPost]
        public async Task<ApiResponse<bool>> ChangePassword(string[] param)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return new ApiResponse<bool>
                {
                    Error = "Пользователь не найден"
                };

            var b = await _userManager.ChangePasswordAsync(user, param[0], param[1]);

            if (b.Succeeded)
                return new ApiResponse<bool>
                {
                    Response = true
                };

            return new ApiResponse<bool>
            {
                Error = "Произошла ошибка при изменении пароля"
            };

        }

        [Authorize]
        [Route("api/changeName")]
        [HttpPost]
        public async Task<ApiResponse<int>> ChangeFullName(string[] param)
        {
            var user = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
                return new ApiResponse<int>
                {
                    Error = "Пользователь не найден"
                };

            user.Name = param[1];
            user.Surname = param[0];
            user.MiddleName = param[2];

            var res = await _dbContext.SaveChangesAsync();

            return new ApiResponse<int>
            {
                Response = res
            };

        }



        [Route("api/register")]
        [HttpPost]
        public async Task<ApiResponse<LoginResponse>> Register(Account account)
        {
            if (account == null)
                return new ApiResponse<LoginResponse> {Error = "Сервер не отвечает"};

            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(account, account.PasswordHash);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(account.UserName);
                    
                    var isSendEmail = await SendEmailCode(user);

                   if (!isSendEmail)
                       return new ApiResponse<LoginResponse>
                       {
                           Error = "При отправке email, произошла ошибка"
                       };

                    return new ApiResponse<LoginResponse>
                    {
                        Response = await GenerateToken(user.UserName)
                    };
                }

                return new ApiResponse<LoginResponse>
                {
                    Error = string.Join('\n',
                        result.Errors.Select(x => "Код ошибки: " + x.Code + "\n\t" + x.Description))
                };
            }

            var allErrors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage);

            return new ApiResponse<LoginResponse>
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

                var isSend=  await SendEmailCode(user);
               
                if(!isSend)
                    return new BaseError
                    {
                        Message = "При отправке email, произошла ошибка"
                    };

                return new BaseError
                {
                    Message = "Подтверите свой email!\nНа вашу почту повторно был отправлен код"
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

        private async Task<bool> SendEmailCode(Account user)
        {
            var code = (await _userManager.GenerateEmailConfirmationTokenAsync(user))
                .Base64ForUrlEncode();

            string https = string.Empty;
            https = HttpContext.Request.IsHttps ? "https://" : "http://";

            var emailuri = https + HttpContext.Request.Host.Value +
                           $"/api/ConfirmEmail?token={code}&id={user.Id}";

            var isSendEmail = await _emailService.SendEmailAsync(user.Email, "Подтвердите email", emailuri);

            return isSendEmail;
        }
}
    
}