using InventoryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services.Orders
{
    internal interface IOrderService
    {
        bool GetOrders(ref List<Order> orders);
        bool CreateOrder(ref List<Order> orders, List<Item> orderItems);
        bool UpdateOrder();
        bool DeleteOrder();
        bool CancelOrder();
    }
}
