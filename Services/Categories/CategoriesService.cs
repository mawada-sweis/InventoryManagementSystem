using InventoryManagementSystem.Models;
using Npgsql;
using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Services.Categories
{
    public class CategoriesService : ICategoriesService
    {
        private readonly string _connectionsString;

        public CategoriesService(string connectionsString)
        {
            this._connectionsString = connectionsString;
        }

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
                        if (categoryFound != null ) categoryFound.name = categoryName;
                        
                        category.name = categoryName;
                        return true;
                    }
                    else return false;

                }
            }

        }
    }
}
