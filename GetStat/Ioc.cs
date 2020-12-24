using System;
using GetStat.Services;
using GetStat.ViewModels;
using GetStat.ViewModels.PagesViewModels;
using GetStat.ViewModels.PagesViewModels.Authorization;
using GetStat.ViewModels.PagesViewModels.Tests;
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

            services.AddScoped<SignInViewModel>();
            services.AddTransient<SignUpViewModel>();
            services.AddTransient<EnterCodePageViewModel>();
            services.AddTransient<ConfirmEmailViewModel>();
            services.AddTransient<AboutUserViewModel>();
            services.AddTransient<MainPageViewModel>();
            services.AddTransient<CreateTestViewModel>();


            services.AddSingleton<PageService>();
            services.AddSingleton<ModalService>();
            services.AddSingleton<AuthorizationService>();
            _provider = services.BuildServiceProvider();
        }

        public static T Resolve<T>()
        {
            return _provider.GetRequiredService<T>();
        }
    }
}