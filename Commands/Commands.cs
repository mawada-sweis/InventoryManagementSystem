using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Items;
using InventoryManagementSystem.Services.Authentication;
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
            user.userEmail = GetUserInput("Email: ");
            user.userPassword = GetUserInput("Password: ");

            if (_authService.Login(user.userEmail, user.userPassword))
            {
                Console.WriteLine("Login successful");
                isAuthenticated = true;
                GetUserInfo(ref user);
            }
            else
            {
                user.userEmail = String.Empty;
                user.userPassword = String.Empty;
                Console.WriteLine("Login failed");
            }
        }

        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }

        private void GetUserInfo(ref User user)
        {
            _authService.GetUserInfo(ref user);
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
            user.userEmail = GetUserInput("Email: ");
            user.userPassword = GetUserInput("Password: ");

            if (!_authService.IsEmailExist(user.userEmail))
            {
                user.userName = GetUserInput("Username: ");
                user.userAddress = GetUserInput("Address: ");
                user.userType = GetUserInput("Are you an Admin? Y/N: ").Equals('N') ? UserType.User : UserType.Admin;

                if (_authService.Register(user.userName, user.userEmail,
                    user.userPassword, user.userAddress, user.userType) &
                    _authService.Login(user.userEmail, user.userPassword))
                {
                    Console.WriteLine("Login Succesful");
                    isAuthenticated = true;
                }
            }
            else
            {
                user.userEmail = String.Empty;
                user.userPassword = String.Empty;
                user.userName = String.Empty;
                user.userAddress = String.Empty;
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
        public void Execute(ref bool isAuthenticated, ref User user)
        {
            string newPassword = GetUserInput("New password: ");
            string resetResult = _authService.resetPassword(ref user, newPassword);

            switch (resetResult)
            {
                case "Success":
                    Console.WriteLine("Password updated successfully");
                    isAuthenticated = true;
                    break;

                case "Same":
                    Console.WriteLine("New password same as current one!");
                    break;

                case "":
                    Console.WriteLine("Update password failed");
                    break;
            }
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
            item.name = GetUserInput("Item name: ");
            item.description = GetUserInput("description: ");
            if (Enum.TryParse(GetUserInput("status (InStock,OutOfStock,LowStock): "), out ItemStatus status))
                item.status = status;

            else item.status = ItemStatus.InStock;

            item.price = int.Parse(GetUserInput("price: "));
            item.quantity = int.Parse(GetUserInput("Quantity availible: "));
            item.sold = int.Parse(GetUserInput("Number of sold item: "));
            item.minQuantity = int.Parse(GetUserInput("Minimum quantity should have in stock: "));

            if (_itemService.AddItem(
                                item.name,
                                item.description,
                                item.price,
                                item.status,
                                item.quantity,
                                item.minQuantity,
                                item.sold)) items.Add(item);

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
        public void Execute(ref List<Item> items, bool isDisplay = true)
        {
            items = _itemService.GetItems(ref items);
            if (isDisplay)
            {
                int count = 1;
                foreach (Item item in items)
                {
                    Console.WriteLine("Item {0}:", count);
                    Console.WriteLine("name: {0}", item.name);
                    Console.WriteLine("description: {0}", item.description);
                    Console.WriteLine("price: {0}", item.price);
                    Console.WriteLine("status: {0}", item.status);
                    Console.WriteLine("quantity: {0}", item.quantity);
                    Console.WriteLine("sold: {0}", item.sold);
                    Console.WriteLine("minQuantity: {0}", item.minQuantity);
                    Console.WriteLine("=================");
                    count++;
                }
            }
        }
    }

    public class UpdateItemCommand
    {
        private readonly IItemService _itemService;
        public UpdateItemCommand(IItemService itemService)
        {
            _itemService = itemService;
        }
        public void Execute(ref List<Item> items)
        {
            try
            {
                string itemName = GetUserInput("Enter the name of the item you want to update:");

                Item itemToUpdate = items.FirstOrDefault(item => item.name == itemName);
                if (itemToUpdate != null) ExecuteItemUpdate(ref itemToUpdate);
                else Console.WriteLine($"Item with name '{itemName}' not found.");

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private void ExecuteItemUpdate(ref Item itemToUpdate)
        {
            try
            {
                Item updatedItem = new Item();

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


                if (itemToUpdate.name != updatedItem.name ||
                itemToUpdate.description != updatedItem.description ||
                itemToUpdate.price != updatedItem.price ||
                itemToUpdate.status != updatedItem.status ||
                itemToUpdate.quantity != updatedItem.quantity ||
                itemToUpdate.sold != updatedItem.sold ||
                itemToUpdate.minQuantity != updatedItem.minQuantity)
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
}
