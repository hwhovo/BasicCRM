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
    public class UsersStatusController : Controller
    {
        private BasicCRMEntities db = new BasicCRMEntities();

        // GET: UsersStatus
        public async Task<ActionResult> Index()
        {
            return View(await db.UsersStatuses.ToListAsync());
        }

        // GET: UsersStatus/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersStatus usersStatus = await db.UsersStatuses.FindAsync(id);
            if (usersStatus == null)
            {
                return HttpNotFound();
            }
            return View(usersStatus);
        }

        // GET: UsersStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsersStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "UserStatusId,UserStatusName")] UsersStatus usersStatus)
        {
            if (ModelState.IsValid)
            {
                db.UsersStatuses.Add(usersStatus);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(usersStatus);
        }

        // GET: UsersStatus/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersStatus usersStatus = await db.UsersStatuses.FindAsync(id);
            if (usersStatus == null)
            {
                return HttpNotFound();
            }
            return View(usersStatus);
        }

        // POST: UsersStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "UserStatusId,UserStatusName")] UsersStatus usersStatus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usersStatus).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(usersStatus);
        }

        // GET: UsersStatus/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersStatus usersStatus = await db.UsersStatuses.FindAsync(id);
            if (usersStatus == null)
            {
                return HttpNotFound();
            }
            return View(usersStatus);
        }

        // POST: UsersStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UsersStatus usersStatus = await db.UsersStatuses.FindAsync(id);
            db.UsersStatuses.Remove(usersStatus);
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
