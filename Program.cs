using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem
{
    internal class Program
    {
        static int MainMenu()
        {
            for (int i = 0; i < 10; i++) Console.Write("*");
            Console.WriteLine("Main menu:");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Reset Password");
            return int.Parse(Console.ReadLine());
        }
        static void Main(string[] args)
        {
            string dbPassword = Environment.GetEnvironmentVariable("PG_PASSWORD");
            string connectionString = $"Host=localhost;Port=5432;Database=inventory_management;Username=postgres;Password={dbPassword}";

            var user = new User(connectionString);

            Console.Write("Enter 1 for Login, 2 for SignUp: ");
            int userInput = int.Parse(Console.ReadLine());

            Console.Write("Enter your email: ");
            string userEmail= Console.ReadLine();

            Console.Write("Enter your password: ");
            string userPassword = Console.ReadLine();

            bool isAuthenticated = false;
            switch (userInput)
            {
                case 1:
                    isAuthenticated = user.Login(userEmail, userPassword);
                    if (isAuthenticated == false)
                    {
                        Console.WriteLine("Email or password is NOT correct, recheack them!");
                    }
                    else { Console.WriteLine("Email is Authenticated"); }
                    break;
                
                case 2:
                    if(user.IsEmailExist(userEmail))
                    {
                        Console.WriteLine("Email is already exist! try to login..");
                        break;
                    }

                    Console.Write("Enter a username: ");
                    string userUsername = Console.ReadLine();

                    Console.Write("Enter your address: ");
                    string userAddress = Console.ReadLine();

                    Console.Write("Are you an Admin? Y/N: ");
                    string userType = Console.Read() == 'N' ? "user":"admin";

                    user.RegisterUser(userUsername, userEmail, userPassword, userAddress, userType);
                    user.Login(userEmail, userPassword);
                    break;
            }

            if (isAuthenticated)
            {
                do
                {
                    userInput = MainMenu();
                    switch (userInput)
                    {
                        case 0: break;
                        case 1:
                            Console.Write("Enter new password: ");
                            string newPassword = Console.ReadLine();
                            if(user.resetPassword(userEmail, newPassword)) 
                                Console.WriteLine("Password updated successfully!");
                            else Console.WriteLine("New password same as current one! try entering new one...");
                            break;
                    }
                } while (userInput != 0);
            }

        }
    }
}
