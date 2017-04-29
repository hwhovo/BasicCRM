using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BasicCRM.Models;
using Microsoft.AspNet.Identity;

namespace BasicCRM.Controllers.TestController
{
    public class TestsArchivesController : Controller
    {
        private BasicCRMEntities db = new BasicCRMEntities();

        // GET: TestsArchives
        public async Task<ActionResult> Index()
        {
            var testsArchives = db.TestsArchives.Include(t => t.Test).Include(t => t.User);
            return View(await testsArchives.ToListAsync());
        }

        // GET: TestsArchives/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestsArchive testsArchive = await db.TestsArchives.FindAsync(id);
            if (testsArchive == null)
            {
                return HttpNotFound();
            }
            return View(testsArchive);
        }

        // GET: TestsArchives/Create
        public ActionResult Create()
        {
            ViewBag.TestId = new SelectList(db.Tests, "TestID", "TestName");
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FirstName");
            return View();
        }

        // POST: TestsArchives/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TestsArchiveId,TestId,UserId,RegDate,UserScore")] TestsArchive testsArchive)
        {
            if (ModelState.IsValid)
            {
                db.TestsArchives.Add(testsArchive);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.TestId = new SelectList(db.Tests, "TestID", "TestName", testsArchive.TestId);
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FirstName", testsArchive.UserId);
            return View(testsArchive);
        }

        // GET: TestsArchives/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestsArchive testsArchive = await db.TestsArchives.FindAsync(id);
            if (testsArchive == null)
            {
                return HttpNotFound();
            }
            ViewBag.TestId = new SelectList(db.Tests, "TestID", "TestName", testsArchive.TestId);
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FirstName", testsArchive.UserId);
            return View(testsArchive);
        }

        // POST: TestsArchives/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "TestsArchiveId,TestId,UserId,RegDate,UserScore")] TestsArchive testsArchive)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testsArchive).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.TestId = new SelectList(db.Tests, "TestID", "TestName", testsArchive.TestId);
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FirstName", testsArchive.UserId);
            return View(testsArchive);
        }

        // GET: TestsArchives/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestsArchive testsArchive = await db.TestsArchives.FindAsync(id);
            if (testsArchive == null)
            {
                return HttpNotFound();
            }

            db.AnswerArchives.RemoveRange(db.AnswerArchives.Where(item => item.TestsArchiveId == id));
            await db.SaveChangesAsync();

            db.TestsArchives.Remove(testsArchive);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        //// POST: TestsArchives/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{

        //}
        [ActionName("Tests")]
        public async Task<ActionResult> LoadUserTests()
        {
            string AspNetSignedUserId = User.Identity.GetUserId();
            var user = db.Users.Where(item => item.AspNetUserID == AspNetSignedUserId).First();

            var testsArchives = db.TestsArchives.Where(item=>item.UserId == user.UserID).Include(t => t.Test).Include(t => t.User);
            return View(await testsArchives.ToListAsync());
        }

        //Load Old tested for checking
        public async Task<ActionResult> Load(int? TestsArchiveId)
        {
            if (TestsArchiveId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var testsArchive = await db.TestsArchives.FindAsync(TestsArchiveId);

            if (testsArchive == null || testsArchive.AnswerArchives == null || testsArchive.AnswerArchives.Count == 0)
            {
                return HttpNotFound();
            }

            return View(testsArchive);

            //return RedirectToActionPermanent("Details", new { id = TestsArchiveId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
