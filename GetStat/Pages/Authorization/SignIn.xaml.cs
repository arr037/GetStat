using System.Security;
using GetStat.ViewModels.Base;

namespace GetStat.Pages.Authorization
{
    /// <summary>
    ///     Логика взаимодействия для SignIn.xaml
    /// </summary>
    public partial class SignIn : BasePage, IHavePassword
    {
        public SignIn()
        {
            InitializeComponent();
        }

        public SecureString SecureString => PasswordText.SecurePassword;
        public bool IsEquals { get; } = true;
    }
}