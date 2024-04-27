using InventoryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace InventoryManagementSystem.Services.Items
{
    public interface IItemService
    {
        void GetItems(ref List<Item> items);
        bool AddItem(Item newItem, ref List<Item> items);
        bool UpdateItem(ref Item item, Item newItem);
        bool DeleteItem(ref List<Item> items, Guid guid);
        bool UpdateQuantity(ref List<Item> items, Guid guid, int newQuantity);
        bool UpdateSoldItem(ref List<Item> items, Guid guid, int sold);
        Item GetItemByName(string itemName, List<ItemCategory> categories);
        List<Item> GetFilterItems(string creteria, string creteriaValue, [Optional] string operatorString);
    }
}
