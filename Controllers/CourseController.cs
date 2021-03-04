using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace ContosoUniversity.Controllers
{
    public class CourseController : Controller
    {
        private readonly SchoolContext _db;

        public CourseController(SchoolContext db)
        {
             _db = db;
        }

        // GET: Course
        public ActionResult Index(int? selectedDepartment)
        {
            var departments = _db.Departments.OrderBy(q => q.Name).ToList();
            ViewBag.SelectedDepartment = new SelectList(departments, "DepartmentID", "Name", selectedDepartment);
            var departmentID = selectedDepartment.GetValueOrDefault();

            var courses = _db.Courses
                .Where(c => !selectedDepartment.HasValue || c.DepartmentID == departmentID)
                .OrderBy(d => d.CourseID)
                .Include(d => d.Department);
            var sql = courses.ToString();
            return View(courses.ToList());
        }

        // GET: Course/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return BadRequest();
            var course = _db.Courses.Find(id);
            if (course == null) return NotFound();
            return View(course);
        }


        public ActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("CourseID,Title,Credits,DepartmentID")]
            Course course)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Courses.Add(course);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("",
                    "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return BadRequest();
            var course = _db.Courses.Find(id);
            if (course == null) return NotFound();
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(int? id)
        {
            if (id == null) return BadRequest();
            var courseToUpdate = _db.Courses.Find(id);
            if (await TryUpdateModelAsync(courseToUpdate))
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

            PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in _db.Departments
                orderby d.Name
                select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID", "Name", selectedDepartment);
        }


        // GET: Course/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return BadRequest();
            var course = _db.Courses.Find(id);
            if (course == null) return NotFound();
            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var course = _db.Courses.Find(id);
            _db.Courses.Remove(course);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UpdateCourseCredits()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateCourseCredits(int? multiplier)
        {
            if (multiplier != null)
                ViewBag.RowsAffected =
                    _db.Database.ExecuteSqlCommand("UPDATE Course SET Credits = Credits * {0}", multiplier);
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}