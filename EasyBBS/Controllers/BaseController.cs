using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyBBS.Models;

namespace EasyBBS.Controllers
{
    public class BaseController : Controller
    {
        private BoardDbContext db = new BoardDbContext(); // DbContextのインスタンス

        /// <summary>
        /// GET: Base
        /// トップページ
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.BoardCount = db.Boards.Count();
            var latestBoards = db.Boards
                               .OrderByDescending(b => b.PostedDate) // 投稿日時が新しい順
                               .Take(3) // 上位3件
                               .ToList();
            // 取得した最新投稿をViewBagに格納してビューに渡す
            ViewBag.LatestBoards = latestBoards;
            return View();

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose(); // DbContextを破棄する
            }
            base.Dispose(disposing);
        }
    }
}