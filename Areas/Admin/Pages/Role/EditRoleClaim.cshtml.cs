using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ex.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Admin.Role
{
    [Authorize(Roles = "Admin")]
    public class EditRoleClaim : RolePageModel
    {
        public EditRoleClaim(RoleManager<IdentityRole> roleManager, AppDbContext myBlogContext) : base(roleManager, myBlogContext)
        {

        }

        public class InputModel
        {
            [Display(Name = "Kiểu (tên) claim")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1}")]
            public string ClaimType { set; get; }
            [Display(Name = "Giá trị")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1}")]
            public string ClaimValue { set; get; }
        }
        [BindProperty]
        public InputModel Input { set; get; }

        public IdentityRole role { set; get; }

        IdentityRoleClaim<string> claim { set; get; }
        public async Task<IActionResult> OnGet(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy role");

            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(claim.RoleId);

            if (role == null) return NotFound("Không tìm thấy role");

            Input = new InputModel()
            {
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? claimid)
        {

            if (claimid == null) return NotFound("Không tìm thấy role");

            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(claim.RoleId);

            if (role == null) return NotFound("Không tìm thấy role");
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (_context.RoleClaims.Any(c => c.RoleId == role.Id && c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id != claim.Id))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có trong role");
                return Page();
            }

            claim.ClaimType = Input.ClaimType;
            claim.ClaimValue = Input.ClaimValue;
            await _context.SaveChangesAsync();
            StatusMessage = "Vừa cập nhật claim";

            return RedirectToPage("./Edit", new { roleid = role.Id });
        }
        public async Task<IActionResult> OnPostDeleteAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy role");

            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(claim.RoleId);

            if (role == null) return NotFound("Không tìm thấy role");

            var result = await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));
            if (!result.Succeeded)
            {
                return NotFound("Không tìm thấy role");
            }

            StatusMessage = "Vừa xóa claim";

            return RedirectToPage("./Edit", new { roleid = role.Id });
        }
    }
}
