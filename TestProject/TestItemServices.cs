using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Items;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.TestProject
{
    [TestFixture]
    public class TestItemService
    {
        private IItemService _itemService;
        private string _connectionString;
        List<Item> items = new List<Item>();

        Item item = new Item
        {
            name = "Test add item",
            description = "this will test if the system add item correctly.",
            price = 20,
            status = ItemStatus.InStock,
            sold = 1,
            minQuantity = 0,
            quantity = 0
        };

        Item newItem = new Item();

        [SetUp]
        public void SetUp()
        {
            _connectionString =
                $"Host=localhost;" +
                $"Port=5432;" +
                $"Database=inventory_management;" +
                $"Username=postgres;" +
                $"Password={Environment.GetEnvironmentVariable("PG_PASSWORD")}";
            _itemService = new ItemService(_connectionString);
        }

        [Test, Order(1)]
        public void TestAddItem_valid()
        {
            Assert.That(
                _itemService.AddItem(
                    "Test add item",
                    "this will test if the system add item correctly.",
                    20,
                    ItemStatus.InStock,
                    1,
                    0,
                    0),
                Is.EqualTo(true));
        }
        
        [Test, Order(2)]
        public void TestAddItem_invalid()
        {
            Assert.That(
                _itemService.AddItem(
                    item.name,
                    item.description,
                    item.price,
                    item.status,
                    item.quantity,
                    item.minQuantity,
                    item.sold
                ),
                Is.EqualTo(false));
        }
        [Test, Order(3)]
        public void TestGetItems()
        {
            _itemService.GetItems(ref items);
            Assert.That(items.Count, Is.GreaterThan(0));
        }

        /*[Test, Order(5)]
        public void TestUpdateItem_valid()
        {

            newItem = item;
            newItem.name = "Test update item";
            newItem.description = "updated description";

            Assert.That(_itemService.UpdateItem(ref item, newItem), Is.EqualTo(true));
        }*/

        [Test, Order(5)]
        public void TestUpdateItem_invalid()
        {
            Assert.That(_itemService.UpdateItem(ref item, item), Is.EqualTo(false));
        }

        [Test, Order(7)]
        public void TestDeleteItem_valid()
        {
            Guid newId = Guid.Parse("727d059b-5067-4eb4-b92f-dbfac40c7142");
            Assert.That(_itemService.DeleteItem(ref items, newId), Is.EqualTo(true));
        }
    }
}
