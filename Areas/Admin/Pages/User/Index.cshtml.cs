using ex.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;

namespace App.Admin.User
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [TempData]
        public string StatusMessage { set; get; }
        public class UserAndRole : AppUser
        {
            public string RoleNames { set; get; }
        }
        public List<UserAndRole> users { get; set; }

        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]

        public int currentPage { set; get; }
        public int countPages { set; get; }

        public int totalUsers { set; get; }

        public async Task OnGet()
        {
            // users = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            var qr = _userManager.Users.OrderBy(u => u.UserName);

            totalUsers = await qr.CountAsync();
            countPages = (int)Math.Ceiling((double)totalUsers / ITEMS_PER_PAGE);


            if (currentPage < 1)
            {
                currentPage = 1;
            }
            if (currentPage > countPages)
            {
                currentPage = countPages;
            }

            var qr1 = qr.Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).Select(u => new UserAndRole
            {
                Id = u.Id,
                UserName = u.UserName,

            });

            users = await qr1.ToListAsync();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(",", roles);
            }
        }
        public void OnPost() => RedirectToPage();
    }
}
