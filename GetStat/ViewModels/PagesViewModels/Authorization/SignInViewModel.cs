using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using GetStat.Animation;
using GetStat.Commands;
using GetStat.Domain;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Services;
using GetStat.Domain.ViewModels;
using GetStat.Domain.Web;
using GetStat.Helpers;
using GetStat.Models;
using GetStat.Pages.Authorization;
using GetStat.Pages.Main;
using GetStat.Services;
using GetStat.ViewModels.Base;

namespace GetStat.ViewModels.PagesViewModels.Authorization
{
    public class SignInViewModel : BaseVM
    {
        private readonly PageService _pageService;
        private readonly AuthorizationService _authorizationService;
        private readonly ModalService _modalService;
        public  bool IsLogging { get; set; }
        public SignInViewModel(PageService pageService,AuthorizationService authorizationService,ModalService modalService)
        {
            _pageService = pageService;
            _authorizationService = authorizationService;
            _modalService = modalService;
        }


        public string ResetUserName { get; set; }

        public string UserName { get; set; }

        public ICommand SendResetPasswordCommand => new DelegateCommand(async () =>
        {
            if (string.IsNullOrWhiteSpace(ResetUserName))
            {
                _modalService.ShowModalWindow("Ошибка","Введите ваш логин");
                return;
            }

            var uri = Config.UrlAddress + "api/resetpassword";
            var response = await WebRequests.PostAsync<ApiResponse<string>>(uri, ResetUserName);

            var res = response.DisplayErrorIfFailedAsync();
            if (res.SuccessFul == false)
            {
                _modalService.ShowModalWindow("Ошибка при авторизации", res.Message);
                return;
            }

            _modalService.ShowModalWindow("Ответ от сервера",response.ServerResponse.Response);
        });
        public ICommand ResetPasswordCommand => new DelegateCommand<FrameworkElement>((scrol) =>
        {
            var sb = new Storyboard();
            var windowHeidht = App.Current.MainWindow.ActualHeight;
            var slideAnim = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(.5)),
                From = new Thickness(0, windowHeidht, 0, -windowHeidht),
                To = new Thickness(0),
                DecelerationRatio = 0.9f,
            };
            Storyboard.SetTargetProperty(slideAnim,new PropertyPath("Margin"));
            sb.Children.Add(slideAnim);
            sb.AddFadeIn(0.2f);
            sb.Begin(scrol);

            scrol.Visibility = Visibility.Visible;
        });

        public ICommand BackResetWindowCommand => new DelegateCommand<FrameworkElement>((element) =>
        {
            var sb = new Storyboard();
            var windowHeidht = App.Current.MainWindow.ActualHeight;
            var slideAnim = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(.5)),
                To = new Thickness(0, windowHeidht, 0, -windowHeidht),
                DecelerationRatio = 0.9f,
            };
            Storyboard.SetTargetProperty(slideAnim, new PropertyPath("Margin"));
            sb.Children.Add(slideAnim);
            sb.AddFadeOut(0.2f);
            sb.Begin(element);
        });

        public ICommand SignIn => new DelegateCommand<IHavePassword>(async item =>
        {


            //var path = new Uri("push.mp3",UriKind.Relative).ToString();
            //await _mediaPlayerService.Play();

            if (!IsLogging)
            {
                await RunCommandAsync(() => IsLogging, async () =>
                {
                    var model = new LoginViewModel
                    {
                        UserName = UserName,
                        Password = item?.SecureString.Unsecure()
                    };

                    var b = await _authorizationService.Login(model);
                    if (b)
                    {
                        _pageService.Navigate(new MainPage());
                    }

                });
            }




        });


        public ICommand SignUp => new DelegateCommand( () =>
        {
            _pageService.NavigateWithAnimation(new SignUp());
        });

        public ICommand SignInWithCode=> new DelegateCommand(() =>
        {
            _pageService.NavigateWithAnimation(new EnterCodePage());
        });
    }
}