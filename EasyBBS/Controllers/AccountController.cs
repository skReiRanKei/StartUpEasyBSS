using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using EasyBBS.Models;

namespace EasyBBS.Controllers
{
    /// <summary>
    /// アカウントコントロール
    /// </summary>
    [Authorize] // デフォルトで認証が必要であることを示す（個々のアクションで[AllowAnonymous]で上書き可能）
    public class AccountController : Controller
    {

        #region "変数"
        // サインイン管理
        private ApplicationSignInManager _signInManager;
        // ユーザー管理
        private ApplicationUserManager _userManager;
        #endregion

        #region "プロパティ"
        /// <summary>
        /// サインイン管理
        /// </summary>
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        /// <summary>
        /// ユーザー管理
        /// </summary>
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion

        #region "コンストラクタ"
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        #endregion

        #region "publicメソッド"

        /// <summary>
        /// GET: /Account/Login
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous] // ログインページは認証不要
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// POST: /Account/Login
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    //return RedirectToLocal(returnUrl);
                    return RedirectToAction("Index", "Base");

                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "無効なログイン試行です。");
                    return View(model);
            }
        }


        /// <summary>
        /// GET: /Account/Register
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }


        /// <summary>
        /// POST: /Account/Register
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email }; // UserNameとEmailを同じにする
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // ユーザー登録後、どリダイレクト
                    return RedirectToAction("Index", "Board");
                }
                AddErrors(result);
            }

            // モデルの検証に失敗したか、ユーザー作成に失敗した場合に、同じビューを返す
            return View(model);
        }


        /// <summary>
        /// OST: /Account/LogOff
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Board"); // ログアウト後、掲示板一覧へ
        }


        /// <summary>
        /// TGET: /Account/IsEmailAvailable
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost] // Remote属性はPOSTリクエストを送信するため
        [AllowAnonymous]
        public JsonResult IsEmailAvailable(string email)
        {
            // UserManagerを使ってメールアドレスが既に存在するか確認
            var user = UserManager.FindByEmail(email);
            return Json(user == null, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "privateメソッド"
        /// <summary>
        /// ヘルパーメソッド
        /// </summary>
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        /// <summary>
        /// エラー
        /// </summary>
        /// <param name="result"></param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        /// <summary>
        /// リダイレクト
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Board"); // デフォルトのログイン後リダイレクト先
        }
        #endregion














    }
}