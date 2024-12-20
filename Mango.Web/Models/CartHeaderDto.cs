﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Web.Models
{
    public class CartHeaderDto
    {
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double CartTotal { get; set; } = 0;

        // Optional fields / useful to properly send data with help of ServiceBus
        public string ? FirstName { get; set; }
        public string? LastName { get; set; }
        public string ? Phone { get; set; }
        public string? Email { get; set; }
    
    }
}
