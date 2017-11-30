using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using identity_demo_for46.Models;
using MailKit.Net.Smtp;
using MimeKit;

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
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {

        }
    }

    // 配置要在此应用程序中使用的应用程序登录管理器。
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager() : base(null, null) { }

        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }
    }
}
