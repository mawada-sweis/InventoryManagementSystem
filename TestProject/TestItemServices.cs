using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Authentication;
using InventoryManagementSystem.Services.Items;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Xml.Linq;

namespace InventoryManagementSystem.TestProject
{
    [TestFixture]
    public class TestItemService
    {
        private IItemService _itemService;
        private string _connectionString;

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
        public void TestItem_valid()
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
        public void TestItem_invalid()
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
                Is.EqualTo(false));
        }
    }
}
