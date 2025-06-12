using System.EnterpriseServices;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EasyBBS.Models;
using System.Data.Entity;
using System;
using static EasyBBS.Models.ApplicationDbContext;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;


namespace EasyBBS.Controllers
{
    /// <summary>
    /// 掲示板の参照・変更回りのコントローラ
    /// /Board
    /// </summary>
    [Authorize] 
    public class BoardController : Controller
    {
        #region "変数"
        /// <summary>
        /// DbContextのインスタンス
        /// </summary>
        private ApplicationDbContext _db;
        #endregion


        #region "コンストラクタ"
        /// <summary>
        /// DBを指定せずに生成
        /// </summary>
        public BoardController() : this(null)
        {
        }
        /// <summary>
        /// DBを指定して生成
        /// </summary>
        /// <param name="db"></param>
        public BoardController(ApplicationDbContext db)
        {
            _db = db ?? new ApplicationDbContext();
        }
        #endregion



        #region "publicメソッド"

        /// <summary>
        /// GET: Board
        /// 掲示板の一覧
        /// </summary>
        /// <returns>掲示板一覧ビュー</returns>
        public ActionResult Index()
        {
            var model = new BoardListModel(_db);
            return View(model);
        }

        /// <summary>
        /// GET: Board/Show/{ID}
        /// 掲示板の詳細を表示　（選択された掲示板を開く）
        /// </summary>
        /// <param name="id">掲示板のID</param>
        /// <returns></returns>
        public ActionResult Show(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                var board = _db.Boards
                   .Include(b => b.Posts)
                   .SingleOrDefault(o => o.Id == id);

                if (board == null)
                {
                    // 掲示板が見つからない場合は404エラーを返す
                    return HttpNotFound();
                }
                return View(board);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Showアクションでエラー: {ex.Message}");
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                // 汎用エラービューを表示
                return View("Error"); 
            }

        }

        /// <summary>
        /// GET: Board/Create
        /// 掲示板の追加ページ
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST: Board/Create
        /// 掲示板の追加処理
        /// </summary>
        /// <param name="data">投稿内容</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create(BoardCreateModel data)
        {
            if (!ModelState.IsValid) 
            {
                return View(data); 
            }
            try
            {

                var newBoard = new BoardEntity
                {
                    Title = data.Title,
                    Text = data.Text,
                    PostedDate = System.DateTime.Now,
                    UserId = User.Identity.GetUserId() 
                };

                _db.Boards.Add(newBoard);
                await _db.SaveChangesAsync(); 

                return RedirectToAction("Show", new { id = newBoard.Id });
            }
            catch (Exception ex)
            {
                // ログに記録
                System.Diagnostics.Debug.WriteLine($"Createアクションでエラー: {ex.Message}");
                ViewBag.ErrorMessage = "掲示板の追加処理でエラーが発生しました。再度お試しください。";
                return View(data); 
            }

        }

        /// <summary>
        /// POST: Board/PostResponse/{id}
        /// 掲示板に返信を投稿
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PostResponse(int id, BoardPostModel data)
        {
            if (!ModelState.IsValid) 
            {
                return RedirectToAction("Show", new { id = id });
            }
            try
            {
                var board = (from o in _db.Boards where o.Id == id select o).DefaultIfEmpty(null).Single();
                if (board != null)
                {
                    board.Posts.Add(new BoardPostEntity
                    {
                        Text = data.Text,
                        PostedDate = DateTime.Now
                    });
                    _db.SaveChanges();
                }
                return RedirectToAction("Show", new { id = id });
            }        
            catch (Exception ex)
            {
                // ログに記録
                System.Diagnostics.Debug.WriteLine($"PostResponseアクションでエラー: {ex.Message}");
                ViewBag.ErrorMessage = "掲示板の返信投稿でエラーが発生しました。再度お試しください。";
                return RedirectToAction("Show", new { id = id });
            }
        }

