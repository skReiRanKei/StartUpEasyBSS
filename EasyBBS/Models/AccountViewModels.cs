using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBBS.Models
{

        /// <summary>
        /// 登録画面のモデル
        /// </summary>
        public class RegisterViewModel
        {
            [System.ComponentModel.DataAnnotations.Required]
            [System.ComponentModel.DataAnnotations.EmailAddress]
            [System.Web.Mvc.Remote("IsEmailAvailable", "Account", HttpMethod = "POST", ErrorMessage = "このメールアドレスは既に登録されています。")]
            public string Email { get; set; }

            [System.ComponentModel.DataAnnotations.Required]
            [System.ComponentModel.DataAnnotations.StringLength(100, ErrorMessage = "{0} は、{2} 文字以上である必要があります。", MinimumLength = 6)]
            [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
            public string Password { get; set; }

            [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
            [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "パスワードと確認のパスワードが一致しません。")]
            public string ConfirmPassword { get; set; }
        }
        

        /// <summary>
        /// ログイン画面のモデル
        /// </summary>
        public class LoginViewModel
        {
            [System.ComponentModel.DataAnnotations.Required]
            [System.ComponentModel.DataAnnotations.EmailAddress]
            public string Email { get; set; }

            [System.ComponentModel.DataAnnotations.Required]
            [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
            public string Password { get; set; }

            [System.ComponentModel.DataAnnotations.Display(Name = "このアカウントを記憶する")]
            public bool RememberMe { get; set; }
        }
}
