using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBBS.Models
{
    /// <summary>
    /// 投稿内容
    /// </summary>
    public class BoardCreateModel
    {
        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Text { get; set; }
    }
}