using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return Console.ReadLine();
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
            return Console.ReadLine();
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
            return Console.ReadLine();
        }

    }
}
