using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models
{
    public class ItemCategory
    {
        public Guid id { get; set; }
        public string name { get; set; }
    }
    public class Item
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int price { get; set; }
        public ItemStatus status { get; set; }
        public int sold { get; set; }
        public int minQuantity { get; set; }
        public int quantity { get; set; }
        public ItemCategory category { get; set; }

    }

    public class User
    {
        public Guid id { get; set; }
        public string userName { get; set; }
        public string userEmail { get; set; }
        public string userPassword { get; set; }
        public string userAddress { get; set; }
        public string userSalt { get; set; }
        public UserType userType { get; set; }
        public List<Order> orders { get; set; }
    }

    public class Order
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public List<Item> items { get; set; }
        public User user { get; set; }
        public int total { get; set; }
        public OrderStatus status { get; set; }
    }

    public enum UserType
    {
        User,
        Admin
    }

    public enum ItemStatus
    {
        InStock,
        OutOfStock,
        LowStock,
        Unknown
    }

    public enum OrderStatus
    {
        foo
    }
}
