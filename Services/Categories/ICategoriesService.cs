using InventoryManagementSystem.Models;
using System.Collections.Generic;

namespace InventoryManagementSystem.Services.Categories
{
    /// <summary>
    /// Service for managing item categories.
    /// </summary>
    public interface ICategoriesService
    {
        /// <summary>
        /// Retrieves all categories from the database.
        /// </summary>
        /// <param name="categories">The list to populate with categories.</param>
        /// <returns>True if categories are successfully retrieved, otherwise false.</returns>
        bool GetCategories(ref List<ItemCategory> categories);

        /// <summary>
        /// Adds a new category to the database.
        /// </summary>
        /// <param name="category">The category to add.</param>
        /// <param name="categories">The list of categories to update after adding.</param>
        /// <returns>True if the category is successfully added, otherwise false.</returns>
        bool AddCategory(ItemCategory category, ref List<ItemCategory> categories);

        /// <summary>
        /// Deletes a category from the database.
        /// </summary>
        /// <param name="categoryToDelete">The category to delete.</param>
        /// <param name="categories">The list of categories to update after deletion.</param>
        /// <returns>True if the category is successfully deleted, otherwise false.</returns>
        bool DeleteCategory(ref ItemCategory categoryToDelete, ref List<ItemCategory> categories);

        /// <summary>
        /// Updates the name of a category in the database.
        /// </summary>
        /// <param name="categoryName">The new name for the category.</param>
        /// <param name="category">The category to update.</param>
        /// <param name="categories">The list of categories to update after renaming.</param>
        /// <returns>True if the category is successfully updated, otherwise false.</returns>
        bool UpdateCategory(string categoryName, ref ItemCategory category, ref List<ItemCategory> categories);
    }
}
