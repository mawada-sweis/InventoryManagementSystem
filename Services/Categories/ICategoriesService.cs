using InventoryManagementSystem.Models;
using System.Collections.Generic;

namespace InventoryManagementSystem.Services.Categories
{
    public interface ICategoriesService
    {
        ItemCategory GetCategories();
        void AddCategory(string category);
        void DeleteCategory(string categoryName);
        void UpdateCategory(string categoryName);
    }
}
