using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Authentication;
using InventoryManagementSystem.Services.Categories;
using InventoryManagementSystem.Services.Items;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public AddItemCommand(IItemService itemService)
        {
            _itemService = itemService;
        }
        public void Execute(ref List<Item> items)
        {
            Item item = new Item();
            string userInput = GetUserInput("Item name: ");
            if (!items.Any(itemSearched => itemSearched.name == userInput))
            {
                Console.WriteLine("{0} item exist");
                return;
            }
            item.name = userInput;
            item.description = GetUserInput("description: ");
            if (Enum.TryParse(GetUserInput("status (InStock,OutOfStock,LowStock): "), out ItemStatus status))
                item.status = status;

            else item.status = ItemStatus.InStock;

            item.price = int.Parse(GetUserInput("price: "));
            item.quantity = int.Parse(GetUserInput("Quantity availible: "));
            item.sold = int.Parse(GetUserInput("Number of sold item: "));
            item.minQuantity = int.Parse(GetUserInput("Minimum quantity should have in stock: "));
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
                    GetUserInput("Do you want to update the status of the item? (Y/N)").ToUpper();
                if (userInput == "Y")
                {
                    userInput = GetUserInput("Enter the new status:");
                    switch (userInput)
                    {
                        case "InStock":
                            updatedItem.status = ItemStatus.InStock;
                            break;
                        case "LowStock":
                            updatedItem.status = ItemStatus.LowStock;
                            break;
                        case "OutOfStock":
                            updatedItem.status = ItemStatus.OutOfStock;
                            break;
                        case "Unknown":
                            updatedItem.status = ItemStatus.Unknown;
                            break;
                    }
                }
                else updatedItem.status = itemToUpdate.status;

                userInput =
                    GetUserInput("Do you want to update the quantity of the item? (Y/N)").ToUpper();
                if (userInput == "Y") updatedItem.quantity = int.Parse(GetUserInput("Enter the new quantity:"));
                else updatedItem.quantity = itemToUpdate.quantity;

                userInput =
                    GetUserInput("Do you want to update the number of sold items? (Y/N)").ToUpper();
                if (userInput == "Y") updatedItem.sold = int.Parse(GetUserInput("Enter the new number:"));
                else updatedItem.sold = itemToUpdate.sold;

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
                itemToUpdate.status != updatedItem.status ||
                itemToUpdate.quantity != updatedItem.quantity ||
                itemToUpdate.sold != updatedItem.sold ||
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
            _categoriesServic.GetCategories(ref categories);
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
                    _categoriesServic.AddCategory(userInput, ref categories);
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
                _categoriesServic.UpdateCategory(userInput, ref category, ref categories);
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
                _categoriesServic.DeleteCategory(ref categoryToDelete, ref categories);
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

}
