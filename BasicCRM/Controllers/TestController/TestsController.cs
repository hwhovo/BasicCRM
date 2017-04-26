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
using System.IO;
using BasicCRM.Common;
using Microsoft.AspNet.Identity;

namespace BasicCRM.Controllers
{
    public class TestsController : Controller
    {
        private BasicCRMEntities db = new BasicCRMEntities();

        // GET: Tests
        public async Task<ActionResult> Index()
        {
            var tests = db.Tests.Include(t => t.Level);

            return View(await tests.ToListAsync());
        }

        // GET: Tests/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = await db.Tests.FindAsync(id);

            if (test == null)
            {
                return HttpNotFound();
            }
            ViewBag.Questions = db.Questions.Where(item => item.TestID == test.TestID);

            return View(test);
        }

        // GET: Tests/Create
        public ActionResult Create()
        {
            ViewBag.LevelID = new SelectList(db.Levels, "LevelID", "LevelName");
            return View();
        }

        // POST: Tests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int LevelID, HttpPostedFileBase file)
        {
            Test test = new Test() { LevelID = LevelID };
            string testName = db.Levels.Find(test.LevelID).LevelName + " test " + (db.Tests.Where(item => item.LevelID == test.LevelID).Count() + 1);
            bool isTestAdded = false;

            testName = DbHelper.DbNameValidator(testName);
            test.TestName = testName = testName.Length > 35 ? testName.Substring(0, 35) : testName;

            string _path = "";
            try
            {
                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(file.FileName);
                    string[] avaliableExtension = { ".xls", ".xlsx" };

                    if (avaliableExtension.Where(item => item.Contains(Path.GetExtension(_FileName).ToLower())).Count() <= 0)
                        throw new FormatException();

                    _path = Path.Combine(Server.MapPath("~/App_Data/Uploads"), _FileName);
                    file.SaveAs(_path);
                }




                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Tests.Add(test);
                        await db.SaveChangesAsync();
                        isTestAdded = true;
                    }
                    catch
                    {
                        throw new DuplicateNameException();
                    }
                    try
                    {
                        DbHelper.DbImportFromExcel(test.TestID, _path);
                    }
                    catch
                    {
                        throw new DataException();
                    }

                    ViewBag.Message = "File Uploaded Successfully!!";

                    return RedirectToAction("Index");
                }

                return View();
            }
            catch (FormatException)
            {
                ViewBag.Message = "File format wrong!! Try again upload only Excel Files with extension .xls or .xlsx!";
            }
            catch (DuplicateNameException)
            {
                ViewBag.Message = "File name wrong!! Please rename file using numbers, letters and rename file shorter length!";
            }
            catch (DataException)
            {
                ViewBag.Message = "File content can't parse to the database! Please check file content!";
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
            }
            if (isTestAdded)
                db.Tests.Remove(test);

            ViewBag.LevelID = new SelectList(db.Levels, "LevelID", "LevelName", test.LevelID);
            return View(test);

        }

        // GET: Tests/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = await db.Tests.FindAsync(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            ViewBag.LevelID = new SelectList(db.Levels, "LevelID", "LevelName", test.LevelID);
            return View(test);
        }

        // POST: Tests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "TestID,LevelID,TestName")] Test test)
        {
            if (ModelState.IsValid)
            {
                db.Entry(test).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.LevelID = new SelectList(db.Levels, "LevelID", "LevelName", test.LevelID);
            return View(test);
        }

        // GET: Tests/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = await db.Tests.FindAsync(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }

        // POST: Tests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Test test = await db.Tests.FindAsync(id);
            var questions = db.Questions.Where(item => item.TestID == test.TestID);
            var Answers = db.Answers.Where(item => questions.Select(it => it.QuestionID).Contains(item.QuestionID ?? -1)).ToList();
            db.Answers.RemoveRange(Answers);
            db.SaveChanges();

            db.Questions.RemoveRange(questions);
            db.SaveChanges();

            db.Tests.Remove(test);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Start(int? TestId)
        {
            if (TestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = await db.Tests.FindAsync(TestId);

            if (test == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = "Test " + test.Level.LevelName;

            ViewBag.Questions = db.Questions.Where(item => item.TestID == test.TestID);

            return View(test);
        }


        [HttpPost]
        public async Task<ActionResult> Start(IEnumerable<int> checkedItemsIds, int? currentTestId)
        {
            if (currentTestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Test test = await db.Tests.FindAsync(currentTestId);

            if (test == null)
            {
                return HttpNotFound();
            }

            var questionsList = db.Questions.Where(item => item.TestID == currentTestId).Include(item => item.Answers);


            #region -- calculating user score for test --
            decimal userScore = 0;
            decimal questionScore = 0;

            foreach (Question question in questionsList)
            {
                questionScore = 0;
                foreach (Answer answer in question.Answers)
                {
                    if ((answer.AnswerPoint == null || answer.AnswerPoint == 0)
                        && checkedItemsIds.Contains(answer.AnswerID))//if checked wrong answer then question score = 0
                    {
                        questionScore = 0;
                        break;
                    }
                    else
                        questionScore += answer.AnswerPoint ?? 0;
                }
                userScore += questionScore;
            }
            #endregion  -- calculating user score for test --


            #region -- adding testArchive to the database --
            string AspNetSignedUserId = User.Identity.GetUserId();
            TestsArchive testArchive = new TestsArchive()
            {
                TestId = currentTestId,
                UserId = db.Users.Where(item => item.AspNetUserID == AspNetSignedUserId).First().UserID,
                UserScore = userScore,
                RegDate = DateTime.Now
            };

            db.TestsArchives.Add(testArchive);

            await db.SaveChangesAsync();
            #endregion -- adding testArchive to the database --


            #region -- adding answersArchives to the database --
            List<AnswerArchive> answersArchives = new List<AnswerArchive>();

            foreach (Question question in questionsList)
                foreach (Answer answer in question.Answers)
                    answersArchives.Add(new AnswerArchive()
                    {
                        AnswerId = answer.AnswerID,
                        IsChecked = checkedItemsIds.Contains(answer.AnswerID),
                        TestsArchiveId = testArchive.TestsArchiveId,
                    });

            db.AnswerArchives.AddRange(answersArchives);
            await db.SaveChangesAsync();
            #endregion -- adding answersArchives to the database --


            return Json( new{ TestsArchiveId = testArchive.TestsArchiveId });
        }

        public async Task<ActionResult> Complete(int? TestsArchiveId)
        {
            if (TestsArchiveId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TestsArchive testArchive = await db.TestsArchives.FindAsync(TestsArchiveId);

            //ViewBag.Score = Score;
            //ViewBag.TestId = TestId;
            //string AspNetSignedUserId = User.Identity.GetUserId();
            //int userId = db.Users.Where(it => it.AspNetUserID == AspNetSignedUserId).First().UserID;

            //ViewBag.TestArchiveId = db.TestsArchives.Where(item => item.TestId == TestId
            //&& item.UserId == userId).First().TestsArchiveId;

            return View(testArchive);
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
