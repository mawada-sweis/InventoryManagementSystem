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
        /// <summary>
        /// Provides global access to shared resources and services used throughout the program.
        /// </summary>
        public static class Globals
        {
            // Connection String for database access
            static readonly string connectionStringTemplate = ConfigurationManager.AppSettings["ConnectionString"];
            // Gets the connection string used for database access
            public static readonly string ConnectionString = connectionStringTemplate.Replace("%PG_PASSWORD%", Environment.GetEnvironmentVariable("PG_PASSWORD"));

            // Initializing services instance
            public static readonly IAuthService authentication = new AuthenticationService(ConnectionString);
            public static readonly IItemService itemService = new ItemService(ConnectionString);
            public static readonly ICategoriesService categoriesService = new CategoriesService(ConnectionString);

            // Initializing Commands
            public static readonly Commands.ResetPassCommand resetPassCommand = new ResetPassCommand(authentication);
            public static Commands.GetItemsCommand getItemsCommand = new GetItemsCommand(itemService);
            public static Commands.GetCategoriesCommand getCategoriesCommand = new GetCategoriesCommand(Globals.categoriesService);
            public static Commands.SearchItemByNameCommand searchItemByNameCommand = new SearchItemByNameCommand(itemService);
            public static Commands.FilterItemsByCriteriaCommand filterItemsByCriteriaCommand = new FilterItemsByCriteriaCommand(itemService);

            // Initializing lists needed
            public static List<ItemCategory> categories { get; set; } = new List<ItemCategory>();
            public static List<Item> items { get; set; } = new List<Item>();
        }

        /// <summary>
        /// Reads a password from the console input without echoing characters
        /// </summary>
        /// <returns>The password entered by the user</returns>
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
                        // Erase the character from console output
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

        /// <summary>
        /// Reads a command from the console input, optionally with a specified route
        /// </summary>
        /// <param name="rout">Optional. The route to append to the command prompt</param>
        /// <returns>The command entered by the user, trimmed and converted to lowercase.</returns>
        static string GetUserInput([Optional] string rout)
        {
            Console.Write("Write a command: \\api\\{0}", rout);
            return Console.ReadLine().Trim().ToLower();
        }

        /// <summary>
        /// Prints the items along with their details to the console
        /// </summary>
        /// <param name="items">The list of items to print</param>
        /// <param name="Categories">Optional. The list of item categories used to populate category names</param>
        static void PrintItems(List<Item> items, [Optional] List<ItemCategory> Categories)
        {
            // If no items match the filter criteria, then return without printing items
            if (items.Count == 0)
            {
                Console.WriteLine("The critiria is not match any item!");
                return;
            }
            if (Categories != null)
            {
                foreach (Item item in items)
                {
                    // Populate the category name for each item
                    item.category.name = Categories.Find(cat => cat.id == item.category.id).name;
                }
            }
            Console.WriteLine("Items:");
            int count = 1;
            foreach (Item item in items)
            {
                Console.WriteLine("Item {0}:", count);
                Console.WriteLine("name: {0}", item.name);
                Console.WriteLine("description: {0}", item.description);
                Console.WriteLine("price: {0}", item.price);
                Console.WriteLine("status: {0}", item.status.ToString());
                Console.WriteLine("quantity: {0}", item.quantity);
                Console.WriteLine("sold: {0}", item.sold);
                Console.WriteLine("minimum quantity: {0}", item.minQuantity);
                Console.WriteLine("quantity in stock: {0}", item.stock);
                // Check if the category is not null before printing its name
                if (item.category != null) Console.WriteLine("category: {0}", item.category.name);
                else Console.WriteLine("category: NOT CATEGORIZED");
                Console.WriteLine("=================");
                count++;
            }
        }

        /// <summary>
        /// Displays the user menu and handles user interactions
        /// </summary>
        /// <param name="user">Reference to the current user</param>
        static void UserMenu(ref User user)
        {
            // Clear the console screen and display a welcome message
            Console.Clear();
            Console.WriteLine("Welcome, {0}", user.userName);

            // Retrieve categories and items from global variables
            List<ItemCategory> Categories = Globals.categories;
            List<Item> Items = Globals.items;

            // Construct the home route based on the user's username
            string homeRout = $"{user.userName}\\home\\";

            // Get the user input initially
            string userInput = GetUserInput(homeRout);

            // Loop indefinitely until the user chooses to logout
            while (true)
            {
                // Execute commands to update categories and items
                Globals.getCategoriesCommand.Execute(Categories);
                Globals.getItemsCommand.Execute(ref Items);

                // Handle user input using a switch statement
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

                // Get the next user input
                userInput = GetUserInput(homeRout);
            }
        }

        /// <summary>
        /// Displays the admin menu and handles admin interactions
        /// </summary>
        /// <param name="user">Reference to the current user</param>
        static void AdminMenu(ref User user)
        {
            // Clear the console screen and display a welcome message
            Console.Clear();
            Console.WriteLine("Welcome, {0}", user.userName);

            // Initialize command instances for various admin operations
            Commands.AddItemCommand addItemCommand = new AddItemCommand(Globals.itemService, Globals.categoriesService);
            Commands.UpdateItemCommand updateItemCommand = new UpdateItemCommand(Globals.itemService);
            Commands.DeleteItemCommand deleteItemCommand = new DeleteItemCommand(Globals.itemService);
            Commands.AddCategoryCommand addCategoryCommand = new AddCategoryCommand(Globals.categoriesService);
            Commands.UpdateCategoryCommand updateCategoryCommand = new UpdateCategoryCommand(Globals.categoriesService);
            Commands.DeleteCategoryCommand deleteCategoryCommand = new DeleteCategoryCommand(Globals.categoriesService);
            Commands.UpdateQuantityItemCommand updateQuntityItemCommand = new UpdateQuantityItemCommand(Globals.itemService);
            Commands.UpdateSoldItemCommand updateSoldItemCommand = new UpdateSoldItemCommand(Globals.itemService);

            // Retrieve categories and items from global variables
            List<ItemCategory> Categories = Globals.categories;
            List<Item> Items = Globals.items;
            List<Item> FilteredItems = new List<Item>();

            // Construct the home route based on the user's username
            string homeRout = $"{user.userName}\\admin\\home\\";

            // Get the user input initially
            string userInput = GetUserInput(homeRout);

            // Loop indefinitely until the admin chooses to logout
            while (true)
            {
                // Execute commands to update categories and items
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
                            "Search item by name => SearchItem\n" +
                            "Filter items by status => FilterStatus\n" +
                            "Filter items by category => FilterCategory\n" +
                            "Filter items by price => FilterPrice\n" +
                            "Filter items by stock => FilterStock\n");
                        break;
                    case "additem":
                        addItemCommand.Execute(ref Items);
                        break;
                    case "displayitems":
                        PrintItems(Items);
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
                        Item itemSearched = Globals.searchItemByNameCommand.Execute(Items, Categories);
                        if (itemSearched == null)
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
                    case "filterstatus":
                        Globals.filterItemsByCriteriaCommand.Execute(ref FilteredItems, "status");
                        PrintItems(FilteredItems, Categories);
                        break;
                    case "filtercategory":
                        Globals.filterItemsByCriteriaCommand.Execute(ref FilteredItems, "category");
                        PrintItems(FilteredItems, Categories);
                        break;
                    case "filterprice":
                        userInput = GetUserInput("What is the operator criteria? ");
                        Globals.filterItemsByCriteriaCommand.Execute(ref FilteredItems, "price", userInput);
                        PrintItems(FilteredItems, Categories);
                        break;
                    case "filterstock":
                        userInput = GetUserInput("What is the operator criteria? ");
                        Globals.filterItemsByCriteriaCommand.Execute(ref FilteredItems, "stock", userInput);
                        PrintItems(FilteredItems, Categories);
                        break;
                    case "logout":
                        return;
                }
                userInput = GetUserInput(homeRout);
            }
        }

        static void Main(string[] args)
        {
            // Flag indicating whether the user is authenticated
            bool isAuthenticated = false;

            // Create a new user instance
            User user = new User();

            // Initialize command instances for login and signup
            Commands.LoginCommand loginCommand = new LoginCommand(Globals.authentication);
            Commands.SignupCommand signupCommand = new SignupCommand(Globals.authentication);

            // Prompt the user to enter email and password
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

            // If the user is authenticated, proceed to the appropriate menu
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
