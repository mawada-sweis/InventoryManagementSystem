using InventoryManagementSystem.Models;
using Npgsql;
using System;
using System.Collections.Generic;
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

        public void AddItem(
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

                    if (command.ExecuteNonQuery() > 0) Console.WriteLine($"{name} addedd successfully");
                    else Console.WriteLine($"{name} addedd faild");
                }
            }
        }

        public bool DeleteItem(Guid id)
        {
            throw new NotImplementedException();
        }

        public Item GetItem()
        {
            throw new NotImplementedException();
        }

        public bool UpdateMessage()
        {
            throw new NotImplementedException();
        }
    }
}
