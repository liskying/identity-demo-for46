using Autofac;
using Autofac.Integration.Mvc;
using identity_demo_for46.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;

namespace identity_demo_for46.App_Start
{
    /// <summary>
    /// 启动助手
    /// </summary>
    public class BootstrapHelper
    {
        /// <summary>
        /// IOC依赖容器
        /// </summary>
        public static IContainer Container { get; private set; }

        /// <summary>
        /// 登录过期时间
        /// </summary>
        public static TimeSpan TimeSpan = TimeSpan.FromDays(1);

        /// <summary>
        /// 依赖注入配置
        /// </summary>
        public static void DIConfig()
        {
            var builder = new ContainerBuilder();

            //注册邮箱服务
            builder.RegisterType<EmailService>().AsSelf()
                .AsImplementedInterfaces().InstancePerRequest();

            //注册短信服务
            builder.RegisterType<SmsService>().AsSelf()
                .AsImplementedInterfaces().InstancePerRequest();

            //注册手机号令牌提供程序
            builder.RegisterType<PhoneNumberTokenProvider<ApplicationUser>>().AsSelf()
                .WithProperty("MessageFormat", "您的认证安全码:{0}");

            //注册邮箱令牌提供程序
            builder.RegisterType<EmailTokenProvider<ApplicationUser>>().AsSelf()
                .WithProperty("Subject", "认证安全码")
                .WithProperty("BodyFormat", "您的认证安全码:{0}");

            //注册密码验证器
            builder.RegisterType<PasswordValidator>()
                .As<IIdentityValidator<string>>()
                .AsImplementedInterfaces()
                .WithProperty("RequiredLength", 6)
                .WithProperty("RequireDigit", false)
                .WithProperty("RequireLowercase", false)
                .WithProperty("RequireNonLetterOrDigit", false)
                .WithProperty("RequireUppercase", false);

            //注入Db
            builder.RegisterType<ApplicationDbContext>().AsSelf()
                .As<DbContext>()
                .As<IdentityDbContext<ApplicationUser>>()
                .AsImplementedInterfaces().InstancePerRequest();

            //注册Owin上下文
            builder.RegisterType<OwinContext>().As<IOwinContext>().AsImplementedInterfaces().InstancePerRequest();

            //注入用户管理器
            builder.RegisterType<ApplicationUserManager>().AsSelf().InstancePerRequest();

            ////用户数据接口
            builder.RegisterType<UserStore<ApplicationUser>>().AsSelf().
                As<IUserStore<ApplicationUser>>().InstancePerRequest();

            ////角色数据接口
            //builder.RegisterType<RoleStore<IdentityRole>>().AsSelf().InstancePerRequest();

            //认证管理器
            builder.Register(c => HttpContext.Current.GetOwinContext().Authentication).As<IAuthenticationManager>().AsImplementedInterfaces().InstancePerRequest();

            //角色管理器
            builder.RegisterType<ApplicationSignInManager>().As<SignInManager<ApplicationUser, string>>().AsImplementedInterfaces().InstancePerRequest();

            //注册控制器
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            //注册过滤器
            builder.RegisterFilterProvider();

            //创建容器
            Container = builder.Build();

            //服务定位
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));

        }
    }
}