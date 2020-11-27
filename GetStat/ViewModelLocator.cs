using System.DirectoryServices;
using GetStat.ViewModels;
using GetStat.ViewModels.PagesViewModels;
using GetStat.ViewModels.PagesViewModels.Authorization;
using GetStat.ViewModels.PagesViewModels.Tests;

namespace GetStat
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel => Ioc.Resolve<MainViewModel>();
        public SignInViewModel SignInViewModel => Ioc.Resolve<SignInViewModel>();
        public SignUpViewModel SignUpViewModel => Ioc.Resolve<SignUpViewModel>();
        public ConfirmEmailViewModel ConfirmEmailViewModel => Ioc.Resolve<ConfirmEmailViewModel>();
        public MainPageViewModel MainPageViewModel => Ioc.Resolve<MainPageViewModel>();
        public EnterCodePageViewModel EnterCodePageViewModel => Ioc.Resolve<EnterCodePageViewModel>();
        public CreateTestViewModel CreateTestViewModel => Ioc.Resolve<CreateTestViewModel>();
    }
}