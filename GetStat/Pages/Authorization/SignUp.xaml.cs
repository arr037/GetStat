using System.Security;
using GetStat.ViewModels.Base;

namespace GetStat.Pages.Authorization
{
    /// <summary>
    ///     Логика взаимодействия для SignUp.xaml
    /// </summary>
    public partial class SignUp : BasePage, IHavePassword
    {
        public SignUp()
        {
            InitializeComponent();
        }

        public SecureString SecureString => PasswordText.SecurePassword;
        public bool IsEquals => PasswordText.Password == PasswordText1.Password;
    }
}