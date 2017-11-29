using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using identity_demo_for46.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System.Web.Mvc;

namespace identity_demo_for46
{
    /// <summary>
    /// 邮箱安全码服务
    /// </summary>
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // 在此处插入电子邮件服务可发送电子邮件。
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Liwenxue", "liwenxuetest@163.com"));
            emailMessage.To.Add(new MailboxAddress("Find PWD", message.Destination));
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart("plain") { Text = message.Body };

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.163.com", 25, false);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.LocalDomain = "tjgeo.cn";

                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Authenticate("liwenxuetest@163.com", "liwen1012");

                    client.Send(emailMessage);
                    client.Disconnect(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Task.FromResult(0);
        }
    }

    /// <summary>
    /// 手机短信码服务
    /// </summary>
    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // 在此处插入 SMS 服务可发送短信。
            return Task.FromResult(0);
        }
    }

    // 配置此应用程序中使用的应用程序用户管理器。UserManager 在 ASP.NET Identity 中定义，并由此应用程序使用。
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store,
            IdentityFactoryOptions<ApplicationUserManager> options,
            IIdentityMessageService smsService,
            IIdentityMessageService emailService)
            : base(store)
        {
            //邮箱安全码发送服务
            this.EmailService = emailService;
            //短信服务
            this.SmsService = smsService;

            // 配置用户名的验证逻辑
            this.UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // 配置密码的验证逻辑
            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // 配置用户锁定默认值
            this.UserLockoutEnabledByDefault = true;
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromDays(1);
            this.MaxFailedAccessAttemptsBeforeLockout = 5;


            // 注册双重身份验证提供程序。此应用程序使用手机和电子邮件作为接收用于验证用户的代码的一个步骤
            // 你可以编写自己的提供程序并将其插入到此处。
            this.RegisterTwoFactorProvider("电话代码", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "你的安全代码是 {0}"
            });

            this.RegisterTwoFactorProvider("电子邮件代码", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "安全代码",
                BodyFormat = "你的安全代码是 {0}"
            });
            
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                this.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
        }

        public static ApplicationUserManager Create(
            IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            return DependencyResolver.Current.GetService<ApplicationUserManager>();
        }
    }

    // 配置要在此应用程序中使用的应用程序登录管理器。
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager,
           IOwinContext context)
            : base(userManager, context.Authentication)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(
            IdentityFactoryOptions<ApplicationSignInManager> options,
            IOwinContext context)
        {
            return DependencyResolver.Current.GetService<ApplicationSignInManager>();
        }
    }
}
