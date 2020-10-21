using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GetStat.ViewModels.Base;

namespace GetStat.Pages
{
    /// <summary>
    /// Логика взаимодействия для SignUp.xaml
    /// </summary>
    public partial class SignUp : Page,IHavePassword
    {
        public SignUp()
        {
            InitializeComponent();
        }

        public SecureString SecureString => PasswordText.SecurePassword;
        public bool IsEquals => PasswordText.Password == PasswordText1.Password;
    }
}
