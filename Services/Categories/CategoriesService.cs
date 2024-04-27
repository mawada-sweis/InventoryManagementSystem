using InventoryManagementSystem.Models;
using Npgsql;
using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Services.Categories
{
    /// <summary>
    /// Service for managing categories of items.
    /// </summary>
    public class CategoriesService : ICategoriesService
    {
        private readonly string _connectionsString;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoriesService"/> class.
        /// </summary>
        /// <param name="connectionsString">The connection string to the database.</param>
        public CategoriesService(string connectionsString)
        {
            this._connectionsString = connectionsString;
        }

        /// <summary>
        /// Adds a new category to the database.
        /// </summary>
        /// <param name="category">The category to add.</param>
        /// <param name="categories">The list of categories to update after adding.</param>
        /// <returns>True if the category is successfully added, otherwise false.</returns>
        bool ICategoriesService.AddCategory(ItemCategory category, ref List<ItemCategory> categories)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionsString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO categories (category_id, category_name) VALUES (@id, @name)";
                        command.Parameters.AddWithValue("@id", category.id);
                        command.Parameters.AddWithValue("@name", category.name);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            categories.Add(new ItemCategory
                            {
                                id = category.id,
                                name = category.name
                            });
                            return true;
                        }
                        else return false;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Deletes a category from the database.
        /// </summary>
        /// <param name="categoryToDelete">The category to delete.</param>
        /// <param name="categories">The list of categories to update after deletion.</param>
        /// <returns>True if the category is successfully deleted, otherwise false.</returns>
        bool ICategoriesService.DeleteCategory(ref ItemCategory categoryToDelete, ref List<ItemCategory> categories)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionsString))
            {
                connection.Open();
                string query = "DELETE FROM categories WHERE category_id = @CategoryID";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryID", categoryToDelete.id);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        categories.Remove(categoryToDelete);
                        return true;
                    }
                    else return false;
                }
            }
        }

        /// <summary>
        /// Retrieves all categories from the database.
        /// </summary>
        /// <param name="categories">The list to populate with categories.</param>
        /// <returns>True if categories are successfully retrieved, otherwise false.</returns>
        bool ICategoriesService.GetCategories(ref List<ItemCategory> categories)
        {
            try
            {
                string query = "SELECT * FROM categories";
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionsString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows) return false;

                            while (reader.Read())
                            {
                                ItemCategory category = new ItemCategory
                                {
                                    id = reader.GetGuid(reader.GetOrdinal("category_id")),
                                    name = reader.GetString(reader.GetOrdinal("category_name")),
                                };
                                if (!categories.Exists(i => i.id == category.id))
                                {
                                    categories.Add(category);
                                }
                            }
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Updates the name of a category in the database.
        /// </summary>
        /// <param name="categoryName">The new name for the category.</param>
        /// <param name="category">The category to update.</param>
        /// <param name="categories">The list of categories to update after renaming.</param>
        /// <returns>True if the category is successfully updated, otherwise false.</returns>
        bool ICategoriesService.UpdateCategory(string categoryName, ref ItemCategory category, ref List<ItemCategory> categories)
        {
            if (categoryName == category.name)
            {
                Console.WriteLine("{0} is same as old value");
                return false;
            }
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionsString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "UPDATE categories SET category_name = @NewName " +
                        "WHERE category_id = @CategoryID";
                    command.Parameters.AddWithValue("NewName", categoryName);
                    command.Parameters.AddWithValue("CategoryID", category.id);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        string oldName = category.name;
                        ItemCategory categoryFound = categories.Find(cat => cat.name.Equals(oldName, StringComparison.OrdinalIgnoreCase));
                        if (categoryFound != null) categoryFound.name = categoryName;

                        category.name = categoryName;
                        return true;
                    }
                    else return false;

                }
            }

        }
    }
}
