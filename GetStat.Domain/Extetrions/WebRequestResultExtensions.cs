using GetStat.Domain.Base;
using GetStat.Domain.Web;

namespace GetStat.Domain.Extetrions
{
    public static class WebRequestResultExtensions
    {
        public static BaseError DisplayErrorIfFailedAsync<T>(this WebRequestResult<ApiResponse<T>> response)
        {
            // If there was no response, bad data, or a response with a error message...
            if (response == null || response.ServerResponse == null || !response.ServerResponse.SuccessFul)
            {
                // Default error message
                // TODO: Localize strings
                var message = "Неизвестная ошибка при вызове сервера";

                // Если мы не получили ответ от сервера
                if (response?.ServerResponse != null)
                    message = response.ServerResponse.Error;

                // Если у нас есть результат, но десериализация json не удалась...
                else if (!string.IsNullOrWhiteSpace(response?.RawServerResponse))
                    message = $"Неожиданный ответ от сервера. {response.RawServerResponse}";

                // Если у нас есть результат, но нет никаких подробностей ответа сервера вообще...
                else if (response != null)
                    message =
                        $"Не удалось связаться с сервером. Код состояния: {response.StatusCode}. {response.StatusDescription};{response.ErrorMessage}";


                return new BaseError {Message = message};
            }

            // All was OK, so return false for no error
            return new BaseError();
        }
    }
}