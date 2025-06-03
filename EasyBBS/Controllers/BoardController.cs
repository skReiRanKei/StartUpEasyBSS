using System.EnterpriseServices;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EasyBBS.Models;
using System.Data.Entity;


namespace EasyBBS.Controllers
{
    /// <summary>
    /// 掲示板の参照・変更回りのコントローラ
    /// /Board
    /// </summary>
    public class BoardController : Controller
    {
        #region "変数"
        /// <summary>
        /// 掲示板のDB回りの処理
        /// </summary>
        private BoardDbContext _db;
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
        public BoardController(BoardDbContext db)
        {
            _db = db ?? new BoardDbContext();
        }
        #endregion



        #region "Public メソッド (アクションメソッド)"
        /// <summary>
        /// GET: Board
        /// 掲示板の一覧
        /// </summary>
        /// <returns>掲示板一覧ビュー</returns>
        public ActionResult Index()
        {
            // BoardListModel は BoardDbContext を受け取って内部でデータを処理する前
            var model = new BoardListModel(_db);
            return View(model);
        }

        /// <summary>
        /// GET: Board/Show/{ID}
        /// 掲示板の詳細を表示
        /// </summary>
        /// <param name="id">掲示板のID</param>
        /// <returns></returns>
        public ActionResult Show(int id)
        {
            var board = _db.Boards
                           .Include(b => b.Posts) // ここで関連するPostをロード
                           .SingleOrDefault(o => o.Id == id);

            if (board == null)
            {
                // 掲示板が見つからない場合は404エラーを返す
                return HttpNotFound();
            }
            return View(board);

        }

        /// <summary>
        /// GET: Board/Create
        /// 掲示板の投稿ページ
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST: Board/Create
        /// 掲示板の投稿
        /// </summary>
        /// <param name="data">投稿内容</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(BoardCreateModel data)
        {
            var result = _db.Boards.Add(new BoardEntity
            {
                Title = data.Title,
                Text = data.Text,
                PostedDate = System.DateTime.Now,
            });

            _db.SaveChanges();
            return Redirect("/Board/Show/" + result.Id);
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
            var board = (from o in _db.Boards where o.Id == id select o).DefaultIfEmpty(null).Single();
            if (board != null)
            {
                board.Posts.Add(new BoardPostEntity
                {
                    Text = data.Text
                });
                _db.SaveChanges();
            }
            return Redirect("/Board/Show/" + id);
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
        // POST: Board/Delete/5
        // 実際にデータベースから削除を実行するアクション
        [HttpPost, ActionName("Delete")] // POSTリクエストで、かつアクション名をDeleteに指定
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // 指定されたIDの掲示板投稿をデータベースから検索
            BoardEntity board = _db.Boards.Include(b => b.Posts).SingleOrDefault(b => b.Id == id);

            if (board == null)
            {
                // 投稿が見つからない場合は、エラーとせず、単にリダイレクト
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

        // GET: Board/Edit/5
        // 編集フォームを表示するアクション
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                // IDが指定されていない場合は、不正なリクエストとして扱う
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // 指定されたIDの掲示板投稿をデータベースから検索
            BoardEntity board = _db.Boards.Find(id);

            if (board == null)
            {
                // 投稿が見つからない場合は、NotFound (404) を返す
                return HttpNotFound();
            }
            // 編集フォームに現在の投稿内容を渡す
            return View(board);
        }

        // POST: Board/Edit/5
        // 編集フォームから送られてきたデータを受け取り、データベースを更新するアクション
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF対策
        public ActionResult Edit(BoardEntity board) // フォームからのデータが BoardEntity オブジェクトとしてバインドされる
        {
            if (ModelState.IsValid)
            {
                // エンティティの状態を "Modified" に設定し、データベースが更新対象として認識するように指示
                // これにより、boardオブジェクトのすべてのプロパティが更新されます。
                _db.Entry(board).State = EntityState.Modified;

                // データベースに変更を保存
                _db.SaveChanges();

                // 更新成功後、詳細表示ページ（または一覧ページ）にリダイレクト
                return RedirectToAction("Show", new { id = board.Id }); // 更新した掲示板の詳細を表示
                                                                        // あるいは return RedirectToAction("Index"); // 一覧ページにリダイレクト
            }
            // 入力エラーがある場合は、フォームを再表示
            return View(board);
        }
        #endregion


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}