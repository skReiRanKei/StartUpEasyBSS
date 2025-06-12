using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;



namespace EasyBBS.Models
{
    // ApplicationUser クラスにユーザーのプロファイル データを追加できます。
    // 詳細については、https://go.microsoft.com/fwlink/?LinkID=317594 を参照してください。
    public class ApplicationUser : IdentityUser
    {
        // ここにユーザー固有の追加プロパティを記述できます。
        // 例: public string DisplayName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // authenticationType は CookieAuthenticationOptions.AuthenticationType で定義されているものと一致させる必要があります
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // ここにカスタム ユーザー クレームを追加します
            return userIdentity;
        }
    }

    // 既存の ApplicationDbContext を拡張して Identity の機能を取り込みます。
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<BoardEntity> Boards { get; set; }
        public DbSet<BoardPostEntity> BoardPostEntities { get; set; }

        /// <summary>
        /// DBの掲示板の情報を格納するモデル
        /// </summary>
        public class BoardEntity
        {
            /// <summary>
            /// ID
            /// </summary>
            [Key]
            public int Id { get; set; }

            /// <summary>
            /// タイトル
            /// </summary>
            [Required]
            public string Title { get; set; }

            /// <summary>
            /// 本文
            /// </summary>
            [Required]
            public string Text { get; set; }

            /// <summary>
            /// 投稿時間
            /// </summary>
            [Required]
            public DateTime PostedDate { get; set; }

            public string UserId { get; set; } // ApplicationUser の Id は string 型
            public virtual ApplicationUser User { get; set; } // ナビゲーションプロパティ


            /// <summary>
            /// 返信
            /// </summary>
            public virtual ICollection<BoardPostEntity> Posts { get; set; }
        }



        /// <summary>
        /// DBの掲示板の返信
        /// </summary>
        public class BoardPostEntity
        {
            /// <summary>
            /// ID
            /// </summary>
            [Key]
            public int Id { get; set; }

            /// <summary>
            /// 内容
            /// </summary>
            [Required]
            public string Text { get; set; }

            /// <summary>
            /// 投稿時間
            /// </summary>
            [Required] // 返信日時も必須にする場合
            public DateTime PostedDate { get; set; }

        }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            // Base クラスのコンストラクタで接続文字列名を指定します。
            // "DefaultConnection" は Web.config で定義します。
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        // DbContext が破棄される際に、基底クラスの Dispose も確実に呼び出す
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}