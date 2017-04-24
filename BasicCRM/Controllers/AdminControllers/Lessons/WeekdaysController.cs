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
    public class WeekdaysController : Controller
    {
        private BasicCRMEntities db = new BasicCRMEntities();

        // GET: Weekdays
        public async Task<ActionResult> Index()
        {
            return View(await db.Weekdays.ToListAsync());
        }

        // GET: Weekdays/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Weekday weekday = await db.Weekdays.FindAsync(id);
            if (weekday == null)
            {
                return HttpNotFound();
            }
            return View(weekday);
        }

        // GET: Weekdays/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Weekdays/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "WeekdayID,WeekdayName")] Weekday weekday)
        {
            if (ModelState.IsValid)
            {
                db.Weekdays.Add(weekday);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(weekday);
        }

        // GET: Weekdays/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Weekday weekday = await db.Weekdays.FindAsync(id);
            if (weekday == null)
            {
                return HttpNotFound();
            }
            return View(weekday);
        }

        // POST: Weekdays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "WeekdayID,WeekdayName")] Weekday weekday)
        {
            if (ModelState.IsValid)
            {
                db.Entry(weekday).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(weekday);
        }

        // GET: Weekdays/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Weekday weekday = await db.Weekdays.FindAsync(id);
            if (weekday == null)
            {
                return HttpNotFound();
            }
            return View(weekday);
        }

        // POST: Weekdays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Weekday weekday = await db.Weekdays.FindAsync(id);
            db.Weekdays.Remove(weekday);
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
