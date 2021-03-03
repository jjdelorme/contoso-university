using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using PagedList;
using Microsoft.AspNetCore.Mvc;

namespace ContosoUniversity.Controllers
{
    public class StudentController : Controller
    {
        private readonly SchoolContext _db = new SchoolContext();

        // GET: Student
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            var students = from s in _db.Students
                select s;
            if (!string.IsNullOrEmpty(searchString))
                students = students.Where(s => s.LastName.Contains(searchString)
                                               || s.FirstMidName.Contains(searchString));
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default: // Name ascending 
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            const int pageSize = 3;
            var pageNumber = page ?? 1;
            return View(students.ToPagedList(pageNumber, pageSize));
        }


        // GET: Student/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new StatusCodeResult(HttpStatusCode.BadRequest);
            var student = _db.Students.Find(id);
            if (student == null) return HttpNotFound();
            return View(student);
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName, FirstMidName, EnrollmentDate")]
            Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Students.Add(student);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("",
                    "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(student);
        }


        // GET: Student/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new StatusCodeResult(HttpStatusCode.BadRequest);
            var student = _db.Students.Find(id);
            if (student == null) return HttpNotFound();
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null) return new StatusCodeResult(HttpStatusCode.BadRequest);
            var studentToUpdate = _db.Students.Find(id);
            if (TryUpdateModel(studentToUpdate, "",
                new[] {"LastName", "FirstMidName", "EnrollmentDate"}))
                try
                {
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("",
                        "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }

            return View(studentToUpdate);
        }

        // GET: Student/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null) return new StatusCodeResult(HttpStatusCode.BadRequest);
            if (saveChangesError.GetValueOrDefault())
                ViewBag.ErrorMessage =
                    "Delete failed. Try again, and if the problem persists see your system administrator.";
            var student = _db.Students.Find(id);
            if (student == null) return HttpNotFound();
            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var student = _db.Students.Find(id);
                _db.Students.Remove(student);
                _db.SaveChanges();
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new {id, saveChangesError = true});
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}