using System;

namespace InventoryManagementSystem.Models
{
    /// <summary>
    /// Represents an item category.
    /// </summary>
    public class ItemCategory
    {
        public Guid id { get; set; }
        public string name { get; set; }

        /// <summary>
        /// Returns the name of the category
        /// </summary>
        /// <returns>The name of the category</returns>
        public override string ToString()
        {
            return name;
        }
    }

    /// <summary>
    /// Represents an item.
    /// </summary>
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
        public int stock { get; set; }

    }

    /// <summary>
    /// Represents a user.
    /// </summary>
    public class User
    {
        public Guid id { get; set; }
        public string userName { get; set; }
        public string userEmail { get; set; }
        public string userPassword { get; set; }
        public string userAddress { get; set; }
        public string userSalt { get; set; }
        public UserType userType { get; set; }
    }

    /// <summary>
    /// Represents the type of a user.
    /// </summary>
    public enum UserType
    {
        User,
        Admin
    }

    /// <summary>
    /// Represents the status of an item.
    /// </summary>
    public enum ItemStatus
    {
        InStock,
        OutOfStock,
        LowStock,
        Unknown
    }
}
