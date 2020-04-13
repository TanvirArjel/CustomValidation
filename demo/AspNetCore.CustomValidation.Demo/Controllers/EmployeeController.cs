using AspNetCore.CustomValidation.Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.CustomValidation.Demo.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                return View();
            }

            return View(employee);
        }
    }
}