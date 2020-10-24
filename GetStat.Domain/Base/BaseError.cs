using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.Domain.Base
{
    public class BaseError
    {
        public string Message { get; set; }
        public bool SuccessFul => string.IsNullOrEmpty(Message);
    }
}
