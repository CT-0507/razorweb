using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ex.models;
using Microsoft.AspNetCore.Mvc;
namespace App.Admin.Role
{
    public class RolePageModel : PageModel
    {
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly MyBlogContext _context;

        [TempData]
        public string StatusMessage { get; set; }
        public RolePageModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext)
        {
            _roleManager = roleManager;
            _context = myBlogContext;
        }

    }
}