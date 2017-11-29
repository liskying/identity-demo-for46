using Autofac;
using Autofac.Integration.Mvc;
using identity_demo_for46.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace identity_demo_for46.App_Start
{
    public class BootstrapHelper
    {
        public static IContainer Container { get; private set; }

        public static void DIConfig()
        {
            var builder = new ContainerBuilder();

            //邮箱服务注册
            builder.RegisterType<EmailService>().As<IIdentityMessageService>();

            //短信服务注册
            builder.RegisterType<SmsService>().As<IIdentityMessageService>();

            //注入Db
            builder.RegisterType<ApplicationDbContext>().AsSelf();

            //角色管理器
            builder.RegisterType<RoleStore<IdentityRole>>().As<IRoleStore<IdentityRole>>();

            //用户管理器
            builder.RegisterType<UserStore<ApplicationUser>>().As<IUserStore<ApplicationUser>>();

            //注入用户管理器
            builder.RegisterType<ApplicationUserManager>().AsSelf();

            //注入Db
            builder.RegisterType<ApplicationDbContext>().AsSelf();

            Container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));

        }
    }
}