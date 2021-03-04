using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using ContosoUniversity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ContosoUniversity.Controllers
{
    public class InstructorController : Controller
    {
        private readonly SchoolContext _db;

        public InstructorController(SchoolContext db)
        {
             _db = db;
        }

        // GET: Instructor
        public ActionResult Index(int? id, int? courseID)
        {
            var viewModel = new InstructorIndexData();

            viewModel.Instructors = _db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses.Select(c => c.Department))
                .OrderBy(i => i.LastName);

            if (id != null)
            {
                ViewBag.InstructorID = id.Value;
                viewModel.Courses = viewModel.Instructors.Where(
                    i => i.ID == id.Value).Single().Courses;
            }

            if (courseID != null)
            {
                ViewBag.CourseID = courseID.Value;
                // Lazy loading
                //viewModel.Enrollments = viewModel.Courses.Where(
                //    x => x.CourseID == courseID).Single().Enrollments;
                // Explicit loading
                var selectedCourse = viewModel.Courses.Where(x => x.CourseID == courseID).Single();
                _db.Entry(selectedCourse).Collection(x => x.Enrollments).Load();
                foreach (var enrollment in selectedCourse.Enrollments)
                    _db.Entry(enrollment).Reference(x => x.Student).Load();

                viewModel.Enrollments = selectedCourse.Enrollments;
            }

            return View(viewModel);
        }


        // GET: Instructor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return BadRequest();
            var instructor = _db.Instructors.Find(id);
            if (instructor == null) return NotFound();
            return View(instructor);
        }

        public ActionResult Create()
        {
            var instructor = new Instructor();
            instructor.Courses = new List<Course>();
            PopulateAssignedCourseData(instructor);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("LastName,FirstMidName,HireDate,OfficeAssignment")]
            Instructor instructor, string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                instructor.Courses = new List<Course>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = _db.Courses.Find(int.Parse(course));
                    instructor.Courses.Add(courseToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                _db.Instructors.Add(instructor);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }


        // GET: Instructor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return BadRequest();
            var instructor = _db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .Where(i => i.ID == id)
                .Single();
            PopulateAssignedCourseData(instructor);
            if (instructor == null) return NotFound();
            return View(instructor);
        }

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            var allCourses = _db.Courses;
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                });
            ViewBag.Courses = viewModel;
        }

        // POST: Instructor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, string[] selectedCourses)
        {
            if (id == null) return BadRequest();
            var instructorToUpdate = _db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .Where(i => i.ID == id)
                .Single();

            if (await TryUpdateModelAsync(instructorToUpdate))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
                        instructorToUpdate.OfficeAssignment = null;

                    UpdateInstructorCourses(selectedCourses, instructorToUpdate);

                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("",
                        "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            PopulateAssignedCourseData(instructorToUpdate);
            return View(instructorToUpdate);
        }

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            if (selectedCourses == null)
            {
                instructorToUpdate.Courses = new List<Course>();
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>
                (instructorToUpdate.Courses.Select(c => c.CourseID));
            foreach (var course in _db.Courses)
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseID)) instructorToUpdate.Courses.Add(course);
                }
                else
                {
                    if (instructorCourses.Contains(course.CourseID)) instructorToUpdate.Courses.Remove(course);
                }
        }


        // GET: Instructor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return BadRequest();
            var instructor = _db.Instructors.Find(id);
            if (instructor == null) return NotFound();
            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var instructor = _db.Instructors
                .Include(i => i.OfficeAssignment)
                .Where(i => i.ID == id)
                .Single();

            instructor.OfficeAssignment = null;
            _db.Instructors.Remove(instructor);

            var department = _db.Departments
                .Where(d => d.InstructorID == id)
                .SingleOrDefault();
            if (department != null) department.InstructorID = null;

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}