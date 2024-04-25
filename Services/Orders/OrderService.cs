using InventoryManagementSystem.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services.Orders
{
    internal class OrderService : IOrderService
    {
        private readonly string _connectionString;
        public OrderService(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public bool CancelOrder()
        {
            throw new NotImplementedException();
        }

        public bool CreateOrder(ref List<Order> orders, List<Item> orderItems)
        {
            throw new NotImplementedException();
        }
        
        public bool DeleteOrder()
        {
            throw new NotImplementedException();
        }
        
        public bool GetOrders(ref List<Order> orders)
        {
            try
            {
                //string query = "SELECT * FROM orders";
                string query = "SELECT oi.* " +
                    "FROM order_items oi " +
                    "JOIN orders o ON oi.order_id = o.order_id" +
                    "WHERE o.user_id = 'your_user_id_here';";
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string statusString = reader.GetString(reader.GetOrdinal("order_status"));
                                // Convert enum string to corresponding enum value
                                OrderStatus statusOrder;
                                if (!Enum.TryParse(statusString, out statusOrder))
                                {
                                    statusOrder = OrderStatus.Unknown;
                                }

                                Order order = new Order
                                {
                                    id = reader.GetGuid(reader.GetOrdinal("order_id")),
                                    date = DateTime.Parse(reader.GetString(reader.GetOrdinal("order_date"))),
                                    userID = reader.GetGuid(reader.GetOrdinal("user_id")),
                                    total = reader.GetInt32(reader.GetOrdinal("order_total")),
                                    status = statusOrder,
                                    //items 
                                };
                                if (!orders.Exists(i => i.id == order.id))
                                {
                                    orders.Add(order);
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
        }

        public bool UpdateOrder()
        {
            throw new NotImplementedException();
        }
    }
}
