using System;
using System.Text;
using System.Text.Json.Serialization;
using GetStat.Api.Domain;
using GetStat.Api.Domain.Abstract;
using GetStat.Api.Domain.Ef;
using GetStat.Api.Hubs;
using GetStat.Domain.Models;
using GetStat.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace GetStat.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            var connection = Configuration.GetConnectionString("DefaultConnection");
            //var connection = Environment.GetEnvironmentVariable("DbContext");
            
            services.AddTransient<EmailService>();
            services.AddScoped<ITestService,TestService>();
            services.AddScoped<GetStatHub>();
            services.AddScoped<CodeHub>();
            services.AddSignalR().AddMessagePackProtocol();
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection));
            services.AddIdentity<Account, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                opts.SignIn.RequireConfirmedEmail = true;
                opts.Password.RequiredLength = 6;
                opts.Password.RequireDigit = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = "JwtBearer";
                opt.DefaultChallengeScheme = "JwtBearer";
            }).AddJwtBearer("JwtBearer", jwtbeaereOpt =>
            {
                
                jwtbeaereOpt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mySuperPass001-*+")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GetStatHub>("/getstat");
                endpoints.MapHub<CodeHub>("/code");
            });
        }
    }
}