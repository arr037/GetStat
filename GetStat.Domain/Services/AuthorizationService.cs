using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dna;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models;

namespace GetStat.Services
{
    public class AuthorizationService
    {
        private readonly ModalService _modalService;
        public string AccountId { get; private set; }


        public AuthorizationService(ModalService modalService)
        {
            _modalService = modalService;
        }

        public async Task<bool> Register(Account account)
        {
            var error =  CheckError(account);

            if (!string.IsNullOrEmpty(error))
            {
                _modalService.ShowModalWindow("Список ошибок:", error);
                return false;
            }


            var response = await WebRequests.PostAsync<ApiResponse<string>>(
                "https://localhost:5001/api/register", account);

            var res = response.DisplayErrorIfFailedAsync();
            if (res.SuccessFul == false)
            {
                _modalService.ShowModalWindow("Ошибка при регистрации", res.Message);
                return false;
            }

            AccountId = response.ServerResponse.Response;
            return true;
        }

        public async Task<bool> ConfirmEmail()
        {
            var data = $"https://localhost:5001/api/Confirm?id={AccountId}";
            var response = await WebRequests.PostAsync<bool>(data);

            if (response?.ServerResponse != null)
            {
                return response.ServerResponse;
            }

            return false;
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
                    if (account.Email.Trim().StartsWith("@"))
                    {
                        errors.Add("Email адрес не может начинатся с '@'");
                    }
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
