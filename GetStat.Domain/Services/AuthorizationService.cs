using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GetStat.Domain;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models;
using GetStat.Domain.Services;
using GetStat.Domain.ViewModels;
using GetStat.Domain.Web;

namespace GetStat.Services
{
    public class AuthorizationService
    {
        private readonly ModalService _modalService;
        private readonly LoginResponseService _loginResponseService;

        public AuthorizationService(ModalService modalService,
            LoginResponseService loginResponseService)
        {
            _modalService = modalService;
            _loginResponseService = loginResponseService;
        }

        public async Task<bool> Register(Account account)
        {
            var error = CheckError(account);

            if (!string.IsNullOrEmpty(error))
            {
                _modalService.ShowModalWindow("Список ошибок:", error);
                return false;
            }

            var response = await WebRequests.PostAsync<ApiResponse<LoginResponse>>(
                Config.UrlAddress+"api/register", account);

            var res = response.DisplayErrorIfFailedAsync();
            if (res.SuccessFul == false)
            {
                _modalService.ShowModalWindow("Ошибка при регистрации", res.Message);
                return false;
            }

            _loginResponseService.LoginResponse = response.ServerResponse.Response;
            return true;
        }

        public async Task<bool> ConfirmEmail()
        {
            var response = await WebRequests.
                PostAsync<ApiResponse<bool>>(
                Config.UrlAddress + "api/IsConfirm", bearerToken:_loginResponseService.LoginResponse.Token);

            var res = response.DisplayErrorIfFailedAsync();
            if (res.SuccessFul == false)
            {
                _modalService.ShowModalWindow("Ошибка", res.Message);
                return false;
            }

            return response.ServerResponse.Response;
        }


        public async Task<bool> Login(LoginViewModel model)
        {
            var response = await WebRequests.PostAsync<ApiResponse<LoginResponse>>(Config.UrlAddress+"api/login", model);

            var res = response.DisplayErrorIfFailedAsync();
            if (res.SuccessFul == false)
            {
                _modalService.ShowModalWindow("Ошибка при авторизации", res.Message);
                return false;
            }

            _loginResponseService.LoginResponse = response.ServerResponse.Response;
            return true;
        }

        private string CheckError(Account account)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(account.UserName))
                errors.Add("Введите логин");

            if (string.IsNullOrWhiteSpace(account.Name))
                errors.Add("Введите ваше имя");

            if (string.IsNullOrWhiteSpace(account.Surname))
                errors.Add("Введите вашу фамилию");

            if (string.IsNullOrWhiteSpace(account.MiddleName))
                errors.Add("Введите отчетсво");

            if (string.IsNullOrEmpty(account.Email))
            {
                errors.Add("Введите email адрес");
            }
            else
            {
                var dat = account.Email.Count(c => c == '@');
                if (dat == 1)
                {
                    if (account.Email.Trim().StartsWith("@")) errors.Add("Email адрес не может начинатся с '@'");
                }
                else if (dat > 1)
                {
                    errors.Add($"{dat} '@' невожножно");
                }

                else
                {
                    errors.Add("Введите корректно email");
                }
            }
            //

            if (string.IsNullOrWhiteSpace(account.PasswordHash))
                errors.Add("Введите корректный пароль");


            return errors.Count == 0 ? null : string.Join("\n", errors);
        }
    }
}