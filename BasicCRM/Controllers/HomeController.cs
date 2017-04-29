using BasicCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace BasicCRM.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string AspNetUserID = User.Identity.GetUserId();

            string signedUserRole = userManager.GetRoles(AspNetUserID).FirstOrDefault();

            switch (signedUserRole)
            {
                case "Admin":
                    break;
                case "Employee":
                    break;
                case "Student":
                    return RedirectToAction("StartPage");
                default:
                    break;
            }

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

        [ActionName("StartPage")]
        public ActionResult StudentIndex()
        {
            string AspNetSignedUserId = User.Identity.GetUserId();

            User user;
            using (BasicCRMEntities db = new BasicCRMEntities())
            {
                user = db.Users.Where(item => item.AspNetUserID == AspNetSignedUserId).First();
                int levelId = user.Bunches.First().Level.LevelID;

                ViewBag.TestId = db.Tests.Where(item => item.LevelID == levelId).First().TestID;
            }

            return View(user);
        }
    }
}