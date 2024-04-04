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
        void AddItem();
        bool UpdateMessage();
        bool DeleteItem(Guid id);
    }
}
