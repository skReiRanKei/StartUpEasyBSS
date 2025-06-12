using Microsoft.AspNet.Identity.EntityFramework;
using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using EasyBBS.Models;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks; // ApplicationDbContext と ApplicationUser のために追加

[assembly: OwinStartup(typeof(EasyBBS.Startup))] // Global.asax の HttpApplication ではなく、OWIN のエントリポイントを指定
namespace EasyBBS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        // Owin 認証の設定
        public void ConfigureAuth(IAppBuilder app)
        {
            // DB コンテキストとユーザー マネージャーを 1 要求につき 1 インスタンス使用するよう構成します。
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // アプリケーションが Cookie を使用してサインインしているユーザーの情報を格納できるようにします。
            // また、サードパーティのログイン プロバイダーを使用してログインしているユーザーの情報を一時的に保存できるものとします。
            // サインイン Cookie の設定
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"), // ログインページのパス
                Provider = new CookieAuthenticationProvider
                {
                    // ユーザーがログインするときにセキュリティ スタンプを検証するロジックをアプリケーションにフックするようにします。
                    // これは、パスワードを変更したり、ソーシャル ログインをアカウントに追加したりするときのセキュリティ機能です。
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30), // 30分ごとに検証
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // アカウントのリンク中に 2 要素認証プロセスを保存するために、2 要素認証 Cookie を有効にします。
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // アプリケーションが、電話またはメールの 2 要素認証コードを記憶できるようにします。
            // これは、ログイン プロセスで 2 番目の検証手順として、デバイスで入力したコードを記憶するために使用されます。
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // ここで外部ログイン プロバイダー (Google、Facebook、Twitter など) をコメント解除できます。
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(
            //    clientId: "",
            //    clientSecret: "");
        }
    }

    // ApplicationUserManager は IdentityUser を管理するためのクラスです。
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));

            // ユーザー名の検証ロジックを構成します。
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false, // 英数字のみを許可するか
                RequireUniqueEmail = true // メールアドレスの一意性を要求するか
            };

            // パスワードの検証ロジックを構成します。
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6, // 最小文字数
                //RequireNonAlphanumeric = true, // 英数字以外の文字を要求するか
                RequireDigit = true, // 数字を要求するか
                RequireLowercase = true, // 小文字を要求するか
                RequireUppercase = true, // 大文字を要求するか
            };

            // ユーザー ロックアウトの既定値を構成します。
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5); // ロックアウト時間
            manager.MaxFailedAccessAttemptsBeforeLockout = 5; // ロックアウトするまでの失敗回数

            // 2 要素認証プロバイダーを登録します。
            // 電話とメールのプロバイダーを登録します。
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService(); // メールサービスの実装があれば
            manager.SmsService = new SmsService();     // SMSサービスの実装があれば

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // ApplicationSignInManager は OWIN コンテキストで使用するサインイン マネージャーを構成します。
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    // ここにメールやSMS送信サービスの実装があれば追加します。
    // デフォルトでは何も送信されません。
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // プラグインする電子メール サービスをここに設定します。
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // プラグインする SMS サービスをここに設定します。
            return Task.FromResult(0);
        }
    }
}