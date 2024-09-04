using BulkyWebRazor.Data;
using BulkyWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        [BindProperty]
        public Category Category { get; set; }

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public void OnGet(int id)
        {
            if (id != 0 && id != null)
                Category = _context.Categories.Find(id);
        }

        public IActionResult OnPost()
        {
            Category obj = _context.Categories.Find(Category.Id);
            if (obj!=null)
            {
                _context.Categories.Remove(obj);
                _context.SaveChanges();
                TempData["success"] = "Category Deleted Successfully";
                return RedirectToPage("Index");
            }

            return Page();

        }
    }
}
