using BasicCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasicCRM.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            BasicCRMEntities db = new BasicCRMEntities();
            ViewBag.str = db.Lessons.FirstOrDefault().LessonName;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}