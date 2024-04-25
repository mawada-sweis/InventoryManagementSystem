﻿using InventoryManagementSystem.Models;
using Npgsql;
using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Services.Items
{
    internal class ItemService : IItemService
    {
        private readonly string _connectionString;
        public ItemService(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public bool AddItem(Item newItem, ref List<Item> items)
        {
            string query = @"INSERT INTO items (item_id, item_name, item_description, item_price, item_status,
                item_quantity_available, item_sold, item_min_quantity, category_id)
                VALUES (@id, @name, @description, @price, @status, @quantityAvailable, @sold, @minQuantity, @category)
                ON CONFLICT (item_name) DO NOTHING";
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddWithValue("@id", newItem.id);
                    command.Parameters.AddWithValue("@name", newItem.name);
                    command.Parameters.AddWithValue("@description", newItem.description);
                    command.Parameters.AddWithValue("@price", newItem.price);
                    command.Parameters.AddWithValue("@status", newItem.status.ToString());
                    command.Parameters.AddWithValue("@quantityAvailable", newItem.quantity);
                    command.Parameters.AddWithValue("@sold", newItem.sold);
                    command.Parameters.AddWithValue("@minQuantity", newItem.minQuantity);
                    command.Parameters.AddWithValue("@category", newItem.category.id);

                    if (command.ExecuteNonQuery() > 0)
                    {
                        items.Add(newItem);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"{newItem.name} addedd faild");
                        return false;
                    }
                }
            }
        }

        public bool DeleteItem(ref List<Item> items, Guid guid)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM items WHERE item_id = @ItemID";
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("ItemID", guid);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"Deleted successfully");
                            items.RemoveAll(item => item.id == guid);
                            return true;
                        }
                        else
                        {
                            Console.WriteLine($"Failed to delete item");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public void GetItems(ref List<Item> items)
        {
            try
            {
                string query = "SELECT * FROM items";
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string statusString = reader.GetString(reader.GetOrdinal("item_status"));
                                // Convert enum string to corresponding enum value
                                ItemStatus status;
                                if (!Enum.TryParse(statusString, out status))
                                {
                                    status = ItemStatus.Unknown;
                                }
                                Item item = new Item
                                {
                                    id = reader.GetGuid(reader.GetOrdinal("item_id")),
                                    name = reader.GetString(reader.GetOrdinal("item_name")),
                                    description = reader.GetString(reader.GetOrdinal("item_description")),
                                    price = reader.GetInt32(reader.GetOrdinal("item_price")),
                                    status = status,
                                    quantity = reader.GetInt32(reader.GetOrdinal("item_quantity_available")),
                                    sold = reader.GetInt32(reader.GetOrdinal("item_sold")),
                                    minQuantity = reader.GetInt32(reader.GetOrdinal("item_min_quantity")),
                                };
                                if (!items.Exists(i => i.id == item.id))
                                {
                                    items.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public bool UpdateItem(ref Item item, Item newItem)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "UPDATE items SET " +
                                   "item_name = @Name, " +
                                   "item_description = @Description, " +
                                   "item_price = @Price, " +
                                   "item_status = @Status, " +
                                   "item_quantity_available = @QuantityAvailable, " +
                                   "item_sold = @Sold, " +
                                   "item_min_quantity = @MinQuantity, " +
                                   "category_id = @CategoryID " +
                                   "WHERE item_id = @ID " +
                                   "AND (item_name <> @Name " +
                                   "OR item_description <> @Description " +
                                   "OR item_price <> @Price " +
                                   "OR item_status <> @Status " +
                                   "OR item_quantity_available <> @QuantityAvailable " +
                                   "OR item_sold <> @Sold " +
                                   "OR item_min_quantity <> @MinQuantity " +
                                   "OR category_id <> @CategoryID)";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", newItem.name);
                        command.Parameters.AddWithValue("@Description", newItem.description);
                        command.Parameters.AddWithValue("@Price", newItem.price);
                        command.Parameters.AddWithValue("@Status", newItem.status.ToString());
                        command.Parameters.AddWithValue("@QuantityAvailable", newItem.quantity);
                        command.Parameters.AddWithValue("@Sold", newItem.sold);
                        command.Parameters.AddWithValue("@MinQuantity", newItem.minQuantity);
                        command.Parameters.AddWithValue("@CategoryID", newItem.category.id);
                        command.Parameters.AddWithValue("@ID", item.id);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            item = newItem;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while updating the item: " + ex.Message);
                return false;
            }
        }

    }
}
