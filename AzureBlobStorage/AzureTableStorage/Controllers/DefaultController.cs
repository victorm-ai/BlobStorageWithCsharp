using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzureTableStorage.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WorkingWithBlobStorage()
        {
            Models.MyBlobStorage ObjectMyBlobStorage = new Models.MyBlobStorage();

            ObjectMyBlobStorage.Create_container();
            ObjectMyBlobStorage.Upload_a_blob_into_a_container();
            ObjectMyBlobStorage.List_the_blobs_in_a_container();
            ObjectMyBlobStorage.Download_blobs_way1();
            ObjectMyBlobStorage.Download_blobs_way2_string();
            //ObjectMyBlobStorage.List_blobs_in_pages_asynchronously();
            ObjectMyBlobStorage.Writing_to_an_append_blob();
            ObjectMyBlobStorage.Delete_blobs();

            return View("DefaultView");
        }
    }
}