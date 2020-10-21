

using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GetStat.Domain.Base
{
    public class ApiResponse
    {
        public bool SuccessFul => string.IsNullOrEmpty(Error);
        public string Error { get; set; }
        public object Response { get; set; }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public new T Response
        {
            get => (T)base.Response; set => base.Response = value;
        }
    }
}
