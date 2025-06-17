using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static EasyBBS.Models.ApplicationDbContext;

namespace EasyBBS.Models
{
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

        /// <summary>
        /// ユーザーID
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// ユーザー
        /// </summary>
        public virtual ApplicationUser User { get; set; } 


        /// <summary>
        /// 返信
        /// </summary>
        public virtual ICollection<BoardPostEntity> Posts { get; set; }
    }
}