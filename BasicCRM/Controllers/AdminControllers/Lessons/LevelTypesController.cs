﻿using System;
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
    public class LevelTypesController : Controller
    {
        private BasicCRMEntities db = new BasicCRMEntities();

        // GET: LevelTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.LevelTypes.ToListAsync());
        }

        // GET: LevelTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LevelType levelType = await db.LevelTypes.FindAsync(id);
            if (levelType == null)
            {
                return HttpNotFound();
            }
            return View(levelType);
        }

        // GET: LevelTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LevelTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LevelTypeID,LevelTypeName")] LevelType levelType)
        {
            if (ModelState.IsValid)
            {
                db.LevelTypes.Add(levelType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(levelType);
        }

        // GET: LevelTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LevelType levelType = await db.LevelTypes.FindAsync(id);
            if (levelType == null)
            {
                return HttpNotFound();
            }
            return View(levelType);
        }

        // POST: LevelTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "LevelTypeID,LevelTypeName")] LevelType levelType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(levelType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(levelType);
        }

        // GET: LevelTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LevelType levelType = await db.LevelTypes.FindAsync(id);
            if (levelType == null)
            {
                return HttpNotFound();
            }
            return View(levelType);
        }

        // POST: LevelTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            LevelType levelType = await db.LevelTypes.FindAsync(id);
            db.LevelTypes.Remove(levelType);
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