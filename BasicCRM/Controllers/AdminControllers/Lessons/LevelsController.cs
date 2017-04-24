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
    public class LevelsController : Controller
    {
        private BasicCRMEntities db = new BasicCRMEntities();

        // GET: Levels
        public async Task<ActionResult> Index()
        {
            var levels = db.Levels.Include(l => l.Lesson).Include(l => l.LevelType);
            return View(await levels.ToListAsync());
        }

        // GET: Levels/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Level level = await db.Levels.FindAsync(id);
            if (level == null)
            {
                return HttpNotFound();
            }
            return View(level);
        }

        // GET: Levels/Create
        public ActionResult Create()
        {
            ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "LessonName");
            ViewBag.LevelTypeID = new SelectList(db.LevelTypes, "LevelTypeID", "LevelTypeName");
            return View();
        }

        // POST: Levels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LevelID,LevelName,LessonID,LevelTypeID")] Level level)
        {
            if (ModelState.IsValid)
            {
                db.Levels.Add(level);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "LessonName", level.LessonID);
            ViewBag.LevelTypeID = new SelectList(db.LevelTypes, "LevelTypeID", "LevelTypeName", level.LevelTypeID);
            return View(level);
        }

        // GET: Levels/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Level level = await db.Levels.FindAsync(id);
            if (level == null)
            {
                return HttpNotFound();
            }
            ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "LessonName", level.LessonID);
            ViewBag.LevelTypeID = new SelectList(db.LevelTypes, "LevelTypeID", "LevelTypeName", level.LevelTypeID);
            return View(level);
        }

        // POST: Levels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "LevelID,LevelName,LessonID,LevelTypeID")] Level level)
        {
            if (ModelState.IsValid)
            {
                db.Entry(level).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "LessonName", level.LessonID);
            ViewBag.LevelTypeID = new SelectList(db.LevelTypes, "LevelTypeID", "LevelTypeName", level.LevelTypeID);
            return View(level);
        }

        // GET: Levels/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Level level = await db.Levels.FindAsync(id);
            if (level == null)
            {
                return HttpNotFound();
            }
            return View(level);
        }

        // POST: Levels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Level level = await db.Levels.FindAsync(id);
            db.Levels.Remove(level);
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
