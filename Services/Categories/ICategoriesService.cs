using InventoryManagementSystem.Models;
using System.Collections.Generic;

namespace InventoryManagementSystem.Services.Categories
{
    public interface ICategoriesService
    {
        ItemCategory GetCategories(List<ItemCategory> categories);
        void AddCategory(string category, List<ItemCategory> categories);
        void DeleteCategory(string categoryName, List<ItemCategory> categories);
        void UpdateCategory(string categoryName, List<ItemCategory> categories);
    }
}
