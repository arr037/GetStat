using GetStat.ViewModels;
using GetStat.ViewModels.PagesViewModels;
using GetStat.ViewModels.PagesViewModels.Authorization;

namespace GetStat
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel => Ioc.Resolve<MainViewModel>();
        public SignInViewModel SignInViewModel => Ioc.Resolve<SignInViewModel>();
        public SignUpViewModel SignUpViewModel => Ioc.Resolve<SignUpViewModel>();
        public ConfirmEmailViewModel ConfirmEmailViewModel => Ioc.Resolve<ConfirmEmailViewModel>();
        public AboutUserViewModel AboutUserViewModel => Ioc.Resolve<AboutUserViewModel>();
        public MainPageViewModel MainPageViewModel => Ioc.Resolve<MainPageViewModel>();
    }
}