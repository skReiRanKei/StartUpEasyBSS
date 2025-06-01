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
            // 掲示板の件数を取得
            int boardCount = db.Boards.Count();

            // ViewBagを使ってビューに件数を渡す
            ViewBag.BoardCount = boardCount;

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