using BulkyWebRazor.Data;
using BulkyWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        [BindProperty] // this indicates that category property is binded to the page and when we do the post, information on the page is binded to this field
        public Category Category { get; set; }

        public CreateModel(ApplicationDbContext context)
        {
                _context = context;
        }
        public void OnGet()
        {
        }

        public IActionResult OnPost() { 
            _context.Categories.Add(Category);
            _context.SaveChanges();
            TempData["success"] = "Category Created Successfully";
            return RedirectToPage("Index");
        }
    }
}
