using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppUnderHood.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid) return Page(); 
            //Verify credential
            if(Credential.UserName=="admin" && Credential.Password == "password")
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "admin")
                    , new Claim(ClaimTypes.Email, "admin@mywebsite.com")
                    , new Claim("Department", "HR")
                    , new Claim("Admin", "true")
                    , new Claim("Manager", "true")
                    ,new Claim("EmploymentDate","2022-02-18")
                };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
                var authProperties = new AuthenticationProperties { IsPersistent = Credential.RememberMe };

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal,authProperties);

                return RedirectToPage("/Index");
            }
            return Page();
        }
    }
    public class Credential
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
