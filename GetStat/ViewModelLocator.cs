using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.ViewModels;
using GetStat.ViewModels.PagesViewModels;

namespace GetStat
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel => Ioc.Resolve<MainViewModel>();
        public SignInViewModel SignInViewModel => Ioc.Resolve<SignInViewModel>();
        public SignUpViewModel SignUpViewModel => Ioc.Resolve<SignUpViewModel>();
        public ConfirmEmailViewModel ConfirmEmailViewModel=> Ioc.Resolve<ConfirmEmailViewModel>();
    }
}
