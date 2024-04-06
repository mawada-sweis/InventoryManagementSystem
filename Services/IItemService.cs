using InventoryManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Services
{
    public interface IItemService
    {
        List<Item> GetItems(ref List<Item> items);
        bool AddItem(string name,
            string description,
            int price,
            ItemStatus status,
            int quantity,
            int minQuantity,
            int sold = 0
            );
        bool UpdateItem(ref Item item, Item newItem);
        void DeleteItem(ref List<Item> items, Guid guid);
    }
}
