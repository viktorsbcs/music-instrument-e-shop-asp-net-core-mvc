﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicShop.Models;
using MusicShop.Models.Interfaces;
using MusicShop.Models.ViewModels;

namespace MusicShop.Controllers
{
    [Authorize(Roles = "Admin")]

    public class AdministrationController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IProductRepository _productRepository;
        private const string _SUPERADMINEMAIL = "admin@gmail.com";

        public AdministrationController(IProductRepository productRepository, ICategoryRepository categoryRepository, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {

            this._productRepository = productRepository;
            this._categoryRepository = categoryRepository;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }

        
        public IActionResult AdminPanel()
        { 
            try
            {
                return View();
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                ModelState.AddModelError("", "Not authotized");
                return View("Home", "Products");
            }

        }

        public IActionResult ViewProducts()
        {
            IEnumerable<Product> products = _productRepository.AllProducts;
            return View(products);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {

            return View(new AddProductViewModel() { CategoryList = _categoryRepository.AllCategories });
        }

        [HttpPost]
        public IActionResult AddProduct(AddProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                Product product = new Product()
                {

                    ProductName = model.ProductName,
                    CategoryId = model.CategoryId,
                    ShortDescription = model.ShortDescription,
                    LongDescription = model.LongDescription,
                    AmountInStock = model.AmountInStock,
                    ImageThumbnailUrl = model.ImageThumbnailUrl,
                    ImageUrl = model.ImageUrl,
                    InStock = true,
                    Price = (decimal)model.Price
                };

                _productRepository.CreateProduct(product);
                return RedirectToAction("AdminPanel");
            }

            ModelState.AddModelError("", "Information incorrect");


            return View(model);
        }


        public IActionResult AddCategory()
        {

            //Category category = new Category()
            //{
            //    CategoryName = "Strings",
            //    CategoryDescription = "Guitar and bass strings"
            //};

            //_categoryRepository.CreateCategory(category);
            return RedirectToAction("AdminPanel");
        }

        public async Task<IActionResult> ViewUsers()
        {
            var userList = _userManager.Users.ToList();

            if (userList == null)
            {
                ModelState.AddModelError("", "No users found");
                return View();
            }
            else
            {
                List<UserViewModel> model = new List<UserViewModel>();

                //Skip including admin account in the list
                var rolesList = _roleManager.Roles.ToList();

                foreach (var user in userList)
                {
                    if (user.Email == _SUPERADMINEMAIL)
                    {
                        continue;
                    }

                    List<RoleViewModel> roleViewModel = new List<RoleViewModel>();

                    foreach (var role in rolesList)
                    {
                        var isUserInRole = await _userManager.IsInRoleAsync(user, role.Name);

                        var roleModel = new RoleViewModel()
                        {
                            Id = role.Id,
                            Name = role.Name,
                            Selected = isUserInRole ? true : false
                        };

                        roleViewModel.Add(roleModel);
                    }

                    var userViewModel = new UserViewModel()
                    {
                        Id = user.Id,
                        Name = user.UserName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        UserInRoles = roleViewModel
                    };

                    model.Add(userViewModel);
                }

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails(string id)
        {
            ViewBag.returnUrl = Request.Headers["Referer"].ToString();

            var user = await _userManager.FindByIdAsync(id);

            if(user != null)
            {
                List<RoleViewModel> roleViewModel = new List<RoleViewModel>();
                var rolesList = _roleManager.Roles.ToList();

                foreach (var role in rolesList)
                {
                    var isUserInRole = await _userManager.IsInRoleAsync(user, role.Name);

                    var roleModel = new RoleViewModel()
                    {
                        Id = role.Id,
                        Name = role.Name,
                        Selected = isUserInRole ? true : false
                    };

                    roleViewModel.Add(roleModel);
                }

                var userViewModel = new UserViewModel()
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    UserInRoles = roleViewModel

                };



                return View(userViewModel);
            }



            ModelState.AddModelError("", "User Id not found");
            return View();
        }

        //Assign & unnasign user roles
        [HttpPost]
        public async Task<IActionResult> UserDetails(UserViewModel model)
        {

            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByIdAsync(model.Id);

                foreach (var role in model.UserInRoles)
                {
                    if (role.Selected)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    } else
                    {
                        await _userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                }

                TempData["UserRolesUpdated"] = "User roles successfuly updated";
               
                return View(model);

            }


            ModelState.AddModelError("", "Error updating user");
            return View(model);
        }

        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    TempData["UserDeleteSuccess"] = "User successfully deleted";
                    return RedirectToAction("ViewUsers");
                }

            }

            ModelState.AddModelError("", "User deletion failed");
            return RedirectToAction("ViewUsers");
        }
    }


}