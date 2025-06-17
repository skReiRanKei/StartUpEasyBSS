using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyBBS.Models
{
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
        [Required] 
        public DateTime PostedDate { get; set; }

    }
}