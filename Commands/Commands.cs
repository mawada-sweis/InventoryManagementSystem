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
    public class LoginCommand
    {
        private readonly IAuthService _authService;

        public LoginCommand(IAuthService authService)
        {
            this._authService = authService;
        }

        public void Execute(ref bool isAuthenticated, ref User user)
        {
            if (_authService.Login(user.userEmail, user.userPassword))
            {
                Console.WriteLine("Login successful");
                isAuthenticated = true;
                _authService.GetUserInfo(ref user);
            }
            else
            {
                user = null;
                Console.WriteLine("Login failed");
            }
        }
    }

    public class SignupCommand
    {
        private readonly IAuthService _authService;

        public SignupCommand(IAuthService authService)
        {
            this._authService = authService;
        }

        public void Execute(ref bool isAuthenticated, ref User user)
        {
            user.userName = GetUserInput("Username: ");
            user.userAddress = GetUserInput("Address: ");
            user.userType = GetUserInput("Are you an Admin? Y/N: ").Equals('N') ? UserType.User : UserType.Admin;

            if (_authService.Register(user) &
                    _authService.Login(user.userEmail, user.userPassword))
            {
                Console.WriteLine("Login Succesful");
                isAuthenticated = true;
                _authService.GetUserInfo(ref user);
            }
            else
            {
                user = null;
                Console.WriteLine("Email is already exist!");
            }
        }

        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }

    }

    public class ResetPassCommand
    {
        private readonly IAuthService _authService;
        public ResetPassCommand(IAuthService authService)
        {
            _authService = authService;
        }
        public void Execute(ref User user)
        {
            string newPassword = GetUserInput("New password: ");
            bool resetResult = _authService.resetPassword(ref user, newPassword);

            if (resetResult) Console.WriteLine("Password updated successfully");

            else Console.WriteLine("New password same as current one!");
        }
        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }

    }

    public class AddItemCommand
    {
        private readonly IItemService _itemService;
        private readonly ICategoriesService _categoriesService;
        public AddItemCommand(IItemService itemService, ICategoriesService categoriesService)
        {
            _itemService = itemService;
            _categoriesService = categoriesService;
        }
        public void Execute(ref List<Item> items)
        {
            Item item = new Item();
            string userInput = GetUserInput("Item name: ");
            if (items.Any(itemSearched => itemSearched.name == userInput))
            {
                Console.WriteLine("{0} item exist", userInput);
                return;
            }
            item.name = userInput;
            item.description = GetUserInput("description: ");

            string[] label = { "price", "quantity", "sold", "minQuantity" };

            for (int i = 0; i < label.Length; i++)
            {
                int temp = int.Parse(GetUserInput($"{label[i]}: "));
                while (temp < 0)
                {
                    Console.WriteLine($"{label[i]} must be greater than 0. Please enter correct value");
                    temp = int.Parse(GetUserInput($"{label[i]}: "));
                }
                PropertyInfo propertyInfo = item.GetType().GetProperty(label[i]);
                propertyInfo.SetValue(item, temp);
            }

            item.stock = item.quantity - item.sold;

            if (item.stock == 0) item.status = ItemStatus.OutOfStock;

            else if (item.stock < item.minQuantity) item.status = ItemStatus.LowStock;

            else item.status = ItemStatus.InStock;

            List<ItemCategory> categories = new List<ItemCategory>();
            _categoriesService.GetCategories(ref categories);
            var categoryStrings = categories.Select(category => category.ToString()).ToList();
            string categoriesString = string.Join(", ", categoryStrings);

            bool isExist = false;
            ItemCategory categorySearched = new ItemCategory();
            while (!isExist)
            {
                userInput = GetUserInput($"choose category: {categoriesString}: ");
                categorySearched = categories.Find(tempCategory => tempCategory.name == userInput);
                if (categorySearched != null) isExist = true;
                else Console.WriteLine("{0} category is not exist", userInput);
            }

            item.category = new ItemCategory { id = categorySearched.id, name = userInput };

            item.id = Guid.NewGuid();
            if (_itemService.AddItem(item, ref items))
                Console.WriteLine($"{item.name} addedd successfully");
            else Console.WriteLine($"{item.name} addedd faild");

        }
        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }
    }

    public class GetItemsCommand
    {
        private readonly IItemService _itemService;
        public GetItemsCommand(IItemService itemService)
        {
            _itemService = itemService;
        }
        public void Execute(ref List<Item> items)
        {
            _itemService.GetItems(ref items);
        }
    }

    public class UpdateItemCommand
    {
        private readonly IItemService _itemService;
        public UpdateItemCommand(IItemService itemService)
        {
            _itemService = itemService;
        }
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

                if (itemToUpdate.category == null)
                {
                    itemToUpdate.category = new ItemCategory { id = Guid.NewGuid(), name = "" };
                }

                if (itemToUpdate.name != updatedItem.name ||
                itemToUpdate.description != updatedItem.description ||
                itemToUpdate.price != updatedItem.price ||
                itemToUpdate.minQuantity != updatedItem.minQuantity ||
                itemToUpdate.category != updatedItem.category)
                {
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

    public class DeleteItemCommand
    {
        private readonly IItemService _itemService;
        public DeleteItemCommand(IItemService itemService)
        {
            _itemService = itemService;
        }

        public void Execute(ref List<Item> items)
        {
            string userInput = GetUserInput("What is name of item to delete? ");
            if (userInput != null)
            {
                Guid? itemID = items.Find(item => item.name == userInput)?.id;

                if (itemID == null)
                {
                    Console.WriteLine("Item not found in the list.");
                    return;
                }

                else _itemService.DeleteItem(ref items, itemID.Value);
            }
        }
        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }
    }

    public class GetCategoriesCommand
    {
        private readonly ICategoriesService _categoriesServic;
        public GetCategoriesCommand(ICategoriesService categoriesService)
        {
            this._categoriesServic = categoriesService;
        }
        public void Execute(List<ItemCategory> categories)
        {
            if (!_categoriesServic.GetCategories(ref categories))
                Console.WriteLine("No categories until now");
        }
    }

    public class AddCategoryCommand
    {
        private readonly ICategoriesService _categoriesServic;
        public AddCategoryCommand(ICategoriesService categoriesService)
        {
            this._categoriesServic = categoriesService;
        }
        public void Execute(ref List<ItemCategory> categories)
        {
            string userInput = GetUserInput("What is name of Category? ");
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
                    Console.WriteLine("{0} category name is exist.", userInput);
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

    public class UpdateCategoryCommand
    {
        private readonly ICategoriesService _categoriesServic;
        public UpdateCategoryCommand(ICategoriesService categoriesService)
        {
            _categoriesServic = categoriesService;
        }
        public void Execute(ref List<ItemCategory> categories)
        {
            string userInput = GetUserInput("What is name of category do you want to change? ");

            var category = categories.FirstOrDefault(c => c.name.Equals(userInput,
                StringComparison.OrdinalIgnoreCase));
            if (category != null)
            {
                userInput = GetUserInput("What is new name of category? ");
                if (_categoriesServic.UpdateCategory(userInput, ref category, ref categories))
                    Console.WriteLine("Categroy updated successfully");
                else Console.WriteLine("Categroy not updated!");
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

    public class DeleteCategoryCommand
    {
        private readonly ICategoriesService _categoriesServic;
        public DeleteCategoryCommand(ICategoriesService categoriesService)
        {
            _categoriesServic = categoriesService;
        }
        public void Execute(ref List<ItemCategory> categories)
        {
            string userInput = GetUserInput("What is name of category do you want to delete? ");

            ItemCategory categoryToDelete = categories.FirstOrDefault(category => category.name.Equals(userInput,
                StringComparison.OrdinalIgnoreCase));
            if (categoryToDelete != null)
            {
                if (_categoriesServic.DeleteCategory(ref categoryToDelete, ref categories))
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

    public class UpdateQuntityItemCommand
    {
        private readonly IItemService _itemService;
        public UpdateQuntityItemCommand(IItemService itemService)
        {
            _itemService = itemService;
        }
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

    public class UpdateSoldItemCommand
    {
        private readonly IItemService _itemService;
        public UpdateSoldItemCommand(IItemService itemService)
        {
            _itemService = itemService;
        }
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

    public class SearchItemByNameCommand
    {
        private readonly IItemService _itemService;
        public SearchItemByNameCommand(IItemService itemService)
        {
            _itemService = itemService;
        }
        public Item Execute(List<Item> items, List<ItemCategory> categories)
        {
            string userInput = GetUserInput("What is name of item for searching? ");
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

    public class FilterItemsByCriteriaCommand
    {
        private readonly IItemService _itemService;
        public FilterItemsByCriteriaCommand(IItemService itemService)
        {
            _itemService = itemService;
        }
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
