using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Domain.Services;
using GetStat.Services;
using GetStat.ViewModels;
using GetStat.ViewModels.PagesViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace GetStat
{
    public static class Ioc
    {
        private static readonly IServiceProvider _provider;

        static Ioc()
        {
            var services = new ServiceCollection();
            services.AddSingleton<MainViewModel>();

            services.AddTransient<SignInViewModel>();
            services.AddTransient<SignUpViewModel>();
            services.AddTransient<ConfirmEmailViewModel>();



            services.AddTransient<EmailService>();
            services.AddSingleton<PageService>();
            services.AddSingleton<ModalService>();
            services.AddSingleton<AuthorizationService>();
            _provider = services.BuildServiceProvider();
        }

        public static T Resolve<T>() => _provider.GetRequiredService<T>();
    }
}
