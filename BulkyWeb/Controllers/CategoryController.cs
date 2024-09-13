using Bulky.Models;
using Bulky.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using System.Data.SqlTypes;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Category> CategoryList = _context.Categories.ToList();
            return View(CategoryList);
        }

        public IActionResult Create() { 
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            //for the post method, category object is passed as parameter by the framework and it will have the values
            //filled by the user on UI

            //to add custom errors
            //if (category.Name!=null && category.Name.ToLower().Equals("test"))
            //{
            //    ModelState.AddModelError("Name", "test category name is invalid");
            //}

            if (ModelState.IsValid) { 
                _context.Categories.Add(category);
                _context.SaveChanges(); // inserts data into table on execution
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index", "Category"); //redirecting to category list page
            }
            return View();
        }

        public IActionResult Edit(int? id) {
            if (id == null || id == 0)
                return NotFound();

            Category category = _context.Categories.Find(id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj != null)
            {
                _context.Categories.Update(obj);
                _context.SaveChanges();
                TempData["success"] = "Category Updated Successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Delete(int id) {
            if (id == null || id==0)
            {
                return NotFound();
            }
            Category category = _context.Categories.FirstOrDefault(x => x.Id == id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Delete(Category obj)
        {
            if(obj == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(obj);
            _context.SaveChanges();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index", "Category");
        }
    }
}
