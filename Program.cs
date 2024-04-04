using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.Commands;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace InventoryManagementSystem
{
    internal class Program
    {
        static string GetUserInput()
        {
            Console.Write("Write a command: \\api\\");
            return Console.ReadLine();
        }

        static void Main(string[] args)
        {
            bool isAuthenticated = false;
            
            string dbPassword = Environment.GetEnvironmentVariable("PG_PASSWORD");
            string connectionString = $"Host=localhost;Port=5432;Database=inventory_management;Username=postgres;Password={dbPassword}";

            User user = new User();
            IAuthService authentication = new AuthenticationService(connectionString);

            Commands.LoginCommand loginCommand = new LoginCommand(authentication);
            Commands.SignupCommand signupCommand = new SignupCommand(authentication);
            Commands.ResetPassCommand resetPassCommand = new ResetPassCommand(authentication);
            switch (GetUserInput())
            {
                case "Login": case "login":
                    loginCommand.Execute(ref isAuthenticated, ref user);
                    break;
                
                case "Signup": case "signup":
                    signupCommand.Execute(ref isAuthenticated, ref user);
                    break;
            }

            if (isAuthenticated)
            {
                string userInput = GetUserInput();
                do
                {
                    switch (userInput.Trim())
                    {
                        case "logout": break;
                        case "reset-password":
                            resetPassCommand.Execute(ref isAuthenticated, ref user);
                            break;
                    }
                } while (GetUserInput().Trim() != "logout");
            }

        }
    }
}
