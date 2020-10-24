using System.Security;

namespace GetStat.ViewModels.Base
{
    public interface IHavePassword
    {
        SecureString SecureString { get; }
        bool IsEquals { get; }
    }
}