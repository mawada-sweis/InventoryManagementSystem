using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Categories;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.TestProject
{
    [TestFixture]
    public class TestCategoryService
    {
        private ICategoriesService _categoriesService;
        private string _connectionString;

        List<ItemCategory> categories = new List<ItemCategory>();
        ItemCategory category = new ItemCategory 
        {
            id = Guid.NewGuid(),
            name = "Test Category"
        };

        [SetUp]
        public void SetUp()
        {
            // Set up the connection string and initialize the category service
            _connectionString =
                $"Host=localhost;" +
                $"Port=5432;" +
                $"Database=inventory_management;" +
                $"Username=postgres;" +
                $"Password={Environment.GetEnvironmentVariable("PG_PASSWORD")}";
            _categoriesService = new CategoriesService(_connectionString);
        }

        [Test, Order(1)]
        public void TestGetCategories()
        {
            // Test fetching categories
            Assert.That(_categoriesService.GetCategories(ref categories), Is.EqualTo(true));
            Assert.That(categories.Count, Is.GreaterThan(0));
        }

        [Test, Order(2)]
        public void TestAddCategory_valid()
        {
            // Test adding a new category
            Assert.That(_categoriesService.AddCategory(category, ref categories), Is.EqualTo(true));
        }

        [Test, Order(3)]
        public void TestAddCategory_invalid()
        {
            // Test adding an existing category (invalid)
            Assert.That(_categoriesService.AddCategory(category, ref categories), Is.EqualTo(false));
        }

        [Test, Order(4)]
        public void TestUpdateCategory_valid()
        {
            // Test updating a category (valid)
            Assert.That(_categoriesService.UpdateCategory("new name", ref category, ref categories), Is.EqualTo(true));
            var categorySearched = categories.Find(cat => cat.name.Equals("new name"));
            Assert.That(categorySearched, Is.Not.Null);
        }

        [Test, Order(5)]
        public void TestUpdateCategory_invalid()
        {
            // Test updating a category (invalid)
            Assert.That(_categoriesService.UpdateCategory(category.name, ref category, ref categories), Is.EqualTo(false));
        }

        [Test, Order(6)]
        public void TestRemoveCategory_valid()
        {
            // Test removing a category (valid)
            Assert.That(_categoriesService.DeleteCategory(ref category, ref categories), Is.EqualTo(true));
        }
        [Test, Order(7)]
        public void TestRemoveCategory_invalid()
        {
            // Test removing a category (invalid)
            Assert.That(_categoriesService.DeleteCategory(ref category, ref categories), Is.EqualTo(false));

        }
    }
}
