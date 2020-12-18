using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Domain.Base;

namespace GetStat.Domain.Services
{
    public class LoginResponseService
    {
        public LoginResponse LoginResponse { get; internal set; }

        public void Clear()
        {
            LoginResponse = null;
            GC.Collect();
        }
    }
}
