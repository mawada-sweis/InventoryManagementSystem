using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Authentication;
using InventoryManagementSystem.Services.Categories;
using InventoryManagementSystem.Services.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace InventoryManagementSystem.Commands
{
    /// <summary>
    /// Represents a command for user login.
    /// </summary>
    public class LoginCommand
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginCommand"/> class.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        public LoginCommand(IAuthService authService)
        {
            this._authService = authService;
        }
        /// <summary>
        /// Executes the login command.
        /// </summary>
        /// <param name="isAuthenticated">A reference to a boolean indicating whether the user is authenticated.</param>
        /// <param name="user">A reference to the user attempting to log in.</param>
        public void Execute(ref bool isAuthenticated, ref User user)
        {
            // Attempt to authenticate the user
            if (_authService.Login(user.userEmail, user.userPassword))
            {
                Console.WriteLine("Login successful");
                isAuthenticated = true;
                // Retrieve additional user information
                _authService.GetUserInfo(ref user);
            }
            else
            {
                // If authentication fails, set the user reference to null
                user = null;
                Console.WriteLine("Login failed");
            }
        }
    }

    /// <summary>
    /// Represents a command for user signup.
    /// </summary>
    public class SignupCommand
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignupCommand"/> class.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        public SignupCommand(IAuthService authService)
        {
            this._authService = authService;
        }

        /// <summary>
        /// Executes the signup command.
        /// </summary>
        /// <param name="isAuthenticated">A reference to a boolean indicating whether the user is authenticated.</param>
        /// <param name="user">A reference to the user to be signed up.</param>
        public void Execute(ref bool isAuthenticated, ref User user)
        {
            // Prompt user for signup information
            user.userName = GetUserInput("Username: ");
            user.userAddress = GetUserInput("Address: ");
            user.userType = GetUserInput("Are you an Admin? Y/N: ").Equals('N') ? UserType.User : UserType.Admin;

            // Register the user and attempt login
            if (_authService.Register(user) && _authService.Login(user.userEmail, user.userPassword))
            {
                Console.WriteLine("Login Successful");
                isAuthenticated = true;
                // Retrieve additional user information
                _authService.GetUserInfo(ref user);
            }
            else
            {
                // If registration fails, set user reference to null
                user = null;
                Console.WriteLine("Email already exists!");
            }
        }

        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }
    }

    /// <summary>
    /// Represents a command for resetting a user's password.
    /// </summary>
    public class ResetPassCommand
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetPassCommand"/> class.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        public ResetPassCommand(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Executes the password reset command.
        /// </summary>
        /// <param name="user">A reference to the user whose password is to be reset.</param>
        public void Execute(ref User user)
        {
            // Prompt user for new password
            string newPassword = GetUserInput("New password: ");

            // Attempt to reset the password
            bool resetResult = _authService.resetPassword(ref user, newPassword);

            if (resetResult) Console.WriteLine("Password updated successfully");

            else Console.WriteLine("New password is the same as the current one!");
        }

        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }
    }

    /// <summary>
    /// Represents a command for adding an item.
    /// </summary>
    public class AddItemCommand
    {
        private readonly IItemService _itemService;
        private readonly ICategoriesService _categoriesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddItemCommand"/> class.
        /// </summary>
        /// <param name="itemService">The item service.</param>
        /// <param name="categoriesService">The categories service.</param>
        public AddItemCommand(IItemService itemService, ICategoriesService categoriesService)
        {
            _itemService = itemService;
            _categoriesService = categoriesService;
        }

        /// <summary>
        /// Executes the command to add an item.
        /// </summary>
        /// <param name="items">The list of items to which the new item will be added.</param>
        public void Execute(ref List<Item> items)
        {
            Item item = new Item();

            // Prompt user for item name
            string userInput = GetUserInput("Item name: ");

            // Check if item with the same name already exists
            if (items.Any(itemSearched => itemSearched.name == userInput))
            {
                Console.WriteLine("{0} item exists", userInput);
                return;
            }

            item.name = userInput;
            item.description = GetUserInput("Description: ");

            // Prompt user for item details
            string[] labels = { "price", "quantity", "sold", "minQuantity" };

            for (int i = 0; i < labels.Length; i++)
            {
                int temp = int.Parse(GetUserInput($"{labels[i]}: "));
                while (temp < 0)
                {
                    Console.WriteLine($"{labels[i]} must be greater than 0. Please enter correct value");
                    temp = int.Parse(GetUserInput($"{labels[i]}: "));
                }
                PropertyInfo propertyInfo = item.GetType().GetProperty(labels[i]);
                propertyInfo.SetValue(item, temp);
            }

            // Calculate item stock and status
            item.stock = item.quantity - item.sold;

            if (item.stock == 0) item.status = ItemStatus.OutOfStock;
            else if (item.stock < item.minQuantity) item.status = ItemStatus.LowStock;
            else item.status = ItemStatus.InStock;

            // Get available categories
            List<ItemCategory> categories = new List<ItemCategory>();
            _categoriesService.GetCategories(ref categories);
            var categoryStrings = categories.Select(category => category.ToString()).ToList();
            string categoriesString = string.Join(", ", categoryStrings);

            bool isExist = false;
            ItemCategory categorySearched = new ItemCategory();
            while (!isExist)
            {
                userInput = GetUserInput($"Choose category: {categoriesString}: ");
                categorySearched = categories.Find(tempCategory => tempCategory.name == userInput);
                if (categorySearched != null) isExist = true;
                else Console.WriteLine("{0} category does not exist", userInput);
            }

            item.category = new ItemCategory { id = categorySearched.id, name = userInput };

            item.id = Guid.NewGuid();

            // Attempt to add the item
            if (_itemService.AddItem(item, ref items))
                Console.WriteLine($"{item.name} added successfully");
            else Console.WriteLine($"{item.name} add failed");
        }

        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }
    }

    /// <summary>
    /// Represents a command for retrieving items.
    /// </summary>
    public class GetItemsCommand
    {
        private readonly IItemService _itemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetItemsCommand"/> class.
        /// </summary>
        /// <param name="itemService">The item service.</param>
        public GetItemsCommand(IItemService itemService)
        {
            _itemService = itemService;
        }

        /// <summary>
        /// Executes the command to retrieve items.
        /// </summary>
        /// <param name="items">The list of items to be retrieved.</param>
        public void Execute(ref List<Item> items)
        {
            _itemService.GetItems(ref items);
        }
    }

    /// <summary>
    /// Represents a command for updating an item.
    /// </summary>
    public class UpdateItemCommand
    {
        private readonly IItemService _itemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateItemCommand"/> class.
        /// </summary>
        /// <param name="itemService">The item service.</param>
        public UpdateItemCommand(IItemService itemService)
        {
            _itemService = itemService;
        }

        /// <summary>
        /// Executes the command to update an item.
        /// </summary>
        /// <param name="items">The list of items.</param>
        /// <param name="categories">The list of item categories.</param>
        public void Execute(ref List<Item> items, ref List<ItemCategory> categories)
        {
            try
            {
                string itemName = GetUserInput("Enter the name of the item you want to update:");

                Item itemToUpdate = items.FirstOrDefault(item => item.name == itemName);
                if (itemToUpdate != null) ExecuteItemUpdate(ref itemToUpdate, ref categories);
                else Console.WriteLine($"Item with name '{itemName}' not found.");

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private void ExecuteItemUpdate(ref Item itemToUpdate, ref List<ItemCategory> categories)
        {
            try
            {
                Item updatedItem = new Item();

                updatedItem.id = itemToUpdate.id;
                updatedItem.quantity = itemToUpdate.quantity;
                updatedItem.status = itemToUpdate.status;
                updatedItem.sold = itemToUpdate.sold;
                updatedItem.stock = itemToUpdate.stock;

                // Prompt user for item updates
                string userInput =
                    GetUserInput("Do you want to update the name of the item? (Y/N)").ToUpper();
                if (userInput == "Y") updatedItem.name = GetUserInput("Enter the new name:");
                else updatedItem.name = itemToUpdate.name;

                userInput =
                    GetUserInput("Do you want to update the description of the item? (Y/N)").ToUpper();
                if (userInput == "Y") updatedItem.description = GetUserInput("Enter the new description:");
                else updatedItem.description = itemToUpdate.description;

                userInput =
                    GetUserInput("Do you want to update the price of the item? (Y/N)").ToUpper();
                if (userInput == "Y") updatedItem.price = int.Parse(GetUserInput("Enter the new price:"));
                else updatedItem.price = itemToUpdate.price;

                userInput =
                    GetUserInput("Do you want to update the minimum quantity of the item? (Y/N)").ToUpper();
                if (userInput == "Y") updatedItem.minQuantity = int.Parse(GetUserInput("Enter the new value:"));
                else updatedItem.minQuantity = itemToUpdate.minQuantity;

                userInput =
                    GetUserInput("Do you want to update the category of the item? (Y/N)").ToUpper();
                if (userInput == "Y")
                {
                    userInput = GetUserInput("Enter the new value:");
                    foreach (var category in categories)
                    {
                        if (category.name.Equals(userInput, StringComparison.OrdinalIgnoreCase))
                        {
                            if (updatedItem.category == null)
                                updatedItem.category = new ItemCategory { id = category.id, name = userInput };
                            else updatedItem.category.name = userInput;
                            itemToUpdate.category = new ItemCategory { id = category.id, name = "" };
                        }
                    }
                }
                else updatedItem.category = itemToUpdate.category;

                // If category is null, assign a default value
                if (itemToUpdate.category == null)
                {
                    itemToUpdate.category = new ItemCategory { id = Guid.NewGuid(), name = "" };
                }

                // Check if any changes were made to the item
                if (itemToUpdate.name != updatedItem.name ||
                itemToUpdate.description != updatedItem.description ||
                itemToUpdate.price != updatedItem.price ||
                itemToUpdate.minQuantity != updatedItem.minQuantity ||
                itemToUpdate.category != updatedItem.category)
                {
                    // Attempt to update the item
                    if (_itemService.UpdateItem(ref itemToUpdate, updatedItem))
                        Console.WriteLine("Item updated successfully!");

                    else Console.WriteLine("Failed to update the item.");
                }

                else Console.WriteLine("No changes were made to the item.");

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }
    }

    /// <summary>
    /// Represents a command for deleting an item.
    /// </summary>
    public class DeleteItemCommand
    {
        private readonly IItemService _itemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteItemCommand"/> class.
        /// </summary>
        /// <param name="itemService">The item service.</param>
        public DeleteItemCommand(IItemService itemService)
        {
            _itemService = itemService;
        }

        /// <summary>
        /// Executes the command to delete an item.
        /// </summary>
        /// <param name="items">The list of items.</param>
        public void Execute(ref List<Item> items)
        {
            string userInput = GetUserInput("What is the name of the item to delete? ");
            if (userInput != null)
            {
                // Find the ID of the item to delete
                Guid? itemID = items.Find(item => item.name == userInput)?.id;

                if (itemID == null)
                {
                    Console.WriteLine("Item not found in the list.");
                    return;
                }
                // Attempt to delete the item
                else _itemService.DeleteItem(ref items, itemID.Value);
            }
        }
        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }
    }

    /// <summary>
    /// Represents a command for retrieving categories.
    /// </summary>
    public class GetCategoriesCommand
    {
        private readonly ICategoriesService _categoriesServic;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCategoriesCommand"/> class.
        /// </summary>
        /// <param name="categoriesService">The categories service.</param>
        public GetCategoriesCommand(ICategoriesService categoriesService)
        {
            this._categoriesServic = categoriesService;
        }

        /// <summary>
        /// Executes the command to retrieve categories.
        /// </summary>
        /// <param name="categories">The list of categories to be retrieved.</param>
        public void Execute(List<ItemCategory> categories)
        {
            if (!_categoriesServic.GetCategories(ref categories))
                Console.WriteLine("No categories available.");
        }
    }

    /// <summary>
    /// Represents a command for adding a category.
    /// </summary>
    public class AddCategoryCommand
    {
        private readonly ICategoriesService _categoriesServic;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddCategoryCommand"/> class.
        /// </summary>
        /// <param name="categoriesService">The categories service.</param>
        public AddCategoryCommand(ICategoriesService categoriesService)
        {
            this._categoriesServic = categoriesService;
        }

        /// <summary>
        /// Executes the command to add a category.
        /// </summary>
        /// <param name="categories">The list of categories.</param>
        public void Execute(ref List<ItemCategory> categories)
        {
            string userInput = GetUserInput("What is the name of the category? ");
            if (userInput != null)
            {
                if (!categories.Any(category => category.name == userInput))
                {
                    ItemCategory newcategory = new ItemCategory
                    {
                        id = Guid.NewGuid(),
                        name = userInput
                    };
                    if (_categoriesServic.AddCategory(newcategory, ref categories))
                        Console.WriteLine($"Category '{userInput}' added successfully.");

                    else Console.WriteLine($"Failed to add category '{userInput}'.");
                }
                else
                {
                    Console.WriteLine("{0} category name already exist.", userInput);
                }
            }
            else Console.WriteLine("Category name must not be empty");
        }
        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }
    }

    /// <summary>
    /// Represents a command for updating a category.
    /// </summary>
    public class UpdateCategoryCommand
    {
        private readonly ICategoriesService _categoriesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCategoryCommand"/> class.
        /// </summary>
        /// <param name="categoriesService">The categories service.</param>
        public UpdateCategoryCommand(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        /// <summary>
        /// Executes the command to update a category.
        /// </summary>
        /// <param name="categories">The list of categories.</param>
        public void Execute(ref List<ItemCategory> categories)
        {
            string userInput = GetUserInput("What is the name of the category you want to change? ");

            var category = categories.FirstOrDefault(c => c.name.Equals(userInput,
                StringComparison.OrdinalIgnoreCase));
            if (category != null)
            {
                userInput = GetUserInput("What is the new name of the category? ");
                if (_categoriesService.UpdateCategory(userInput, ref category, ref categories))
                    Console.WriteLine("Category updated successfully");
                else Console.WriteLine("Category not updated!");
            }
            else
            {
                Console.WriteLine("There is no category with this name.");
            }
        }

        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }
    }

    /// <summary>
    /// Represents a command for deleting a category.
    /// </summary>
    public class DeleteCategoryCommand
    {
        private readonly ICategoriesService _categoriesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCategoryCommand"/> class.
        /// </summary>
        /// <param name="categoriesService">The categories service.</param>
        public DeleteCategoryCommand(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        /// <summary>
        /// Executes the command to delete a category.
        /// </summary>
        /// <param name="categories">The list of categories.</param>
        public void Execute(ref List<ItemCategory> categories)
        {
            string userInput = GetUserInput("What is the name of category do you want to delete? ");

            ItemCategory categoryToDelete = categories.FirstOrDefault(category => category.name.Equals(userInput,
                StringComparison.OrdinalIgnoreCase));
            if (categoryToDelete != null)
            {
                if (_categoriesService.DeleteCategory(ref categoryToDelete, ref categories))
                    Console.WriteLine("Category deleted successfully.");
                else Console.WriteLine("Category not found in the database.");
            }
            else
            {
                Console.WriteLine("There is no category with this name.");
            }
        }

        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }
    }

    /// <summary>
    /// Represents a command for updating the quantity of an item.
    /// </summary>
    public class UpdateQuantityItemCommand
    {
        private readonly IItemService _itemService;
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateQuantityItemCommand"/> class.
        /// </summary>
        /// <param name="itemService">The item service.</param>
        public UpdateQuantityItemCommand(IItemService itemService)
        {
            _itemService = itemService;
        }

        /// <summary>
        /// Executes the command to update the quantity of an item.
        /// </summary>
        /// <param name="items">The list of items.</param>
        public void Execute(ref List<Item> items)
        {
            try
            {
                string itemName = GetUserInput("Enter the name of the item you want to update:");

                Item itemToUpdate = items.FirstOrDefault(item => item.name == itemName);
                if (itemToUpdate != null)
                {
                    int itemQuantity = Int32.Parse(GetUserInput("Enter the new quantity of the item:"));
                    if (_itemService.UpdateQuantity(ref items, itemToUpdate.id, itemQuantity))
                    {
                        Console.WriteLine("Updated successfully!");
                        return;
                    }
                }
                else Console.WriteLine($"Item with name '{itemName}' not found.");

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }
    }

    /// <summary>
    /// Represents a command for updating the sold quantity of an item.
    /// </summary>
    public class UpdateSoldItemCommand
    {
        private readonly IItemService _itemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSoldItemCommand"/> class.
        /// </summary>
        /// <param name="itemService">The item service.</param>
        public UpdateSoldItemCommand(IItemService itemService)
        {
            _itemService = itemService;
        }

        /// <summary>
        /// Executes the command to update the sold quantity of an item.
        /// </summary>
        /// <param name="items">The list of items.</param>
        public void Execute(ref List<Item> items)
        {
            try
            {
                string itemName = GetUserInput("Enter the name of the item you want to update:");

                Item itemToUpdate = items.FirstOrDefault(item => item.name == itemName);
                if (itemToUpdate != null)
                {
                    int itemSolded = Int32.Parse(GetUserInput("Enter the number of sold items:"));
                    if (_itemService.UpdateSoldItem(ref items, itemToUpdate.id, itemSolded))
                    {
                        Console.WriteLine("Updated successfully!");
                        return;
                    }
                }
                else Console.WriteLine($"Item with name '{itemName}' not found.");

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }
    }

    /// <summary>
    /// Represents a command for searching an item by its name.
    /// </summary>
    public class SearchItemByNameCommand
    {
        private readonly IItemService _itemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchItemByNameCommand"/> class.
        /// </summary>
        /// <param name="itemService">The item service.</param>
        public SearchItemByNameCommand(IItemService itemService)
        {
            _itemService = itemService;
        }

        /// <summary>
        /// Executes the command to search for an item by its name.
        /// </summary>
        /// <param name="items">The list of items.</param>
        /// <param name="categories">The list of categories.</param>
        /// <returns>The found item.</returns>
        public Item Execute(List<Item> items, List<ItemCategory> categories)
        {
            string userInput = GetUserInput("What is the name of the item for searching? ");
            Item itemSearched = items.Find(item => item.name == userInput);
            Item itemRetrieved = null;
            if (itemSearched != null)
            {
                itemRetrieved = _itemService.GetItemByName(userInput, categories);
            }
            return itemRetrieved;
        }
        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }
    }

    /// <summary>
    /// Represents a command for filtering items by specified criteria.
    /// </summary>
    public class FilterItemsByCriteriaCommand
    {
        private readonly IItemService _itemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterItemsByCriteriaCommand"/> class.
        /// </summary>
        /// <param name="itemService">The item service.</param>
        public FilterItemsByCriteriaCommand(IItemService itemService)
        {
            _itemService = itemService;
        }

        /// <summary>
        /// Executes the command to filter items by the specified criteria.
        /// </summary>
        /// <param name="itemsFiltered">The list of filtered items.</param>
        /// <param name="criteria">The filtering criteria.</param>
        /// <param name="operatorString">The optional operator string.</param>
        public void Execute(ref List<Item> itemsFiltered, string criteria, [Optional] string operatorString)
        {
            itemsFiltered.Clear();
            string userInput = GetUserInput("What is the criteria value?");

            if (criteria == "status")
            {
                if (!Enum.TryParse<ItemStatus>(userInput, out ItemStatus result))
                {
                    userInput = "Unknown";
                }
            }
            itemsFiltered = _itemService.GetFilterItems(criteria, userInput, operatorString);
        }
        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }
    }
}
