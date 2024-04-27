using InventoryManagementSystem.Models;
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
                item_quantity, item_sold, item_min_quantity, category_id, item_stock)
                VALUES (@id, @name, @description, @price, @status, @quantity, @sold, @minQuantity, @category, @stock)
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
                    command.Parameters.AddWithValue("@quantity", newItem.quantity);
                    command.Parameters.AddWithValue("@sold", newItem.sold);
                    command.Parameters.AddWithValue("@minQuantity", newItem.minQuantity);
                    command.Parameters.AddWithValue("@category", newItem.category.id);
                    command.Parameters.AddWithValue("@stock", newItem.stock);

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
                                    quantity = reader.GetInt32(reader.GetOrdinal("item_quantity")),
                                    sold = reader.GetInt32(reader.GetOrdinal("item_sold")),
                                    minQuantity = reader.GetInt32(reader.GetOrdinal("item_min_quantity")),
                                    stock = reader.GetInt32(reader.GetOrdinal("item_stock"))
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
            if (newItem.name != item.name ||
                newItem.description != item.description ||
                newItem.price != item.price ||
                newItem.minQuantity != item.minQuantity ||
                newItem.category != item.category)
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
                                       "item_quantity = @Quantity, " +
                                       "item_sold = @Sold, " +
                                       "item_min_quantity = @MinQuantity, " +
                                       "category_id = @CategoryID, " +
                                       "item_stock = @Stock " +
                                       "WHERE item_id = @ID";

                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Name", newItem.name);
                            command.Parameters.AddWithValue("@Description", newItem.description);
                            command.Parameters.AddWithValue("@Price", newItem.price);
                            command.Parameters.AddWithValue("@Status", newItem.status.ToString());
                            command.Parameters.AddWithValue("@Quantity", newItem.quantity);
                            command.Parameters.AddWithValue("@Sold", newItem.sold);
                            command.Parameters.AddWithValue("@MinQuantity", newItem.minQuantity);
                            command.Parameters.AddWithValue("@CategoryID", newItem.category.id);
                            command.Parameters.AddWithValue("@Stock", newItem.stock);
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
            else
            {
                Console.WriteLine("There updated value is same as current values!");
                return false;
            }
        }

        public bool UpdateQuantity(ref List<Item> items, Guid guid, int newQuantity)
        {
            var itemSearch = items.Find(item => item.id == guid);
            if (itemSearch == null)
            {
                Console.WriteLine("item not found!");
                return false;
            }
            if (itemSearch.quantity == newQuantity)
            {
                Console.WriteLine("quantity same as now!");
                return false;
            }
            if (newQuantity < itemSearch.minQuantity)
            {
                Console.WriteLine("quantity must be at least {0}", itemSearch.minQuantity);
                return false;
            }
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "UPDATE items " +
                        "SET item_quantity = @NewQuantity, " +
                        "item_sold = @SoldValue, " +
                        "item_stock = @NewQuantity " +
                        "WHERE item_id = @ItemID";
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ItemID", guid);
                        command.Parameters.AddWithValue("@NewQuantity", newQuantity);
                        command.Parameters.AddWithValue("@SoldValue", 0);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            itemSearch.quantity = newQuantity;
                            return true;
                        }
                        else return false;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public bool UpdateSoldItem(ref List<Item> items, Guid guid, int soldItems)
        {
            Item item = items.Find(itemSearch => itemSearch.id == guid);
            if (item == null)
            {
                Console.WriteLine("item not found!");
                return false;
            }
            if (soldItems > item.stock)
            {
                Console.WriteLine("There is no enough items in stock!");
                return false;
            }
            int newStock = item.quantity - soldItems;
            ItemStatus status;
            if (item.stock <= item.minQuantity) status = ItemStatus.LowStock;
            else if (item.stock > item.minQuantity) status = ItemStatus.InStock;
            else if (item.stock == 0) status = ItemStatus.OutOfStock;
            else status = ItemStatus.Unknown;

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "UPDATE items " +
                        "SET item_sold = @SoldValue, " +
                        "item_stock = @Stock, " +
                        "item_status = @Status " +
                        "WHERE item_id = @ItemID";
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ItemID", guid);
                        command.Parameters.AddWithValue("@SoldValue", soldItems);
                        command.Parameters.AddWithValue("@Stock", newStock);
                        command.Parameters.AddWithValue("@Status", status.ToString());

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            item.sold = soldItems;
                            item.stock = newStock;
                            item.status = status;
                            return true;
                        }
                        else return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public Item GetItemByName(string itemName, List<ItemCategory> categories)
        {
            Item item = null;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM items WHERE item_name = @ItemName";
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ItemName", itemName);
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string statusString = reader.GetString(reader.GetOrdinal("item_status"));
                                ItemStatus status;
                                if (!Enum.TryParse(statusString, out status))
                                {
                                    status = ItemStatus.Unknown;
                                }

                                Guid categoryID = reader.GetGuid(reader.GetOrdinal("category_id"));
                                item = new Item
                                {
                                    id = reader.GetGuid(reader.GetOrdinal("item_id")),
                                    name = reader.GetString(reader.GetOrdinal("item_name")),
                                    description = reader.GetString(reader.GetOrdinal("item_description")),
                                    price = reader.GetInt32(reader.GetOrdinal("item_price")),
                                    status = status,
                                    quantity = reader.GetInt32(reader.GetOrdinal("item_quantity")),
                                    sold = reader.GetInt32(reader.GetOrdinal("item_sold")),
                                    minQuantity = reader.GetInt32(reader.GetOrdinal("item_min_quantity")),
                                    stock = reader.GetInt32(reader.GetOrdinal("item_stock")),
                                    category = categories.Find(cat => cat.id == categoryID),
                                };
                            }
                            return item;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return item;
            }
        }

        public List<Item> GetFilterItems(string criteria, string creteriaValue)
        {
            List<Item> items = new List<Item>();
            string query;

            if (criteria == "status")
            {
                query = "SELECT * FROM items WHERE item_status = @CreteriaValue";
            }
            else if (criteria == "category")
            {
                query = "SELECT items.* " +
                    "FROM items " +
                    "JOIN categories ON items.category_id = categories.category_id " +
                    "WHERE categories.category_name = @CreteriaValue;";
            }
            else return null;

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CreteriaValue", creteriaValue);
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string statusString = reader.GetString(reader.GetOrdinal("item_status"));
                                ItemStatus status;
                                if (!Enum.TryParse(statusString, out status))
                                {
                                    status = ItemStatus.Unknown;
                                }
                                ItemCategory category = new ItemCategory
                                {
                                    id = reader.GetGuid(reader.GetOrdinal("category_id"))
                                };
                                Item tempItem = new Item
                                {
                                    id = reader.GetGuid(reader.GetOrdinal("item_id")),
                                    name = reader.GetString(reader.GetOrdinal("item_name")),
                                    description = reader.GetString(reader.GetOrdinal("item_description")),
                                    price = reader.GetInt32(reader.GetOrdinal("item_price")),
                                    status = status,
                                    quantity = reader.GetInt32(reader.GetOrdinal("item_quantity")),
                                    sold = reader.GetInt32(reader.GetOrdinal("item_sold")),
                                    minQuantity = reader.GetInt32(reader.GetOrdinal("item_min_quantity")),
                                    stock = reader.GetInt32(reader.GetOrdinal("item_stock")),
                                    category = category,

                                };
                                items.Add(tempItem);
                            }
                        }
                    }
                }

                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<Item> GetFilterItems(string creteria, int creteriaValue)
        {
            throw new NotImplementedException();
        }
    }
}
