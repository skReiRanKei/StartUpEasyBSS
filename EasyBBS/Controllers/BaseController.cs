using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EasyBBS.Models;

namespace EasyBBS.Controllers
{
    /// <summary>
    /// ベースコントロール
    /// </summary>
    public class BaseController : Controller
    {
        #region "定数"
        // 初期表示させる件数　（任意で変えられるように定数化）
        private const int TOP_VIEW_CNT = 3;
        #endregion

        #region "変数"
        // DbContextのインスタンス
        private BoardDbContext _db;
        #endregion

        #region "コンストラクタ"
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BaseController() 
        {
            // 初期化
            _db = new BoardDbContext(); 
        }
        #endregion

        #region "publicメソッド"
        /// <summary>
        /// GET: Base
        /// トップページ
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                // Viewの件数
                ViewBag.BoardCount = _db.Boards.Count();

                // 投稿日時が新しい上位を取得
                var latestBoards = _db.Boards
                                   .OrderByDescending(b => b.PostedDate)
                                   .Take(TOP_VIEW_CNT)
                                   .ToList();

                // 取得した最新投稿をViewBagに格納してビューに渡す
                ViewBag.LatestBoards = latestBoards;

                
            }
            catch (Exception ex)
            {
                // 必要に応じてログ出力
                System.Diagnostics.Debug.WriteLine($"エラー発生: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"スタックトレース: {ex.StackTrace}");

                ViewBag.ErrorMessage = "現在、掲示板情報を表示できません。時間をおいて再度お試しください。";

                // エラー時は0件として扱う
                ViewBag.BoardCount = 0;
                // 空のリストを渡す
                ViewBag.LatestBoards = new List<BoardEntity>(); 
            }
            return View();

        }
        #endregion

        #region "privateメソッド"
        
        /// <summary>
        /// 破棄
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose(); 
            }
            base.Dispose(disposing);
        }
        #endregion


    }
}