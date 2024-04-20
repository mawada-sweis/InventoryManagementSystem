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

        void ICategoriesService.AddCategory(string categoryName, ref List<ItemCategory> categories)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionsString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand())
                    {
                        Guid category_id = Guid.NewGuid();
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO categories (category_id, category_name) VALUES (@id, @name)";
                        command.Parameters.AddWithValue("@id", category_id);
                        command.Parameters.AddWithValue("@name", categoryName);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            categories.Add(new ItemCategory
                            {
                                id = category_id,
                                name = categoryName
                            });
                            Console.WriteLine($"Category '{categoryName}' added successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to add category '{categoryName}'.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        void ICategoriesService.DeleteCategory(ref ItemCategory categoryToDelete, ref List<ItemCategory> categories)
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
                        Console.WriteLine("Category deleted successfully.");
                    }
                    else Console.WriteLine("Category not found in the database.");
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
                            if (!reader.HasRows)
                            {
                                Console.WriteLine("No categories until now");
                                return false;
                            }
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

        void ICategoriesService.UpdateCategory(string categoryName, ref ItemCategory category, ref List<ItemCategory> categories)
        {
            using (var connection = new NpgsqlConnection(_connectionsString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
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
                                category.name = categoryName;
                                Console.WriteLine("Categroy updated successfully");
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error updating category: " + ex.Message);
                        transaction.Rollback();
                    }
                }
            }

        }
    }
}
