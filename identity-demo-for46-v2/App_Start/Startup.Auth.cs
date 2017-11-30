using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using identity_demo_for46.Models;
using identity_demo_for46.App_Start;
using System.Web.Mvc;

namespace identity_demo_for46
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            //DI注入
            BootstrapHelper.DIConfig();

            //Autofac中间件
            app.UseAutofacMiddleware(BootstrapHelper.Container);

            //Db上下文
            app.CreatePerOwinContext(() => DependencyResolver.Current.GetService<ApplicationDbContext>());

            //用户管理器
            app.CreatePerOwinContext<ApplicationUserManager>((options, context) =>
            {
                IDependencyResolver current = DependencyResolver.Current;

                var store = current.GetService<IUserStore<ApplicationUser>>();
                var manager = new ApplicationUserManager(store);
                //用户验证器
                manager.UserValidator = new UserValidator<ApplicationUser>(manager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = true
                };

                //密码验证器
                manager.PasswordValidator = current.GetService<IIdentityValidator<string>>();

                //用户锁定默认值
                manager.UserLockoutEnabledByDefault = true;
                manager.DefaultAccountLockoutTimeSpan = BootstrapHelper.TimeSpan;
                manager.MaxFailedAccessAttemptsBeforeLockout = 5;

                //注册双因素登录：短信认证
                manager.RegisterTwoFactorProvider("电话代码", current.GetService<PhoneNumberTokenProvider<ApplicationUser>>());

                //注册双因素登录：邮箱认证
                manager.RegisterTwoFactorProvider("电子邮件代码", current.GetService<EmailTokenProvider<ApplicationUser>>());

                //邮箱安全码发送服务
                manager.EmailService = current.GetService<SmsService>();
                //短信服务
                manager.SmsService = current.GetService<EmailService>();

                var dataProtectionProvider = options.DataProtectionProvider;
                if (dataProtectionProvider != null)
                {
                    manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
                }
                return manager;

            });

            //登录管理器
            app.CreatePerOwinContext<ApplicationSignInManager>((options, context) =>
            {
                var signManager = DependencyResolver.Current.GetService<SignInManager<ApplicationUser, string>>() as ApplicationSignInManager;
                signManager.UserManager = context.GetUserManager<ApplicationUserManager>();
                signManager.AuthenticationManager = context.Authentication;
                return signManager;
            });

            //应用内登录
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator
                        .OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                         BootstrapHelper.TimeSpan, (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });

            //应用外登录
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            //双因素登录
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromDays(1));

            //记住我
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // 取消注释以下行可允许使用第三方登录提供程序登录
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});

            //app.UseWeChatAuthentication(new WeChatAuthenticationOptions() {

            //});

            //app.UseTencentAuthentication(new TencentAuthenticationOptions
            //{
            //    AppId = "1106442119",
            //    AppKey = "BLVew9TMjo9y4bpX"
            //});

        }
    }
}