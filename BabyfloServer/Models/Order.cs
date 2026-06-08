using System;
using System.Collections.Generic;

namespace BabyfloServer.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        // Customer & Shipping Details
        public string CustomerName { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public int? CustomerAccountId { get; set; }
        
        // Payment Metrics
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; 
        public string ReferenceNumber { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = "Pending"; 
        
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // Relationship linking items
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
    }
}