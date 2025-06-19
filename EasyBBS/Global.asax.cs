using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Web.Optimization;
using EasyBBS.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace EasyBBS
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // アプリケーションのスタートアップで実行するコードです
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // ロールと初期管理者ユーザーのセットアップ
            CreateRolesAndUsers();

        }

        private void CreateRolesAndUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            // ロール "Admin" が存在しない場合は作成
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new ApplicationRole();
                role.Name = "Admin";
                roleManager.Create(role);
            }

            // ロール "User" が存在しない場合は作成 (一般的なユーザー用)
            if (!roleManager.RoleExists("User"))
            {
                var role = new ApplicationRole();
                role.Name = "User";
                roleManager.Create(role);
            }

            var adminUser = userManager.FindByName("whiterock0801@yahoo.co.jp"); 
            if (adminUser == null)
            {
                var newAdminUser = new ApplicationUser { UserName = "whiterock0801@yahoo.co.jp", Email = "whiterock0801@yahoo.co.jp" };
                var result = userManager.Create(newAdminUser, "YourPassword123!"); 
                if (result.Succeeded)
                {
                    userManager.AddToRole(newAdminUser.Id, "Admin");
                }
            }
            else
            {
                // ユーザーは存在するがAdminロールを持っていない場合
                if (!userManager.IsInRole(adminUser.Id, "Admin"))
                {
                    userManager.AddToRole(adminUser.Id, "Admin");
                }
            }
        }
    }
}