        // GET: Board/Delete/5
        // 削除確認画面を表示するアクション
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                // IDが指定されていない場合は、不正なリクエストとして扱う
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                // 指定されたIDの掲示板投稿をデータベースから検索
                BoardEntity board = _db.Boards.Find(id);

                if (board == null)
                {
                    // 投稿が見つからない場合は、NotFound (404) を返す
                    return HttpNotFound();
                }
                // 削除確認画面に投稿の詳細を渡す
                return View(board);
            }  
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Deleteアクションでエラー: {ex.Message}");
                Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                // 汎用エラービューを表示
                return View("Error");
            }
}


        // POST: Board/Delete/5
        // 実際にデータベースから削除を実行
        [HttpPost, ActionName("Delete")] 
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {

                // 指定されたIDの掲示板投稿をデータベースから検索
                BoardEntity board = _db.Boards.Include(b => b.Posts).SingleOrDefault(b => b.Id == id);

                if (board == null)
                {
                    TempData["InfoMessage"] = "指定された掲示板は既に存在しないか、削除済みです。";
                    return RedirectToAction("Index");
                }
                // 権限チェック
                if (board.UserId != User.Identity.GetUserId())
                {
                    TempData["ErrorMessage"] = "他のユーザーの投稿は削除できません。";
                    return RedirectToAction("Index");
                }

                if (board.Posts != null && board.Posts.Any())
                {
                    _db.BoardPostEntities.RemoveRange(board.Posts);

                }

                // 投稿を削除
                _db.Boards.Remove(board);
                // データベースに変更を保存
                _db.SaveChanges();

                // 削除成功後、掲示板一覧ページにリダイレクト
                return RedirectToAction("Index");
            }
                                
            catch (Exception ex)
            {
                // ログに記録
                System.Diagnostics.Debug.WriteLine($"Deleteアクションでエラー: {ex.Message}");
                TempData["ErrorMessage"] = "掲示板の削除中にエラーが発生しました。時間をおいて再度お試しいただくか、管理者にご連絡ください。";
                return RedirectToAction("Index");
            }

        }

        // GET: Board/Edit/5
        /// <summary>
        /// 投稿の編集フォームを表示する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                // IDが指定されていない場合は、不正なリクエストとして扱う
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                // 指定されたIDの掲示板投稿をデータベースから検索
                BoardEntity board = _db.Boards.Find(id);

                if (board == null)
                {
                    // 投稿が見つからない場合は、NotFound (404) を返す
                    return HttpNotFound();
                }
                // 投稿者チェック
                if (board.UserId != User.Identity.GetUserId())
                {
                    // 権限がない場合、アクセス拒否またはエラーページへリダイレクト
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden); // 403 Forbidden
                                                                               // または return RedirectToAction("Index", "Board", new { errorMessage = "他のユーザーの投稿は編集できません。" });
                }
                // 編集フォームに現在の投稿内容を渡す
                return View(board);
            }
            catch (Exception ex)
            {
                // ログに記録
                System.Diagnostics.Debug.WriteLine($"Editアクションでエラー: {ex.Message}");

                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                // 汎用エラービューを表示
                return View("Error"); 
            }
        }

        // POST: Board/Edit/5
        // 編集フォームから送られてきたデータを受け取り、データベースを更新する
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public ActionResult Edit(BoardEntity board) 
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // データベースが更新対象として認識するように指示
                    _db.Entry(board).State = EntityState.Modified;

                    // データベースに変更を保存
                    _db.SaveChanges();

                    // 更新成功後、詳細表示ページ（または一覧ページ）にリダイレクト
                    return RedirectToAction("Show", new { id = board.Id }); 
                                                                        
                }
                // 入力エラーがある場合は、フォームを再表示
                return View(board);
            }
            catch (Exception ex)
            {
                // ログに記録
                System.Diagnostics.Debug.WriteLine($"Editアクションでエラー: {ex.Message}");
                ViewBag.ErrorMessage = "掲示板の編集でエラーが発生しました。再度お試しください。";
                return View(board);
            }
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