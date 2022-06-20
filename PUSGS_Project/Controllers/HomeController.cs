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
using DataAcesss.Repos.Contracts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace PUSGS_Project.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class HomeController : Controller
    {
        private static object takeOrderLocker = new object();
        private static object createProductLocker = new object();
        private static object verifyLocker = new object();

        private readonly ILogger<HomeController> _logger;

        #region Services
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly ISignUpRequestService _signUpRequestService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IProductOrderService _productOrderService;
        #endregion

        #region Managers
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        #endregion

        private readonly IWebHostEnvironment _env;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IMapper _mapper;
        
        public HomeController(ILogger<HomeController> logger, IUserService userService,
            UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IWebHostEnvironment env,
            RoleManager<IdentityRole> roleManager, ISignUpRequestService signUpRequestService,
            IEmailService emailService, IHttpContextAccessor httpContextAccessor, IMapper mapper,
            IProductService productService, IOrderService orderService, IProductOrderService productOrderService)
        {
            _logger = logger;
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
            _roleManager = roleManager;
            _signUpRequestService = signUpRequestService;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _productService = productService;
            _orderService = orderService;
            _productOrderService = productOrderService;
        }

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                return RedirectToAction("Profile");
            }
            return RedirectToAction("SignIn");
        }

        #region SignInSignUp
        public IActionResult Join()
        {
            return View();
        }

        [HttpGet]
        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn", "Home");
        }

        [HttpPost]
        public IActionResult ExternalSignUp(string returnUrl)
        {
            try
            {
                var redirectUrl = Url.Action("ExternalSignUpCallback", "Home", new { ReturnUrl = returnUrl });

                var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

                return new ChallengeResult("Google", properties);
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message});
            }
            
        }

        public async Task<IActionResult> ExternalSignUpCallback(string returnUrl = null, string remoteError = null)
        {
            try
            {
                SignUpViewModel model = new SignUpViewModel()
                {
                    ReturnUrl = returnUrl,
                    ExternalSignUp = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
                };

                if (remoteError != null)
                {
                    ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                    return View("SignUp", model);
                }

                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    ModelState.AddModelError(string.Empty, "Error loading external information.");
                    return View("SignUp", model);
                }

                var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, true);
                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                    if (email != null)
                    {
                        
                        var user = await _userManager.FindByEmailAsync(email);

                        if (user == null)
                        {
                            // Create unique username based on unique email
                            MD5 md5 = new MD5CryptoServiceProvider();
                            byte[] digest = md5.ComputeHash(Encoding.UTF8.GetBytes(email));
                            string base64digest = Convert.ToBase64String(digest, 0, digest.Length);
                            var username = base64digest.Substring(0, base64digest.Length - 2);

                            // Create password
                            var password = "Password!123";

                            user = new AppUser()
                            {
                                UserName = username,
                                Email = email,
                                Password = password,
                                Birthdate = DateTime.Now.AddYears(-18).AddDays(-1),
                            };

                            await _userManager.CreateAsync(user);
                            await _userManager.AddToRoleAsync(user, "CONSUMER");
                        }

                        await _userManager.AddLoginAsync(user, info);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                            
                        return RedirectToAction("Index");
                    }

                    ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}.";
                    ViewBag.ErrorMessage = "Please contact administrators.";

                    return View("MyError", new ErrorViewModel() { Message = ViewBag.ErrorTitle + ViewBag.ErrorMessage });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message});
            }
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            try
            {
                if (_signInManager.IsSignedIn(HttpContext.User))
                {
                    return RedirectToAction("Profile");
                }
                return View();
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "No user with this username exists");
                        return View(model);
                    }
                    if (user.Password == model.Password)
                    {
                        await _signInManager.SignInAsync(user, false);

                        if (await _userManager.IsInRoleAsync(user, "TEMP_DELIVERER"))
                        {
                            return RedirectToAction("Profile");
                        }
                        return RedirectToAction("Index");
                    }

                    ModelState.AddModelError("", "Invalid password");
                }
                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SignUp()
        {
            try
            {
                if (_signInManager.IsSignedIn(HttpContext.User))
                {
                    return View("Index");
                }

                SignUpViewModel model = new SignUpViewModel()
                {
                    ExternalSignUp = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
                };

                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var isValid = true;

                    // Username uniqueness check
                    if (_userManager.Users.FirstOrDefault(x => x.UserName == model.Username) != null)
                    {
                        ModelState.AddModelError("", "That username is already in use");
                        isValid = false;
                    }

                    // Email uniqueness check
                    //if (_userManager.Users.FirstOrDefault(x => x.Email == model.Email) != null)
                    //{
                    //    ModelState.AddModelError("", "That email address is already in use");
                    //    isValid = false;
                    //}

                    if (!isValid)
                    {
                        if (model.ImagePath != null && !model.ImagePath.Contains("/user.jpg"))
                        {
                            // Delete uploaded user image
                            System.IO.File.Delete(model.ImagePath);
                        }
                        return View();
                    }

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
                        // Add user to role
                        if (model.Role == "CONSUMER")
                        {
                            await _userManager.AddToRoleAsync(user, model.Role);
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, "TEMP_DELIVERER");
                        }

                        await _signInManager.SignInAsync(user, false);

                        if (model.Role == "DELIVERER")
                        {
                            var newUser = _userManager.Users.FirstOrDefault(x => x.UserName == user.UserName);
                            if (newUser != null)
                            {
                                // Make sign up request
                                _signUpRequestService.CreateRequest(newUser.Id);
                            }

                            // Redirect to profile
                            return RedirectToAction("Profile");
                        }
                        else
                        {
                            // Send email for successfull registration 
                            await _emailService.SendEmailAsync(user.Email, "Liman Restaurant - Registration", "<p>Successful registration!</p>");
                            return RedirectToAction("CurrentOrders");
                        }
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }

                return View();
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }
        #endregion

        #region Profile
        [Authorize(Roles = "ADMINISTRATOR, DELIVERER, CONSUMER, TEMP_DELIVERER")]
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            try
            {
                ViewBag.DelivererPreview = false;

                // Find the signed in user
                var user = await _userManager.GetUserAsync(HttpContext.User);

                // Set status
                ViewBag.Status = _signUpRequestService.GetLatestRequestStatus(user.Id);

                if (ModelState.IsValid)
                {
                    // If user and model have same values no need to write in DB
                    if (Helpers.Helpers.ComapareUserAndProfileModel(user, model))
                        return View(model);

                    // Check for uniqueness of username and email
                    if (model.Username != user.UserName)
                    {
                        var userWithSameUsername = await _userManager.FindByNameAsync(model.Username);
                        if (userWithSameUsername != null)
                        {
                            var errorMessage = $"A user with this username ('{userWithSameUsername.UserName}') already exists";
                            ModelState.AddModelError("", errorMessage);
                            Helpers.Helpers.UserToProfileModel(user, model);
                            return View(model);
                        }
                    }

                    if (model.Email != user.Email)
                    {
                        var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);
                        if (userWithSameEmail != null)
                        {
                            var errorMessage = $"A user with this email address ('{userWithSameEmail.Email}') already exists";
                            ModelState.AddModelError("", errorMessage);
                            Helpers.Helpers.UserToProfileModel(user, model);
                            return View(model);
                        }
                    }

                    // Assign new values to signed in user
                    Helpers.Helpers.ProfileModelToUser(model, user);

                    // Update signed in user 
                    var result = await _userManager.UpdateAsync(user);

                    // Display errors if updating wasn't successfull
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        Helpers.Helpers.UserToProfileModel(user, model);
                        return View(model);
                    }
                }

                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }

        [Authorize(Roles = "ADMINISTRATOR, DELIVERER, CONSUMER, TEMP_DELIVERER")]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                ViewBag.DelivererPreview = false;

                var user = await _userManager.GetUserAsync(HttpContext.User);
                var model = _mapper.Map<AppUser, ProfileViewModel>(user);
                model.ConfirmPassword = model.Password;
                if (await _userManager.IsInRoleAsync(user, "ADMINISTRATOR"))
                {
                    model.Role = "ADMINISTRATOR";
                }
                else if (await _userManager.IsInRoleAsync(user, "DELIVERER"))
                {
                    model.Role = "DELIVERER";
                }
                else if (await _userManager.IsInRoleAsync(user, "TEMP_DELIVERER"))
                {
                    model.Role = "DELIVERER";
                }
                else
                {
                    model.Role = "CONSUMER";
                }

                // Set status
                ViewBag.Status = _signUpRequestService.GetLatestRequestStatus(user.Id);

                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        public IActionResult DelivererPreview(string userId, SignUpRequestStatus status)
        {
            try
            {
                var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
                if (user != null)
                {
                    var model = _mapper.Map<AppUser, ProfileViewModel>(user);
                    model.ConfirmPassword = model.Password;
                    model.Role = "DELIVERER";
                    ViewBag.DelivererPreview = true;
                    ViewBag.Status = status;
                    return View("Profile", model);
                }
                return View("MyError", new ErrorViewModel() { Message = "User not found" });
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }

        #endregion

        #region Admin
        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpGet]
        public IActionResult Verification()
        {
            try
            {
                VerificationModel model = new VerificationModel()
                {
                    Requests = _signUpRequestService.GetAllRequests()
                };
                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpPost]
        public async Task<IActionResult> Verify()
        {
            try
            {
                // Find user
                var userId = Request.Form["userId"];
                var user = await _userManager.FindByIdAsync(userId);

                var id = Convert.ToInt32(Request.Form["id"]);
                var approved = Convert.ToBoolean(Request.Form["approved"]);

                SignUpRequestStatus status;
                if (approved)
                {
                    status = SignUpRequestStatus.APPROVED;
                    await _userManager.AddToRoleAsync(user, "DELIVERER");
                }
                else
                {
                    status = SignUpRequestStatus.DISAPPROVED;
                    await _userManager.RemoveFromRoleAsync(user, "DELIVERER");
                }

                await _signUpRequestService.Update(id, status);

                // Send mail 
                if (user != null)
                {
                    var message = $"<p>Registration request <b>{status}</b> by administrator(s)</p>";
                    await _emailService.SendEmailAsync(user.Email, "Liman Restaurant - Registration", message);
                }

                return RedirectToAction("Verification");
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpGet]
        public IActionResult NewProduct()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }
        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpPost]
        public IActionResult NewProduct(NewProductModel model)
        {
            try
            {
                ViewBag.Success = "";
                bool productWithSameName = false;

                if (ModelState.IsValid)
                {
                    lock (createProductLocker)
                    {
                        if (_productService.Read(model.Name) != null)
                        {
                            productWithSameName = true;
                        }
                        else
                        {
                            _productService.Create(new Product { Name = model.Name, Price = model.Price, Ingridients = model.Ingridients });

                            ViewBag.Success = "Your new product is successfully created!";
                        }
                        Thread.Sleep(5000);
                    }
                }
                else
                {
                    if (model.Ingridients == string.Empty || model.Ingridients == null)
                    {
                        ModelState.AddModelError("", "A product must contain minimum 1 ingridient.");
                    }
                }
                if (productWithSameName)
                {
                    ModelState.AddModelError("", "A product with that name already exists.");
                }
                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }
        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpGet]
        public IActionResult AllOrders()
        {
            try
            {
                ActiveOrdersModel model = new ActiveOrdersModel();

                model.Orders = _orderService.ReadAll();

                model.Products = new Dictionary<int, Dictionary<Product, int>>();

                foreach (var order in model.Orders)
                {
                    var productOrders = _productOrderService.ReadForOrder(order.OrderId);
                    var orderProducts = new Dictionary<Product, int>();
                    foreach (var productOrder in productOrders)
                    {
                        var product = _productService.Read(productOrder.ProductId);
                        if (product == null)
                        {
                            continue;
                        }
                        orderProducts.Add(product, productOrder.Amount);
                    }
                    model.Products.Add(order.OrderId, orderProducts);
                }

                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }
        #endregion

        #region Deliverer
        [Authorize(Roles = "DELIVERER")]
        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            try
            {
                ActiveOrdersModel model = new ActiveOrdersModel();

                var currUser = await _userManager.GetUserAsync(User);
                if (currUser == null)
                {
                    return RedirectToAction("MyError", new ErrorViewModel() { Message = "User not found" });
                }

                var myOrders = _orderService.ReadCompletedOrdersByDeliverer(currUser.Id);
                model.Orders = myOrders;

                model.Products = new Dictionary<int, Dictionary<Product, int>>();

                foreach (var order in myOrders)
                {
                    var productOrders = _productOrderService.ReadForOrder(order.OrderId);
                    var orderProducts = new Dictionary<Product, int>();
                    foreach (var productOrder in productOrders)
                    {
                        var product = _productService.Read(productOrder.ProductId);
                        if (product == null)
                        {
                            continue;
                        }
                        orderProducts.Add(product, productOrder.Amount);
                    }
                    model.Products.Add(order.OrderId, orderProducts);
                }

                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }
        [Authorize(Roles = "DELIVERER")]
        [HttpGet]
        public async Task<IActionResult> NewOrders()
        {
            try
            {
                ActiveOrdersModel model = new ActiveOrdersModel();

                // Read all pending orders for Deliverer to take
                model.Orders = _orderService.ReadAll(OrderState.ON_HOLD);

                if (model.Orders != null && model.Orders.Count > 0)
                {
                    model.Products = new Dictionary<int, Dictionary<Product, int>>();
                    foreach (var order in model.Orders)
                    {
                        var productOrders = _productOrderService.ReadForOrder(order.OrderId);

                        var orderProducts = new Dictionary<Product, int>();

                        foreach (var productOrder in productOrders)
                        {
                            // Read product
                            var product = _productService.Read(productOrder.ProductId);

                            // Add product with amount of it
                            orderProducts.Add(product, productOrder.Amount);
                        }
                        // Link all products with amounts to belonging order
                        model.Products.Add(order.OrderId, orderProducts);
                    }
                }

                var currUser = await _userManager.GetUserAsync(HttpContext.User);
                if (currUser == null)
                {
                    return RedirectToAction("MyError", new ErrorViewModel() { Message = "User not found" });
                }

                var orderId = _orderService.DelivererHasActiveOrder(currUser.Id);

                ViewBag.HasActiveOrder = (orderId != -1);

                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            } 
        }
        [Authorize(Roles = "DELIVERER")]
        [HttpPost]
        public async Task<IActionResult> TakeOrder()
        {
            try
            {
                var currUser = await _userManager.GetUserAsync(HttpContext.User);
                if (currUser == null)
                {
                    return RedirectToAction("MyError", new ErrorViewModel() { Message = "User not found" });
                }

                bool orderNotFound = false;
                bool orderSuccessfullyUpdated = false;
                var triedOrderId = -1;
                var updatedOrderId = -1;
                ViewBag.ConcurrentMessage = "";
                lock (takeOrderLocker)
                {
                    var orderId = Convert.ToInt32(Request.Form["takenOrderId"]);
                    var order = _orderService.Read(orderId);
                    if (order == null)
                    {
                        orderNotFound = true;
                    }
                    else
                    {
                        triedOrderId = orderId;
                        if (order.OrderState == OrderState.ON_HOLD)
                        {
                            order.OrderState = OrderState.ACTIVE;

                            order.DelivererId = currUser.Id;

                            // Update order
                            _orderService.Update(order);

                            updatedOrderId = order.OrderId;

                            orderSuccessfullyUpdated = true;
                        }
                    }
                }
                if (orderNotFound)
                {
                    return RedirectToAction("MyError", new ErrorViewModel() { Message = "Order not found" });
                }
                else if (orderSuccessfullyUpdated)
                {
                    return RedirectToAction("CurrentOrder", new { orderId = updatedOrderId });
                }

                ViewBag.ConcurrentMessage = "Order that you are trying to take is no longer available.";
                return RedirectToAction("CurrentOrder", new { orderId = triedOrderId });
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }

        [Authorize(Roles = "DELIVERER")]
        [HttpPost]
        public IActionResult FinishOrder()
        {
            try
            {
                var orderId = Convert.ToInt32(Request.Form["takenOrderId"]);
                var order = _orderService.Read(orderId);
                if (order == null)
                {
                    return RedirectToAction("MyError", new ErrorViewModel() { Message = "Order not found" });
                }

                Enum.TryParse(Request.Form["finishedOrderStatus"], out OrderState orderState);

                if (orderState != OrderState.COMPLETE && orderState != OrderState.INCOMPLETE)
                {
                    return RedirectToAction("MyError", new ErrorViewModel() { Message = "Unexpected order state" });
                }

                order.OrderState = orderState;

                _orderService.Update(order);

                return RedirectToAction("CurrentOrder", new { orderId = orderId });
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }
        [Authorize(Roles = "DELIVERER")]
        [HttpGet]
        public async Task<IActionResult> CheckCurrentOrder()
        {
            try
            {
                var currUser = await _userManager.GetUserAsync(User);
                if (currUser == null)
                {
                    return RedirectToAction("MyError", new ErrorViewModel() { Message = "User not found" });
                }
                var orderId = _orderService.DelivererHasActiveOrder(currUser.Id);

                return RedirectToAction("CurrentOrder", new { orderId = orderId });
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }
        #endregion

        #region Consumer
        [Authorize(Roles = "CONSUMER")]
        [HttpGet]
        public async Task<IActionResult> PreviousOrders()
        {
            try
            {
                ActiveOrdersModel model = new ActiveOrdersModel();
                var currUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                if (currUser == null)
                {
                    return RedirectToAction("MyError", new ErrorViewModel() { Message = "User not found" });
                }
                var userOrders = _orderService.ReadAll(currUser.Id);
                var completeUserOrders = userOrders.FindAll(x => x.OrderState == OrderState.COMPLETE);
                if (completeUserOrders != null)
                {
                    model.Orders = completeUserOrders;
                    var productOrders = _productOrderService.ReadAll();
                    var products = _productService.ReadAll();
                    if (productOrders != null && products != null)
                    {
                        model.Products = new Dictionary<int, Dictionary<Product, int>>();
                        foreach (var order in model.Orders)
                        {
                            var orderProducts = productOrders.FindAll(x => x.OrderId == order.OrderId);
                            if (orderProducts == null)
                            {
                                continue;
                            }
                            model.Products[order.OrderId] = new Dictionary<Product, int>();
                            foreach (var orderProduct in orderProducts)
                            {
                                var product = products.Find(x => x.ProductId == orderProduct.ProductId);
                                if (product == null)
                                {
                                    continue;
                                }
                                model.Products[order.OrderId].Add(product, orderProduct.Amount);
                            }
                        }
                    }
                }
                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }
        [Authorize(Roles = "CONSUMER")]
        [HttpGet]
        public IActionResult NewOrder(string[] errors = null)
        {
            try
            {
                if (errors != null)
                {
                    foreach (var error in errors)
                    {
                        if (error == null || error == string.Empty)
                        {
                            continue;
                        }
                        ModelState.AddModelError("", error);
                    }
                }
                ViewData["products"] = _productService.ReadAll();
                NewOrderModel model = new NewOrderModel();
                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }

        [Authorize(Roles = "CONSUMER")]
        [HttpPost]
        public async Task<IActionResult> NewOrder(NewOrderModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newOrder = new Order();
                    newOrder.OrderState = OrderState.ON_HOLD;
                    newOrder.Price = model.Price;
                    newOrder.Time = 45;
                    newOrder.Address = model.Address;
                    newOrder.Comment = model.Comment;
                    var currUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                    if (currUser != null)
                    {
                        newOrder.AppUserId = currUser.Id;
                    }

                    await _orderService.Create(newOrder);

                    var productData = Helpers.Helpers.CountProducts(model.Products);

                    foreach (var item in productData)
                    {
                        var product = _productService.Read(item.Key);
                        if (product == null)
                        {
                            continue;
                        }
                        var newProductOrder = new ProductOrder();
                        newProductOrder.Product = product;
                        newProductOrder.ProductId = product.ProductId;
                        newProductOrder.Amount = item.Value;
                        newProductOrder.Order = newOrder;
                        newProductOrder.OrderId = newOrder.OrderId;

                        product.ProductOrders.Add(newProductOrder);
                        newOrder.ProductOrders.Add(newProductOrder);

                        _productService.Update(product);
                        _orderService.Update(newOrder);
                    }
                    return RedirectToAction("CurrentOrder", "Home", new { orderId = newOrder.OrderId });
                }

                if (model.Products == string.Empty || model.Products == null)
                {
                    ModelState.AddModelError("", "Minimum 1 product is required");
                }

                string[] errors = new string[ModelState.ErrorCount];
                short i = 0;
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        if (error.ErrorMessage.Contains("The Products field"))
                        {
                            continue;
                        }
                        errors[i++] = error.ErrorMessage;
                    }
                }
                if (i == 0)
                {
                    errors = null;
                }

                return RedirectToAction("NewOrder", new { errors = errors });
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }

        [Authorize(Roles = "CONSUMER, DELIVERER")]
        [HttpGet]
        public async Task<IActionResult> CurrentOrder(int orderId = -2)
        {
            try
            {
                if (orderId == -2)
                {
                    return RedirectToAction("MyError", new ErrorViewModel() { Message = "Error with order ID" });
                }

                if (orderId == -1)
                {
                    ViewBag.Message = "You don't have a current order or the order that you are looking for is no longer available.";
                    return View();
                }

                var order = _orderService.Read(orderId);
                if (order == null)
                {
                    return RedirectToAction("MyError", new ErrorViewModel() { Message = "Order not found" });
                }

                CurrentOrderModel model = new CurrentOrderModel()
                {
                    Id = order.OrderId,
                    Address = order.Address,
                    Comment = order.Comment,
                    Price = order.Price,
                    Time = order.Time,
                    Status = order.OrderState,
                    Products = new Dictionary<Product, int>()
                };

                var productOrders = _productOrderService.ReadForOrder(order.OrderId);
                foreach (var productOrder in productOrders)
                {
                    if (productOrder.OrderId == order.OrderId)
                    {
                        var product = _productService.Read(productOrder.ProductId);
                        if (product != null)
                        {
                            model.Products[product] = productOrder.Amount;
                        }
                    }

                }

                var currUser = await _userManager.GetUserAsync(HttpContext.User);
                if (currUser == null)
                {
                    return RedirectToAction("MyError", new ErrorViewModel() { Message = "User not found" });
                }

                if (User.IsInRole("DELIVERER"))
                {
                    var delivererOrderId = _orderService.DelivererHasActiveOrder(currUser.Id);

                    ViewBag.HasActiveOrder = (delivererOrderId != -1);
                    if (ViewBag.HasActiveOrder)
                    {
                        ViewBag.ThisOrder = (delivererOrderId == orderId);
                    }
                }

                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }

        [Authorize(Roles = "CONSUMER, DELIVERER")]
        [HttpPost]
        public IActionResult CurrentOrder()
        {
            try
            {
                int orderId = Convert.ToInt32(Request.Form["orderId"]);

                return RedirectToAction("CurrentOrder", new { orderId = orderId });
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }
        [Authorize(Roles = "CONSUMER")]
        [HttpGet]
        public async Task<IActionResult> ActiveOrders()
        {
            try
            {
                ActiveOrdersModel model = new ActiveOrdersModel();
                var currUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                if (currUser == null)
                {
                    return RedirectToAction("MyError", new ErrorViewModel() { Message = "User not found" });
                }
                var userOrders = _orderService.ReadAll(currUser.Id);
                var activeUserOrders = userOrders.FindAll(x => x.OrderState == OrderState.ON_HOLD);
                if (activeUserOrders != null)
                {
                    model.Orders = activeUserOrders;
                    var productOrders = _productOrderService.ReadAll();
                    var products = _productService.ReadAll();
                    if (productOrders != null && products != null)
                    {
                        model.Products = new Dictionary<int, Dictionary<Product, int>>();
                        foreach (var order in model.Orders)
                        {
                            var orderProducts = productOrders.FindAll(x => x.OrderId == order.OrderId);
                            if (orderProducts == null)
                            {
                                continue;
                            }
                            model.Products[order.OrderId] = new Dictionary<Product, int>();
                            foreach (var orderProduct in orderProducts)
                            {
                                var product = products.Find(x => x.ProductId == orderProduct.ProductId);
                                if (product == null)
                                {
                                    continue;
                                }
                                model.Products[order.OrderId].Add(product, orderProduct.Amount);
                            }
                        }
                    }
                }
                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("MyError", new ErrorViewModel() { Message = e.Message });
            }
        }
        #endregion

        [HttpPost]
        public void UploadImage()
        {
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    var imageFile = Request.Form.Files.FirstOrDefault();

                    var fullPath = Path.Combine(_env.ContentRootPath, "wwwroot\\images", imageFile.FileName);

                    using (FileStream fs = System.IO.File.Create(fullPath))
                    {
                        imageFile.CopyTo(fs);
                        fs.Flush();
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult MyError(ErrorViewModel model)
        {
            model.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View("Error",model);
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
            //// Create role
            //IdentityRole role = new IdentityRole
            //{
            //    Name = Request.Form["role"]
            //};

            //await _roleManager.CreateAsync(role);

            //var luka_consumer = _userManager.Users.First(x => x.UserName == "Luka97_2");
            var luka_deliverer = _userManager.Users.FirstOrDefault(x => x.UserName == "LukaDeliverer");

            //luka_consumer.UserName = "LukaConsumer";
            //luka_deliverer.UserName = "LukaDeliverer";

            //await _userManager.UpdateAsync(luka_consumer);
            //await _userManager.UpdateAsync(luka_deliverer);

            await _userManager.AddToRoleAsync(luka_deliverer, "ADMINISTRATOR");
            
            return View("AddRole");
        }
        #endregion
    }
}
