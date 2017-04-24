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
    public class PassportTypesController : Controller
    {
        private BasicCRMEntities db = new BasicCRMEntities();

        // GET: PassportTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.PassportTypes.ToListAsync());
        }

        // GET: PassportTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassportType passportType = await db.PassportTypes.FindAsync(id);
            if (passportType == null)
            {
                return HttpNotFound();
            }
            return View(passportType);
        }

        // GET: PassportTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PassportTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PassportTypeID,PassportTypeName")] PassportType passportType)
        {
            if (ModelState.IsValid)
            {
                db.PassportTypes.Add(passportType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(passportType);
        }

        // GET: PassportTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassportType passportType = await db.PassportTypes.FindAsync(id);
            if (passportType == null)
            {
                return HttpNotFound();
            }
            return View(passportType);
        }

        // POST: PassportTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PassportTypeID,PassportTypeName")] PassportType passportType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(passportType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(passportType);
        }

        // GET: PassportTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassportType passportType = await db.PassportTypes.FindAsync(id);
            if (passportType == null)
            {
                return HttpNotFound();
            }
            return View(passportType);
        }

        // POST: PassportTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PassportType passportType = await db.PassportTypes.FindAsync(id);
            db.PassportTypes.Remove(passportType);
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
