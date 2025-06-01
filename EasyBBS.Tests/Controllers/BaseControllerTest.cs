using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyBBS.Controllers;
using System.Web.Mvc;


namespace EasyBBS.Tests.Controllers
{
    /// <summary>
    /// トップ画面用テスト
    /// </summary>
    [TestClass]
    public class BaseControllerTest
    {
        /// <summary>
        /// トップ画面
        /// </summary>
        [TestMethod]
        public void Index()
        {
            // エラーが無いかだけチェック
            var controller = new BaseController();
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}