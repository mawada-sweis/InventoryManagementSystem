using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        public void Execute(ref Item item)
        {
            item.name = GetUserInput("Item name: ");
            item.description = GetUserInput("description: ");
            if (Enum.TryParse(GetUserInput("status (InStock,OutOfStock,LowStock): "), out ItemStatus status)) 
                item.status = status;
            
            else item.status = ItemStatus.InStock;
            
            item.price = int.Parse(GetUserInput("price: "));
            item.quantity = int.Parse(GetUserInput("Quantity availible: "));
            item.sold = int.Parse(GetUserInput("Number of sold item: "));
            item.minQuantity = int.Parse(GetUserInput("Minimum quantity should have in stock: "));

            _itemService.AddItem(
                                item.name ,
                                item.description,
                                item.price,
                                item.status,
                                item.quantity,
                                item.minQuantity,
                                item.sold);
        }
        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }
    }
}
