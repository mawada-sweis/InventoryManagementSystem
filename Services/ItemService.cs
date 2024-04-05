using InventoryManagementSystem.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InventoryManagementSystem.Services
{
    internal class ItemService : IItemService
    {
        private readonly string _connectionString;
        public ItemService(string connectionString) 
        {
            this._connectionString = connectionString;
        }

        public bool AddItem(
            string name, 
            string description,
            int price,
            ItemStatus status,
            int quantity,
            int minQuantity,
            int sold
            )
        {
            Guid id = Guid.NewGuid();
            
            string query = "INSERT INTO items (item_id, item_name, item_description, item_price, item_status, " +
                "item_quantity_available, item_sold, item_min_quantity) " +
                "VALUES (@id, @name, @description, @price, @status, @quantityAvailable, @sold, @minQuantity)";
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@description", description);
                    command.Parameters.AddWithValue("@price", price);
                    command.Parameters.AddWithValue("@status", status.ToString());
                    command.Parameters.AddWithValue("@quantityAvailable", quantity);
                    command.Parameters.AddWithValue("@sold", sold) ;
                    command.Parameters.AddWithValue("@minQuantity", minQuantity);

                    if (command.ExecuteNonQuery() > 0)
                    {
                        Console.WriteLine($"{name} addedd successfully");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"{name} addedd faild");
                        return false;
                    }
                }
            }
        }

        public bool DeleteItem(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<Item> GetItems(ref List<Item> items)
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
                return items;
            }
            catch (Exception ex) 
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return items;
            }
        }

        public bool UpdateMessage()
        {
            throw new NotImplementedException();
        }
    }
}
