using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

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
    /// 既存の ApplicationDbContext を拡張して Identity の機能を取り込み
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // 掲示板内容
        public DbSet<BoardEntity> Boards { get; set; }
        // 返信の内容
        public DbSet<BoardPostEntity> BoardPostEntities { get; set; }


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