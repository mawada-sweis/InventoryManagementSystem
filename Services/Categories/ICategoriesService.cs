using InventoryManagementSystem.Models;
using System.Collections.Generic;

namespace InventoryManagementSystem.Services.Categories
{
    public interface ICategoriesService
    {
        bool GetCategories(ref List<ItemCategory> categories);
        bool AddCategory(ItemCategory category, ref List<ItemCategory> categories);
        bool DeleteCategory(ref ItemCategory categoryToDelete, ref List<ItemCategory> categories);
        bool UpdateCategory(string categoryName, ref ItemCategory category, ref List<ItemCategory> categories);
    }
}
