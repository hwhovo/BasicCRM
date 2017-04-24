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
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace BasicCRM.Controllers.AdminControllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private BasicCRMEntities db = new BasicCRMEntities();
        private ApplicationDbContext AspDb = ApplicationDbContext.Create();

        #region -- Autentification --
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;


        public UsersController()
        {
        }

        public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion -- Autentification --


        // GET: Users
        public async Task<ActionResult> Index()
        {
            var users = db.Users.Include(u => u.Lesson).Include(u => u.UsersStatus);//.Include(u => u.UsersType);
            Dictionary<string, string> userRoles = new Dictionary<string, string>();
            foreach (var item in users)
            {
                userRoles.Add(item.AspNetUserID, UserManager.GetRoles(item.AspNetUserID).FirstOrDefault());
            }
            ViewBag.userRoles = userRoles;
            return View(await users.ToListAsync());
        }

        //
        // GET: /Users/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Users/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }



        // GET: Users/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            ViewBag.LessonId = new SelectList(db.Lessons, "LessonID", "LessonName");
            ViewBag.UserStatusId = new SelectList(db.UsersStatuses, "UserStatusId", "UserStatusName");

            #region -- Create Roles --
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(ApplicationDbContext.Create()));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ApplicationDbContext.Create()));

            roleManager.Create(new IdentityRole { Name = "Admin" });
            roleManager.Create(new IdentityRole { Name = "Employee" });
            roleManager.Create(new IdentityRole { Name = "Student" });
            #endregion -- Create Roles --



            ViewBag.Roles = new SelectList(AspDb.Roles.Select(item => item.Name).ToList());

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateUserViewModel user)//[Bind(Include = "UserID,FirstName,LastName,Email,Password,UserStatusId,UserTypeId,LessonId,RegistationDate,HireDate,FireDate")] User user)
        {


            if (ModelState.IsValid)
            {
                var authUser = new ApplicationUser { UserName = user.Email, Email = user.Email };
                var result = await UserManager.CreateAsync(authUser, user.Password);
                if (result.Succeeded)
                {
                    string authUserID = authUser.Id;

                    await UserManager.AddToRoleAsync(authUserID, user.UserRoleName);
                    await SignInManager.SignInAsync(authUser, isPersistent: false, rememberBrowser: false);

                    User crmUser = new User()
                    {
                        AspNetUserID = authUserID,
                        Email = user.Email,
                        FireDate = user.FireDate,
                        HireDate = user.HireDate,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        LessonId = user.LessonId,
                        Password = UserManager.PasswordHasher.HashPassword(user.Password),
                    RegistationDate = user.RegistationDate,
                        UserName = user.UserName,
                        UserStatusId = user.UserStatusId
                    };

                    db.Users.Add(crmUser);
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                //AddErrors(result);
            }

            ViewBag.LessonId = new SelectList(db.Lessons, "LessonID", "LessonName", user.LessonId);
            ViewBag.UserStatusId = new SelectList(db.UsersStatuses, "UserStatusId", "UserStatusName", user.UserStatusId);
            ViewBag.Roles = new SelectList(AspDb.Roles.Select(item => item.Name).ToList());

            //    ViewBag.UserTypeId = new SelectList(db.UsersTypes, "UserTypeID", "UserTypeName", user.UserTypeId);
            return View(user);
        }


        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }


            ViewBag.LessonId = new SelectList(db.Lessons, "LessonID", "LessonName", user.LessonId);
            ViewBag.UserStatusId = new SelectList(db.UsersStatuses, "UserStatusId", "UserStatusName", user.UserStatusId);
            ViewBag.Roles = new SelectList(AspDb.Roles.Select(item => item.Name).ToList());
            ViewBag.Code = await UserManager.GeneratePasswordResetTokenAsync(user.AspNetUserID);
            ViewBag.AuthId = user.AspNetUserID;

            //    ViewBag.UserTypeId = new SelectList(db.UsersTypes, "UserTypeID", "UserTypeName", user.UserTypeId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "UserID,FirstName,LastName,Email,Password,UserStatusId,LessonId,RegistationDate,HireDate,FireDate")] User user, string UserRoleName, string Code, string AuthId)
        {
            AuthId = AuthId.Replace("}", "").Replace("{", "").Replace("AuthId", "").Replace("=", "").Trim();
            Code = AuthId.Replace("}", "").Replace("{", "").Replace("Code", "").Replace("=", "").Trim();
            if (ModelState.IsValid)
            {
                user.Password = UserManager.PasswordHasher.HashPassword(user.Password);
                user.AspNetUserID = AuthId;
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //var result = await UserManager.ResetPasswordAsync(AuthId, Code, user.Password);
                var AspUser = AspDb.Users.Find(AuthId);
                AspUser.PasswordHash = user.Password;

                UserManager.RemoveFromRoles(AspUser.Id, AspUser.Roles.Select(item=>item.RoleId).ToArray());
                UserManager.AddToRole(AspUser.Id, UserRoleName);

                AspDb.Entry(AspUser).State = EntityState.Modified;
                AspDb.SaveChanges();

                

                return RedirectToAction("Index");
            }
            ViewBag.LessonId = new SelectList(db.Lessons, "LessonID", "LessonName", user.LessonId);
            ViewBag.UserStatusId = new SelectList(db.UsersStatuses, "UserStatusId", "UserStatusName", user.UserStatusId);
            ViewBag.Roles = new SelectList(AspDb.Roles.Select(item => item.Name).ToList());
            ViewBag.Code = await UserManager.GeneratePasswordResetTokenAsync(AuthId);
            ViewBag.AuthId = AuthId;

            //    ViewBag.UserTypeId = new SelectList(db.UsersTypes, "UserTypeID", "UserTypeName", user.UserTypeId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            User user = await db.Users.FindAsync(id);
            db.Users.Remove(user);
            await db.SaveChangesAsync();

            var AspUser = AspDb.Users.Find(id);
            AspDb.Users.Remove(AspUser);
            await AspDb.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }

                db.Dispose();
                AspDb.Dispose();
            }
            base.Dispose(disposing);
        }

        #region -- Helpers --


#endregion -- Helpers --
    }
}
