using InventoryManagementSystem.Models;
using System.Collections.Generic;

namespace InventoryManagementSystem.Services.Categories
{
    public interface ICategoriesService
    {
        void GetCategories(ref List<ItemCategory> categories);
        void AddCategory(string category, ref List<ItemCategory> categories);
        void DeleteCategory(string categoryName, ref List<ItemCategory> categories);
        void UpdateCategory(string categoryName, ref ItemCategory category, ref List<ItemCategory> categories);
    }
}
