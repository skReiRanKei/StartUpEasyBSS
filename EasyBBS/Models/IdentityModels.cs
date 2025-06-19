using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace EasyBBS.Models
{
    /// <summary>
    /// ログインユーザーの情報取得
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// ユーザー身分を識別
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // ユーザー情報
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // カスタム ユーザー クレームを追加
            return userIdentity;
        }
    }

    /// <summary>
    /// ロールを有効にする
    /// </summary>
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }
    }

    /// <summary>
    /// ロール管理クラス
    /// </summary>
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }
        // 作成
        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));
        }
    }

    /// <summary>
    /// 既存の ApplicationDbContext を拡張して Identity の機能を取り込み
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // 掲示板内容
        public DbSet<BoardEntity> Boards { get; set; }
        // 返信の内容
        public DbSet<BoardPostEntity> BoardPostEntities { get; set; }
        // ロール
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

        }
        /// <summary>
        /// 作成
        /// </summary>
        /// <returns></returns>
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


        /// <summary>
        /// 破棄
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}