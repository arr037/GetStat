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
    /// Логика взаимодействия для SignIn.xaml
    /// </summary>
    public partial class SignIn : Page,IHavePassword
    {
        public SignIn()
        {
            InitializeComponent();
        }

        public SecureString SecureString => PasswordText.SecurePassword;
        public bool IsEquals { get; } = true;
    }
}
