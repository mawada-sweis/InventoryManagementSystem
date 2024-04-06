using InventoryManagementSystem.Commands;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace InventoryManagementSystem
{
    internal class Program
    {
        public static class Globals
        {
            public static string ConnectionString { get; } = $"Host=localhost;Port=5432;Database=inventory_management;Username=postgres;Password={Environment.GetEnvironmentVariable("PG_PASSWORD")}";
        }
        static string GetUserInput([Optional] string rout)
        {
            Console.Write("Write a command: \\api\\{0}", rout);
            return Console.ReadLine().Trim();
        }
        static void AccountMenu(ResetPassCommand resetPassCommand, bool isAuthenticated, ref User user, string rout, ref string userInput)
        {
            string userInputAccount = GetUserInput(rout);
            while (true)
            {
                switch (userInputAccount)
                {
                    case "return-home":
                        userInput = "home";
                        return;

                    case "logout":
                        userInput = "logout";
                        return;

                    case "reset-password":
                        resetPassCommand.Execute(ref isAuthenticated, ref user);
                        break;
                }
                userInputAccount = GetUserInput(rout);
            }
        }
        static void HomeUser(string rout, ref string userInput)
        {
            string userInputMenu = userInput;
            while (true)
            {
                switch (userInputMenu)
                {
                    case "logout":
                        userInput = "logout";
                        return;

                    case "menu":
                        Console.WriteLine("======Menu======\n menu now empty!");
                        break;
                }
                userInputMenu = GetUserInput(rout);
            }
        }
        static void HomeAdmin(string rout, ref string userInput)
        {
            IItemService itemService = new ItemService(Globals.ConnectionString);
            Commands.AddItemCommand addItemCommand = new AddItemCommand(itemService);
            Commands.GetItemsCommand getItemsCommand = new GetItemsCommand(itemService);
            Commands.UpdateItemCommand updateItemCommand = new UpdateItemCommand(itemService);
            Commands.DeleteItemCommand deleteItemCommand = new DeleteItemCommand(itemService);
            List<Item> items = new List<Item>();
            string userInputMenu = GetUserInput(rout);
            while (true)
            {
                switch (userInputMenu)
                {
                    case "logout":
                        userInput = "logout";
                        return;

                    case "menu":
                        Console.WriteLine("======Menu======\n " +
                            "Add Item => AddItem\n" +
                            "Display All Items => DisplayItems\n" +
                            "Update Item by Name => UpdateItem\n" +
                            "Delete Item by Name => DeleteItem\n");

                        break;
                    case "AddItem":
                        addItemCommand.Execute(ref items);
                        break;
                    case "DisplayItems":
                        getItemsCommand.Execute(ref items);
                        break;
                    case "UpdateItem":
                        getItemsCommand.Execute(ref items);
                        updateItemCommand.Execute(ref items);
                        break;
                    case "DeleteItem":
                        getItemsCommand.Execute(ref items);
                        deleteItemCommand.Execute(ref items);
                        break;

                }
                userInputMenu = GetUserInput(rout);
            }
        }
        static void UserMenu(ResetPassCommand resetPassCommand, bool isAuthenticated, ref User user)
        {
            Console.Clear();
            Console.WriteLine("Welcome, {0}", user.userName);

            string homeRout = $"{user.userName}\\";
            string userInput = GetUserInput(homeRout);

            while (true)
            {
                if (userInput == "account")
                    AccountMenu(resetPassCommand, isAuthenticated, ref user, $"{user.userName}\\account\\", ref userInput);

                if (userInput == "home" || userInput == "menu")
                    HomeUser(homeRout + "home\\", ref userInput);

                if (userInput == "logout")
                {
                    Console.WriteLine("Logout Successful");
                    return;
                }

                userInput = GetUserInput(homeRout);
            }
        }
        static void AdminMenu(ResetPassCommand resetPassCommand, bool isAuthenticated, ref User user)
        {
            Console.Clear();
            Console.WriteLine("Welcome, {0}", user.userName);

            string homeRout = $"{user.userName}\\admin\\";
            string userInput = GetUserInput(homeRout);

            while (true)
            {
                if (userInput == "account")
                {
                    AccountMenu(resetPassCommand, isAuthenticated, ref user, $"{user.userName}\\account\\", ref userInput);
                }
                if (userInput == "home" || userInput == "menu")
                {
                    HomeAdmin(homeRout + "home\\", ref userInput);
                }
                if (userInput == "logout")
                {
                    Console.WriteLine("Logout Successful");
                    return;
                }
                userInput = GetUserInput(homeRout);
            }
        }
        static void Main(string[] args)
        {
            bool isAuthenticated = false;

            User user = new User();
            IAuthService authentication = new AuthenticationService(Globals.ConnectionString);

            Commands.LoginCommand loginCommand = new LoginCommand(authentication);
            Commands.SignupCommand signupCommand = new SignupCommand(authentication);
            Commands.ResetPassCommand resetPassCommand = new ResetPassCommand(authentication);
            switch (GetUserInput())
            {
                case "Login":
                case "login":
                    loginCommand.Execute(ref isAuthenticated, ref user);
                    break;

                case "Signup":
                case "signup":
                    signupCommand.Execute(ref isAuthenticated, ref user);
                    break;
            }

            if (isAuthenticated)
            {
                switch (user.userType)
                {
                    case UserType.User:
                        UserMenu(resetPassCommand, isAuthenticated, ref user);
                        break;
                    case UserType.Admin:
                        AdminMenu(resetPassCommand, isAuthenticated, ref user);
                        break;
                }
            }
        }
    }
}
