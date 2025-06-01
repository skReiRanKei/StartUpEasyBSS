﻿using System.Collections.Generic;
using System.Linq;

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
        public BoardListModel(BoardDbContext db)
        {
            Boards = db.Boards.ToList();
        }
    }
}