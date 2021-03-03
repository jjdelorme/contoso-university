using System.Linq;
using ContosoUniversity.DAL;
using ContosoUniversity.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ContosoUniversity.Controllers
{
    public class HomeController : Controller
    {
        private readonly SchoolContext _db = new SchoolContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            var data = from student in _db.Students
                group student by student.EnrollmentDate
                into dateGroup
                select new EnrollmentDateGroup
                {
                    EnrollmentDate = dateGroup.Key,
                    StudentCount = dateGroup.Count()
                };

            return View(data.ToList());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}