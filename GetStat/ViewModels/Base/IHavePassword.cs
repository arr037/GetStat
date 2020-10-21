using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace GetStat.ViewModels.Base
{
    public interface IHavePassword
    {
        SecureString SecureString { get; }
        bool IsEquals { get; }
    }
}
