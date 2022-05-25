using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PUSGS_Project.Models;
using ServiceLayer.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using DataAcesss.Data;
using ServiceLayer;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ServiceLayer.Helpers;

namespace PUSGS_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IUserService userService,
            UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IWebHostEnvironment env,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Join()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {

            }
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser();
                user.UserName = model.Username;
                user.Email = model.Email;
                user.Password = model.Password;
                user.Fullname = model.Fullname;
                user.Address = model.Address;
                user.Birthdate = model.Birthdate;
                user.ImagePath = model.ImagePath;

                // Create user
                var result = await _userManager.CreateAsync(user, user.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);

                    // Make SignUpRequest for Administrator to verify
                    // await _userManager.AddToRoleAsync(user, model.Role);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("",error.Description);
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public void UploadImage()
        {
            if (Request.Form.Files.Count > 0)
            {
                var imageFile = Request.Form.Files.First();

                var fullPath = Path.Combine(_env.ContentRootPath, "wwwroot\\images", imageFile.FileName);

                using (FileStream fs = System.IO.File.Create(fullPath))
                {
                    imageFile.CopyTo(fs);
                    fs.Flush();
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region AddRoles
        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRoles()
        {
            // Create role
            IdentityRole role = new IdentityRole
            {
                Name = Request.Form["role"]
            };

            await _roleManager.CreateAsync(role);

            return View("AddRole");
        }
        #endregion
    }
}
