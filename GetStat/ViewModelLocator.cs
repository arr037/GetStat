using System.DirectoryServices;
using GetStat.ViewModels;
using GetStat.ViewModels.PagesViewModels;
using GetStat.ViewModels.PagesViewModels.Authorization;
using GetStat.ViewModels.PagesViewModels.Help;
using GetStat.ViewModels.PagesViewModels.Tests;
using GetStat.ViewModels.PagesViewModels.Tests.StartTest;

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
        public MyTestViewModel MyTestViewModel => Ioc.Resolve<MyTestViewModel>();
        public JoinWithCodeViewModel JoinWithCodeViewModel => Ioc.Resolve<JoinWithCodeViewModel>();
        public StartTestViewModel StartTestViewModel => Ioc.Resolve<StartTestViewModel>();
        public GetResultViewModel GetResultViewModel => Ioc.Resolve<GetResultViewModel>();
        public GetResultPageViewModel GetResultPageViewModel => Ioc.Resolve<GetResultPageViewModel>();
        public TeacherResultViewModel TeacherResultViewModel => Ioc.Resolve<TeacherResultViewModel>();
        public HelperPageViewModel HelperPageViewModel => Ioc.Resolve<HelperPageViewModel>();
        public RequestPageViewModel RequestPageViewModel => Ioc.Resolve<RequestPageViewModel>();

    }
}