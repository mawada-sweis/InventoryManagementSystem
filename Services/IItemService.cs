using InventoryManagementSystem.Models;
using Npgsql.Replication.PgOutput.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public interface IItemService
    {
        Item GetItem();
        void AddItem(string name,
            string description,
            int price,
            ItemStatus status,
            int quantity,
            int minQuantity,
            int sold = 0
            );
        bool UpdateMessage();
        bool DeleteItem(Guid id);
    }
}
