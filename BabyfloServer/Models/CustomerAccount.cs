using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BabyfloServer.Models
{
    public class CustomerAccount
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ContactNumber { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string DeliveryAddress { get; set; } = string.Empty;

        public DateTime AccountCreated { get; set; } = DateTime.UtcNow;

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}