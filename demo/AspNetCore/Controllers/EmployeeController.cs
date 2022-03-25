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
            return View(new Employee());
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

        // GET: Employee/Create
        public ActionResult CreateWithResx()
        {
            return View(new EmployeeWithResx());
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWithResx(EmployeeWithResx employee)
        {
            if (ModelState.IsValid)
            {
                return View();
            }

            return View(employee);
        }
    }
}