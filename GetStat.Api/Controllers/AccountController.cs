using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GetStat.Api.Domain;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models;
using GetStat.Domain.Services;
using GetStat.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GetStat.Api.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<Account> _userManager;
        private readonly EmailService _emailService;

        public AccountController(AppDbContext dbContext, UserManager<Account> userManager,EmailService emailService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailService = emailService;
        }


        [HttpGet]
        [Route("")]
        public string Check() => "Ok";


        [Route("api/register")]
        [HttpPost]
        public async Task<ApiResponse<string>> Register(Account account)
        {
            if (account == null)
                return new ApiResponse<string> { Error = "Сервер не отвечает"};

            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(account,account.PasswordHash);
                if (result.Succeeded)
                {
                    var user = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.UserName == account.UserName);
                    var code = (await _userManager.GenerateEmailConfirmationTokenAsync(user)).Base64ForUrlEncode();
                    var emailuri = "https://" + HttpContext.Request.Host.Value+$"/api/Confirm?id={user.Id}&token={code}";

                    Console.WriteLine("\n\n\n"+emailuri+"\n\n\n");
                    await _emailService.SendEmailAsync(account.Email, "Подтвердите email", emailuri);

                    return new ApiResponse<string>
                    {
                        Response = account.Id
                    };
                }

                return new ApiResponse<string>
                {
                    Error = string.Join('\n', result.Errors.Select(x=>"Код ошибки: "+ x.Code+"\n\t"+x.Description))
                };
            }

            var allErrors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage);

            return new ApiResponse<string>
            {
                Error = string.Join('\n',allErrors)
            };
        }

        //[Route("api/login")]
        //[HttpPost]
        //public async Task<ApiResponse> Login(LoginViewModel model)
        //{
        //    if (model == null)
        //    {
        //        // return error
        //    }

        //    var user = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.UserName == model.UserName);

        //    if (user == null)
        //    {
        //        // return error
        //    }
        //    var hasher = new PasswordHasher<string>();
        //    var pass = hasher.VerifyHashedPassword(model.UserName, user.Password, model.Password);
        //    if (pass != PasswordVerificationResult.Success)
        //    {
        //        // return error
        //    }

        //    //Generate jwn,
        //    //return token


        //    return null;
        //}


        //private async Task<List<string>> CheckError(Account account)
        //{
        //    var errors = new List<string>();

        //    if (string.IsNullOrWhiteSpace(account.UserName))
        //        errors.Add("Введите логин");

        //    if (string.IsNullOrWhiteSpace(account.Name))
        //        errors.Add("Введите ваше имя");

        //    if (string.IsNullOrWhiteSpace(account.Surname))
        //        errors.Add("Введите вашу фамилию");

        //    if (string.IsNullOrWhiteSpace(account.MiddleName))
        //        errors.Add("Введите отчетсво");

           
        //    }
        //    //
        //    var accounts = _dbContext.Accounts;

        //    if (await accounts.AnyAsync(x => x.UserName == account.UserName))
        //    {
        //        errors.Add("Такой логин уже существует");
        //    }
        //    if (await accounts.AnyAsync(x => x.Email == account.Email && x.EmailConfirmed))
        //    {
        //        errors.Add("Такой email уже зарегестирован в базе");
        //    }

        //    return errors;
        //}
    }
}