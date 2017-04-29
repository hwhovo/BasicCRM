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
    public class BunchesController : Controller
    {
        private BasicCRMEntities db = new BasicCRMEntities();

        // GET: Bunches
        public async Task<ActionResult> Index()
        {
            var bunches = db.Bunches.Include(b => b.Level).Include(b => b.Room).Include(b => b.User);
            return View(await bunches.ToListAsync());
        }

        // GET: Bunches/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bunch bunch = await db.Bunches.FindAsync(id);
            if (bunch == null)
            {
                return HttpNotFound();
            }
            return View(bunch);
        }

        // GET: Bunches/Create
        public ActionResult Create()
        {
            ViewBag.LevelID = new SelectList(db.Levels, "LevelID", "LevelName");
            ViewBag.RoomID = new SelectList(db.Rooms, "RoomID", "RoomName");
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FirstName");
            return View();
        }

        // POST: Bunches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "BunchID,BunchName,UserID,LevelID,RegistationDate,CompleteDate,RoomID,StartTime,EndTime")] Bunch bunch)
        {
            if (ModelState.IsValid)
            {
                bunch.RegistationDate = bunch.RegistationDate ?? DateTime.Now;

                db.Bunches.Add(bunch);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.LevelID = new SelectList(db.Levels, "LevelID", "LevelName", bunch.LevelID);
            ViewBag.RoomID = new SelectList(db.Rooms, "RoomID", "RoomName", bunch.RoomID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FirstName", bunch.UserID);
            return View(bunch);
        }

        public string GenerateBunchName(string LevelName)
        {
            if (LevelName == null)
            {
                return "";
            }
            if (!db.Levels.Any(item => item.LevelName == LevelName))
            {
                return "";
            }

            var level = db.Levels.Where(item => item.LevelName == LevelName).First();

            string BunchName = level.LevelPrefix + "-" + DateTime.Now.ToString("ddMMyyyy").ToString() + "-";

            //Registered Bunches Count Today
            int count = 1;
            if (db.Bunches.Any(item => item.Level.LevelName == LevelName))
            {
                var levelBunches = db.Bunches.Where(item => item.Level.LevelName == LevelName);

                foreach (var item in levelBunches)
                    if (item.RegistationDate.Value.ToShortDateString() == DateTime.Now.ToShortDateString())
                        count += 1;
            }

            BunchName += count;

            return BunchName;
        }

        // GET: Bunches/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bunch bunch = await db.Bunches.FindAsync(id);
            if (bunch == null)
            {
                return HttpNotFound();
            }
            ViewBag.LevelID = new SelectList(db.Levels, "LevelID", "LevelName", bunch.LevelID);
            ViewBag.RoomID = new SelectList(db.Rooms, "RoomID", "RoomName", bunch.RoomID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FirstName", bunch.UserID);
            return View(bunch);
        }

        // POST: Bunches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "BunchID,BunchName,UserID,LevelID,RegistationDate,CompleteDate,RoomID,StartTime,EndTime")] Bunch bunch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bunch).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.LevelID = new SelectList(db.Levels, "LevelID", "LevelName", bunch.LevelID);
            ViewBag.RoomID = new SelectList(db.Rooms, "RoomID", "RoomName", bunch.RoomID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FirstName", bunch.UserID);
            return View(bunch);
        }

        // GET: Bunches/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bunch bunch = await db.Bunches.FindAsync(id);
            if (bunch == null)
            {
                return HttpNotFound();
            }
            return View(bunch);
        }

        // POST: Bunches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Bunch bunch = await db.Bunches.FindAsync(id);
            db.Bunches.Remove(bunch);
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
