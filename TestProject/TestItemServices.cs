﻿using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Categories;
using InventoryManagementSystem.Services.Items;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryManagementSystem.TestProject
{
    [TestFixture]
    public class TestItemService
    {
        private IItemService _itemService;
        private ICategoriesService _categoriesService;
        private string _connectionString;
        List<Item> items = new List<Item>();

        Item item = new Item
        {
            name = "Test add item",
            description = "this will test if the system add item correctly.",
            price = 20,
            status = ItemStatus.InStock,
            sold = 0,
            minQuantity = 1,
            quantity = 2,
            stock = 2
        };

        List<ItemCategory> categories = new List<ItemCategory>();

        [SetUp]
        public void SetUp()
        {
            // Set up the connection string and initialize the item and category services
            _connectionString =
                $"Host=localhost;" +
                $"Port=5432;" +
                $"Database=inventory_management;" +
                $"Username=postgres;" +
                $"Password={Environment.GetEnvironmentVariable("PG_PASSWORD")}";
            _itemService = new ItemService(_connectionString);
            _categoriesService = new CategoriesService(_connectionString);
            _categoriesService.GetCategories(ref categories);
        }

        [Test, Order(1)]
        public void TestAddItem_valid()
        {
            // Test adding a new item (valid)
            item.category = categories[0];
            item.id = Guid.NewGuid();
            Assert.That(
                _itemService.AddItem(item, ref items),
                Is.EqualTo(true));
        }

        [Test, Order(3)]
        public void TestAddItem_invalid()
        {
            // Test adding an existing item (invalid)
            item = items.FirstOrDefault(item => item.name.Trim() == "Test add item".Trim());
            Assert.That(
                _itemService.AddItem(item, ref items),
                Is.EqualTo(false));
        }

        [Test, Order(2)]
        public void TestGetItems()
        {
            // Test fetching items
            _itemService.GetItems(ref items);
            Assert.That(items.Count, Is.GreaterThan(0));
        }

        [Test, Order(4)]
        public void TestUpdateItem_valid()
        {
            // Test updating an item (valid)
            item = items.FirstOrDefault(item => item.name == "Test add item");
            Item newItem = new Item
            {
                id = item.id,
                name = "Test update item",
                description = "updated description",
                price = item.price,
                status = item.status,
                quantity = item.quantity,
                sold = item.sold,
                minQuantity = item.minQuantity,
                stock = item.stock,
                category = categories[0]
            };

            Assert.That(_itemService.UpdateItem(ref item, newItem), Is.EqualTo(true));

            int index = items.FindIndex(searchItem => searchItem.id == item.id);
            items[index] = item;
        }

        [Test, Order(5)]
        public void TestUpdateItem_invalid()
        {
            // Test updating an item (invalid)
            Assert.That(_itemService.UpdateItem(ref item, item), Is.EqualTo(false));
        }

        [Test, Order(20)]
        public void TestDeleteItem_valid()
        {
            // Test deleting an item (valid)
            item = items.FirstOrDefault(item => item.name == "Test update item");
            Assert.That(_itemService.DeleteItem(ref items, item.id), Is.EqualTo(true));
        }

        [Test, Order(21)]
        public void TestDeleteItem_invalid()
        {
            // Test deleting an item (invalid)
            Assert.That(_itemService.DeleteItem(ref items, item.id), Is.EqualTo(false));
        }

        [Test, Order(6)]
        public void TestUpdateQuantity_valid()
        {
            // Test updating item quantity (valid)
            Assert.That(_itemService.UpdateQuantity(ref items, item.id, 3), Is.EqualTo(true));
        }
        [Test, Order(7)]
        public void TestUpdateQuantity_invalid()
        {
            // Test updating item quantity (invalid)
            Assert.That(_itemService.UpdateQuantity(ref items, item.id, item.minQuantity - 1), Is.EqualTo(false));
        }

        [Test, Order(8)]
        public void TestUpdateSold_valid()
        {
            // Test updating item sold quantity (valid)
            Assert.That(_itemService.UpdateSoldItem(ref items, item.id, 1), Is.EqualTo(true));
        }

        [Test, Order(9)]
        public void TestUpdateSold_invalid()
        {
            // Test updating item sold quantity (invalid)
            Assert.That(_itemService.UpdateSoldItem(ref items, item.id, 3), Is.EqualTo(false));
        }

        [Test, Order(10)]
        public void TestSearchItem_valid()
        {
            // Test searching for an item by name (valid)
            Item itemRetrieved = _itemService.GetItemByName(item.name, categories);
            Assert.That(item, Has.Property("id").EqualTo(itemRetrieved.id)
                           & Has.Property("name").EqualTo(itemRetrieved.name)
                           & Has.Property("description").EqualTo(itemRetrieved.description)
                           & Has.Property("price").EqualTo(itemRetrieved.price)
                           & Has.Property("status").EqualTo(itemRetrieved.status)
                           & Has.Property("quantity").EqualTo(itemRetrieved.quantity)
                           & Has.Property("sold").EqualTo(itemRetrieved.sold)
                           & Has.Property("minQuantity").EqualTo(itemRetrieved.minQuantity)
                           & Has.Property("category").EqualTo(itemRetrieved.category)
                           & Has.Property("stock").EqualTo(itemRetrieved.stock)
                           );

        }
        [Test, Order(11)]
        public void TestSearchItem_invalid()
        {
            // Test searching for an item by name (invalid)
            Assert.That(_itemService.GetItemByName("hallo tester", categories), Is.EqualTo(null));
        }

        [Test, Order(12)]
        public void TestFilterStatusItem_valid()
        {
            // Test filtering items by status (valid)
            List<Item> filtereditems = _itemService.GetFilterItems("status", ItemStatus.LowStock.ToString());
            Assert.That(filtereditems.Count, Is.GreaterThan(0));
        }
        [Test, Order(13)]
        public void TestFilterStatusItem_invalid()
        {
            // Test filtering items by status (invalid)
            List<Item> filtereditems = _itemService.GetFilterItems("status", ItemStatus.OutOfStock.ToString());
            Assert.That(filtereditems.Count, Is.EqualTo(0));
        }
        [Test, Order(14)]
        public void TestFilterCategoryItem_valid()
        {
            // Test filtering items by category (valid)
            List<Item> filtereditems = _itemService.GetFilterItems("category", "Book");
            Assert.That(filtereditems.Count, Is.GreaterThan(0));
        }
        [Test, Order(15)]
        public void TestFilterCategoryItem_invalid()
        {
            // Test filtering items by category (invalid)
            List<Item> filtereditems = _itemService.GetFilterItems("category", "Tsss");
            Assert.That(filtereditems.Count, Is.EqualTo(0));
        }
        [Test, Order(16)]
        public void TestFilterPriceItem_valid()
        {
            // Test filtering items by price (valid)
            List<Item> filtereditems = _itemService.GetFilterItems("price", "1000", "<");
            Assert.That(filtereditems.Count, Is.GreaterThan(0));
        }
        [Test, Order(17)]
        public void TestFilterPriceItem_invalid()
        {
            // Test filtering items by price (invalid)
            List<Item> filtereditems = _itemService.GetFilterItems("price", "5", "!==");
            Assert.That(filtereditems, Is.Null);
        }
        [Test, Order(18)]
        public void TestFilterStockItem_valid()
        {
            // Test filtering items by stock (valid)
            List<Item> filtereditems = _itemService.GetFilterItems("stock", "4", ">=");
            Assert.That(filtereditems.Count, Is.GreaterThan(0));
        }
        [Test, Order(19)]
        public void TestFilterStockItem_invalid()
        {
            // Test filtering items by stock (invalid)
            List<Item> filtereditems = _itemService.GetFilterItems("stock", "0", "!==");
            Assert.That(filtereditems, Is.Null);
        }
    }
}
