﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MusicShop.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string CartId { get; set; }

        [ForeignKey("IdentityUser")]
        public string UserId { get; set; }

        public IdentityUser User { get; set; }
        public Cart Cart { get; set; }

        public List<CartItem> CartItems {get; set; }

        public decimal TotalOrderValue { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
