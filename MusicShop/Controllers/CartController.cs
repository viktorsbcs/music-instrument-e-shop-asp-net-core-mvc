﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicShop.Models;
using MusicShop.Models.Interfaces;
using MusicShop.Models.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MusicShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly Cart _cart;

        public CartController(IProductRepository productRepository, Cart cart)
        {
            this._productRepository = productRepository;
            this._cart = cart;
        }
        public IActionResult AddToCart(int productId, int quantity)
        {
            var product = _productRepository.GetProductById(productId);
            if (product != null)
            {
                _cart.AddToCart(product, quantity);
            }


            return RedirectToAction("Products", "Home");
            
        }

        public IActionResult ViewCart()
        {
            _cart.CartItems = _cart.GetCartItems();
            return View(new CartViewModel() { CartId = _cart.CartId ,CartItems = _cart.CartItems, CartTotalValue = _cart.TotalCartValue() });
        }

       
    }
}
