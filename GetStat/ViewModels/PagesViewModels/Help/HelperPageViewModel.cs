using GetStat.Commands;
using GetStat.Pages.Authorization;
using GetStat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GetStat.Annotations;
using GetStat.Domain;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Services;
using GetStat.Domain.Web;
using GetStat.Pages.Main;

namespace GetStat.ViewModels.PagesViewModels.Help
{
    public class HelperPageViewModel : BaseVm
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string LastName { get; set; }


        private readonly PageService pageService;
        private readonly LoginResponseService _loginResponseService;
        private readonly MediaPlayerService _mediaPlayerService;
        private readonly ModalService _modalService;
        public int Volume { get; set; }
        public bool IsAutoPlay { get; set; }
        public HelperPageViewModel(PageService pageService,
            LoginResponseService loginResponseService,
            MediaPlayerService mediaPlayerService,ModalService modalService)
        {
            this.pageService = pageService;
            _loginResponseService = loginResponseService;
            _mediaPlayerService = mediaPlayerService;
            _modalService = modalService;
            IsAutoPlay= _mediaPlayerService.IsAutoPlay;
            Volume=_mediaPlayerService.Volume;

            Name = _loginResponseService.LoginResponse.Name;
            Surname = _loginResponseService.LoginResponse.Surname;
            LastName = _loginResponseService.LoginResponse.MiddleName;
        }


        public ICommand ChangePasswordCommand => new DelegateCommand<Grid>(async (element) =>
        {
            var boxes = FindVisualChildren<PasswordBox>(element).ToList();
            if (boxes.Count > 0)
            {
                var old = boxes[0].Password;
                var newPass = boxes[1].Password;
                var newPassConfirm = boxes[2].Password;

                if (newPass == newPassConfirm)
                {
                    var response = await WebRequests.PostAsync<ApiResponse<bool>>
                    (Config.UrlAddress + "api/changePassword", new string[] { old, newPass },
                        bearerToken: _loginResponseService.LoginResponse?.Token);

                    var res = response.DisplayErrorIfFailedAsync();

                    if (!res.SuccessFul)
                    {
                        _modalService.ShowModalWindow("Ошибка", res.Message);
                        return;
                    }
                    _modalService.ShowModalWindow("Успешно", "Ваш пароль успешно изменен");
                    _loginResponseService.Clear();
                    pageService.NavigateWithAnimation(new SignIn(),PageAnimation.SlideAndFadeInFromTop,PageAnimation.SlideAndFadeOutToBottom);
                }
                else
                {
                    _modalService.ShowModalWindow("Ошибка", "Пароли не совпадают");
                }
            }


            
        });

        public ICommand ChangeFullNameCommand => new DelegateCommand(async () =>
        {
            var response = await WebRequests.PostAsync<ApiResponse<int>>
            (Config.UrlAddress + "api/changeName", new string[] { Surname, Name, LastName },
                bearerToken: _loginResponseService.LoginResponse?.Token);

            var res = response.DisplayErrorIfFailedAsync();

            if (!res.SuccessFul)
            {
                _modalService.ShowModalWindow("Ошибка", res.Message);
                return;
            }

            _modalService.ShowModalWindow("Успешно", "Данные изменены");
            if (_loginResponseService.LoginResponse != null)
            {
                _loginResponseService.LoginResponse.Name = Name;
                _loginResponseService.LoginResponse.Surname = Surname;
                _loginResponseService.LoginResponse.MiddleName = LastName;
            }
        });

        public ICommand CheckPlayCommand => new DelegateCommand(() =>
        {
            _mediaPlayerService.IsAutoPlay = IsAutoPlay;
            _mediaPlayerService.Volume = Volume;
            _mediaPlayerService.Play(true);
        });

        public ICommand SaveCommand => new DelegateCommand(() =>
        {
            GetStatApp.Default.IsAutoPlay = IsAutoPlay;
            GetStatApp.Default.Volume= Volume;
            GetStatApp.Default.Save();

        });

        public ICommand BackCommand => new DelegateCommand(() =>
        {
            if (string.IsNullOrEmpty(_loginResponseService.LoginResponse.Token))
            {
                pageService.NavigateWithAnimation(new EnterCodePage(), PageAnimation.SlideAndFadeInFromTop, PageAnimation.SlideAndFadeOutToBottom);
            }
            else
            {
                pageService.NavigateWithAnimation(new MainPage(), PageAnimation.SlideAndFadeInFromTop, PageAnimation.SlideAndFadeOutToBottom);
            }
        });

        public static IEnumerable<T> FindVisualChildren<T>([NotNull] DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            var queue = new Queue<DependencyObject>(new[] { parent });

            while (queue.Any())
            {
                var reference = queue.Dequeue();
                var count = VisualTreeHelper.GetChildrenCount(reference);

                for (var i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(reference, i);
                    if (child is T children)
                        yield return children;

                    queue.Enqueue(child);
                }
            }
        }
    }
}
