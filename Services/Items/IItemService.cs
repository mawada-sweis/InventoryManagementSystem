using InventoryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace InventoryManagementSystem.Services.Items
{
    /// <summary>
    /// Service for managing items.
    /// </summary>
    public interface IItemService
    {
        /// <summary>
        /// Retrieves all items from the database.
        /// </summary>
        /// <param name="items">The list to populate with items.</param>
        void GetItems(ref List<Item> items);

        /// <summary>
        /// Adds a new item to the database.
        /// </summary>
        /// <param name="newItem">The item to add.</param>
        /// <param name="items">The list of items to update after adding.</param>
        /// <returns>True if the item is successfully added, otherwise false.</returns>
        bool AddItem(Item newItem, ref List<Item> items);

        /// <summary>
        /// Updates an existing item in the database.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <param name="newItem">The updated item.</param>
        /// <returns>True if the item is successfully updated, otherwise false.</returns>
        bool UpdateItem(ref Item item, Item newItem);

        /// <summary>
        /// Deletes an item from the database.
        /// </summary>
        /// <param name="items">The list of items to delete from.</param>
        /// <param name="guid">The ID of the item to delete.</param>
        /// <returns>True if the item is successfully deleted, otherwise false.</returns>
        bool DeleteItem(ref List<Item> items, Guid guid);

        /// <summary>
        /// Updates the quantity of an item in the database.
        /// </summary>
        /// <param name="items">The list of items to update.</param>
        /// <param name="guid">The ID of the item to update.</param>
        /// <param name="newQuantity">The new quantity value.</param>
        /// <returns>True if the quantity is successfully updated, otherwise false.</returns>
        bool UpdateQuantity(ref List<Item> items, Guid guid, int newQuantity);

        /// <summary>
        /// Updates the sold quantity of an item in the database.
        /// </summary>
        /// <param name="items">The list of items to update.</param>
        /// <param name="guid">The ID of the item to update.</param>
        /// <param name="sold">The quantity sold.</param>
        /// <returns>True if the sold quantity is successfully updated, otherwise false.</returns>
        bool UpdateSoldItem(ref List<Item> items, Guid guid, int sold);

        /// <summary>
        /// Retrieves an item by its name from the database.
        /// </summary>
        /// <param name="itemName">The name of the item to retrieve.</param>
        /// <param name="categories">The list of categories used for reference.</param>
        /// <returns>The item if found, otherwise null.</returns>
        Item GetItemByName(string itemName, List<ItemCategory> categories);

        /// <summary>
        /// Filters items based on a given criteria and value.
        /// </summary>
        /// <param name="criteria">The criteria for filtering (e.g., status, category, price, stock).</param>
        /// <param name="criteriaValue">The value to filter by.</param>
        /// <param name="operatorString">The operator to use for comparison (e.g., >, <, >=, <=, !=).</param>
        /// <returns>The list of filtered items.</returns>
        List<Item> GetFilterItems(string criteria, string criteriaValue, [Optional] string operatorString);
    }
}
