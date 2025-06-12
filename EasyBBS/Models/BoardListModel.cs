using System.Collections.Generic;
using System.Linq;
using static EasyBBS.Models.ApplicationDbContext;

namespace EasyBBS.Models
{
    /// <summary>
    /// 掲示板の一覧時のモデル
    /// </summary>
    public class BoardListModel
    {
        /// <summary>
        /// 一覧
        /// </summary>
        public List<BoardEntity> Boards { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        public BoardListModel(ApplicationDbContext db)
        {
            Boards = db.Boards.ToList();
        }
    }
}