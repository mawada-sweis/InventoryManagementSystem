using InventoryManagementSystem.Models;
using System.Collections.Generic;

namespace InventoryManagementSystem.Services.Categories
{
    internal interface ICategoriesService
    {
        List<Category> GetCategories();
        void AddCategory(string category);
        void DeleteCategory(string categoryName);
        void UpdateCategory(string categoryName);
    }
}
