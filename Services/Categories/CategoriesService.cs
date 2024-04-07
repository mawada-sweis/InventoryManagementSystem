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

        void ICategoriesService.DeleteCategory(string categoryName, ref List<ItemCategory> categories)
        {
            throw new NotImplementedException();
        }

        void ICategoriesService.GetCategories(ref List<ItemCategory> categories)
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
                                return;
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
                        }
                    }
                }

                if (categories.Count > 0)
                {
                    foreach (var category in categories)
                    {
                        Console.WriteLine($"Name: {category.name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        void ICategoriesService.UpdateCategory(string categoryName, ref List<ItemCategory> categories)
        {
            throw new NotImplementedException();
        }
    }
}
