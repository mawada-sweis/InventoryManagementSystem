using InventoryManagementSystem.Commands;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Authentication;
using InventoryManagementSystem.Services.Categories;
using InventoryManagementSystem.Services.Items;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices;

namespace InventoryManagementSystem
{
    internal class Program
    {
        public static class Globals
        {
            static readonly string connectionStringTemplate = ConfigurationManager.AppSettings["ConnectionString"];
            public static readonly string ConnectionString = connectionStringTemplate.Replace("%PG_PASSWORD%", Environment.GetEnvironmentVariable("PG_PASSWORD"));

            public static readonly IAuthService authentication = new AuthenticationService(ConnectionString);
            public static readonly IItemService itemService = new ItemService(ConnectionString);
            public static readonly ICategoriesService categoriesService = new CategoriesService(ConnectionString);

            public static readonly Commands.ResetPassCommand resetPassCommand = new ResetPassCommand(authentication);
            public static Commands.GetItemsCommand getItemsCommand = new GetItemsCommand(itemService);
            public static Commands.GetCategoriesCommand getCategoriesCommand = new GetCategoriesCommand(Globals.categoriesService);
            public static Commands.SearchItemByNameCommand searchItemByNameCommand = new SearchItemByNameCommand(itemService);

            public static List<ItemCategory> categories { get; set; } = new List<ItemCategory>();
            public static List<Item> items { get; set; } = new List<Item>();
        }
        private static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Enter)
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b");
                    }
                    else
                    {
                        password += key.KeyChar;
                        Console.Write("*");
                    }
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
        static string GetUserInput([Optional] string rout)
        {
            Console.Write("Write a command: \\api\\{0}", rout);
            return Console.ReadLine().Trim().ToLower();
        }
        static void UserMenu(ref User user)
        {
            Console.Clear();
            Console.WriteLine("Welcome, {0}", user.userName);

            List<ItemCategory> Categories = Globals.categories;
            List<Item> Items = Globals.items;

            string homeRout = $"{user.userName}\\home\\";
            string userInput = GetUserInput(homeRout);

            while (true)
            {
                Globals.getCategoriesCommand.Execute(Categories);
                Globals.getItemsCommand.Execute(ref Items);

                switch (userInput)
                {
                    case "reset-password":
                        Globals.resetPassCommand.Execute(ref user);
                        break;

                    case "menu":
                        Console.WriteLine("======Menu======\n menu now empty!");
                        break;

                    case "logout":
                        return;
                }

                userInput = GetUserInput(homeRout);
            }
        }
        static void AdminMenu(ref User user)
        {
            Console.Clear();
            Console.WriteLine("Welcome, {0}", user.userName);
            Commands.AddItemCommand addItemCommand = new AddItemCommand(Globals.itemService, Globals.categoriesService);
            Commands.UpdateItemCommand updateItemCommand = new UpdateItemCommand(Globals.itemService);
            Commands.DeleteItemCommand deleteItemCommand = new DeleteItemCommand(Globals.itemService);
            Commands.AddCategoryCommand addCategoryCommand = new AddCategoryCommand(Globals.categoriesService);
            Commands.UpdateCategoryCommand updateCategoryCommand = new UpdateCategoryCommand(Globals.categoriesService);
            Commands.DeleteCategoryCommand deleteCategoryCommand = new DeleteCategoryCommand(Globals.categoriesService);
            Commands.UpdateQuntityItemCommand updateQuntityItemCommand = new UpdateQuntityItemCommand(Globals.itemService);
            Commands.UpdateSoldItemCommand updateSoldItemCommand = new UpdateSoldItemCommand(Globals.itemService);

            List<ItemCategory> Categories = Globals.categories;
            List<Item> Items = Globals.items;

            string homeRout = $"{user.userName}\\admin\\home\\";
            string userInput = GetUserInput(homeRout);
            while (true)
            {
                Globals.getCategoriesCommand.Execute(Categories);
                Globals.getItemsCommand.Execute(ref Items);

                switch (userInput)
                {
                    case "reset-password":
                        Globals.resetPassCommand.Execute(ref user);
                        break;

                    case "menu":
                        Console.WriteLine("======Menu======\n" +
                            "Add Item => AddItem\n" +
                            "Display All Items => DisplayItems\n" +
                            "Update Item by Name => UpdateItem\n" +
                            "Delete Item by Name => DeleteItem\n" +
                            "Update Quantity of specific item => UpdateQuantity\n" +
                            "Sold item => SoldItem\n" +
                            "=========================================================\n" +
                            "Display All Categories => DisplayCategories\n" +
                            "Add New Item Category => AddCategory\n" +
                            "Update Category by Name => UpdateCategory\n" +
                            "Delete Category by Name => DeleteCategory\n" +
                            "=========================================================\n" +
                            "Search item by name => SearchItem\n");
                        break;
                    case "additem":
                        addItemCommand.Execute(ref Items);
                        break;
                    case "displayitems":
                        if (Items.Count == 0)
                        {
                            Console.WriteLine("There is no items yet");
                            break;
                        }
                        Console.WriteLine("Items:");
                        int count = 1;
                        foreach (Item item in Items)
                        {
                            Console.WriteLine("Item {0}:", count);
                            Console.WriteLine("name: {0}", item.name);
                            Console.WriteLine("description: {0}", item.description);
                            Console.WriteLine("price: {0}", item.price);
                            Console.WriteLine("status: {0}", item.status);
                            Console.WriteLine("quantity: {0}", item.quantity);
                            Console.WriteLine("sold: {0}", item.sold);
                            Console.WriteLine("minimum quantity: {0}", item.minQuantity);
                            Console.WriteLine("quantity in stock: {0}", item.stock);
                            if (item.category != null) Console.WriteLine("category: {0}", item.category.name);
                            else Console.WriteLine("category: NOT CATEGORIZED");
                            Console.WriteLine("=================");
                            count++;
                        }
                        break;
                    case "updateitem":
                        updateItemCommand.Execute(ref Items, ref Categories);
                        break;
                    case "deleteitem":
                        deleteItemCommand.Execute(ref Items);
                        break;
                    case "updatequantity":
                        updateQuntityItemCommand.Execute(ref Items);
                        break;
                    case "solditem":
                        updateSoldItemCommand.Execute(ref Items);
                        break;
                    case "displaycategories":
                        if (Categories.Count == 0)
                        {
                            Console.WriteLine("There is no categories yet");
                            break;
                        }
                        Console.WriteLine("Categories:");
                        foreach (ItemCategory category in Categories)
                        {
                            Console.WriteLine("Category name: {0}", category.name);
                        }
                        break;
                    case "addcategory":
                        addCategoryCommand.Execute(ref Categories);
                        break;
                    case "updatecategory":
                        updateCategoryCommand.Execute(ref Categories);
                        break;
                    case "deletecategory":
                        deleteCategoryCommand.Execute(ref Categories);
                        break;
                    case "searchitem":
                        Item itemSearched = Globals.searchItemByNameCommand.Execute(Items);
                        if(itemSearched == null)
                        {
                            Console.WriteLine("The item is not exist!");
                        }
                        else
                        {
                            Console.WriteLine("{0} item founded! here details:", itemSearched.name);
                            Console.WriteLine("description: {0}", itemSearched.description);
                            Console.WriteLine("price: {0}", itemSearched.price);
                            Console.WriteLine("status: {0}", itemSearched.status.ToString());
                            Console.WriteLine("quantity: {0}", itemSearched.quantity);
                            Console.WriteLine("sold: {0}", itemSearched.sold);
                            Console.WriteLine("minimum quantity: {0}", itemSearched.minQuantity);
                            Console.WriteLine("quantity in stock: {0}", itemSearched.stock);
                            if (itemSearched.category != null) Console.WriteLine("category: {0}", itemSearched.category.name);
                            else Console.WriteLine("category: NOT CATEGORIZED");
                        }
                        break;
                    case "logout":
                        return;
                }
                userInput = GetUserInput(homeRout);
            }
        }
        static void Main(string[] args)
        {
            bool isAuthenticated = false;

            User user = new User();

            Commands.LoginCommand loginCommand = new LoginCommand(Globals.authentication);
            Commands.SignupCommand signupCommand = new SignupCommand(Globals.authentication);

            Console.Write("Enter email:");
            user.userEmail = Console.ReadLine().Trim().ToLower();

            Console.Write("Enter password:");
            user.userPassword = ReadPassword();

            switch (GetUserInput())
            {
                case "login":
                    loginCommand.Execute(ref isAuthenticated, ref user);
                    break;

                case "signup":
                    signupCommand.Execute(ref isAuthenticated, ref user);
                    break;
            }

            if (isAuthenticated)
            {
                switch (user.userType)
                {
                    case UserType.User:
                        UserMenu(ref user);
                        Console.WriteLine("Logout Successful");
                        break;
                    case UserType.Admin:
                        AdminMenu(ref user);
                        Console.WriteLine("Logout Successful");
                        break;
                }
            }
        }
    }
}
