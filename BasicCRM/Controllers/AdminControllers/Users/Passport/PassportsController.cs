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

namespace BasicCRM.Controllers.AdminControllers
{
    public class PassportsController : Controller
    {
        private BasicCRMEntities db = new BasicCRMEntities();

        // GET: Passports
        public async Task<ActionResult> Index()
        {
            var passports = db.Passports.Include(p => p.PassportType).Include(p => p.User);
            return View(await passports.ToListAsync());
        }

        // GET: Passports/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Passport passport = await db.Passports.FindAsync(id);
            if (passport == null)
            {
                return HttpNotFound();
            }
            return View(passport);
        }

        // GET: Passports/Create
        public ActionResult Create()
        {
            ViewBag.PassportTypeID = new SelectList(db.PassportTypes, "PassportTypeID", "PassportTypeName");
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FirstName");
            return View();
        }

        // POST: Passports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PassportID,Number,Authorithy,DateOfIssue,PassportTypeID,UserID")] Passport passport)
        {
            if (ModelState.IsValid)
            {
                db.Passports.Add(passport);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PassportTypeID = new SelectList(db.PassportTypes, "PassportTypeID", "PassportTypeName", passport.PassportTypeID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FirstName", passport.UserID);
            return View(passport);
        }

        // GET: Passports/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Passport passport = await db.Passports.FindAsync(id);
            if (passport == null)
            {
                return HttpNotFound();
            }
            ViewBag.PassportTypeID = new SelectList(db.PassportTypes, "PassportTypeID", "PassportTypeName", passport.PassportTypeID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FirstName", passport.UserID);
            return View(passport);
        }

        // POST: Passports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PassportID,Number,Authorithy,DateOfIssue,PassportTypeID,UserID")] Passport passport)
        {
            if (ModelState.IsValid)
            {
                db.Entry(passport).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PassportTypeID = new SelectList(db.PassportTypes, "PassportTypeID", "PassportTypeName", passport.PassportTypeID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FirstName", passport.UserID);
            return View(passport);
        }

        // GET: Passports/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Passport passport = await db.Passports.FindAsync(id);
            if (passport == null)
            {
                return HttpNotFound();
            }
            return View(passport);
        }

        // POST: Passports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Passport passport = await db.Passports.FindAsync(id);
            db.Passports.Remove(passport);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
