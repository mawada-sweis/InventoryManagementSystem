using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Authentication;
using NUnit.Framework;
using System;
using System.Globalization;

namespace InventoryManagementSystem.TestProject
{
    [TestFixture]
    public class TestAuthentication
    {
        private IAuthService _authService;
        private string _connectionString;

        string validUserEmail = "mawada@gmail.com";
        string validUserPass = "123321";
        string validAdminEmail = "sweis@gmail.com";
        string validAdminPass = "159951";

        string userUsername = "TestUserUsername";
        string userEmail = "TestRunUser@gmail.com";
        string userPassword = "123";
        string userAddress = "Test user address";

        string adminUsername = "TestAdminUsername";
        string adminEmail = "TestRunAdmin@gmail.com";
        string adminPassword = "123";
        string adminAddress = "Test Admin address";

        [SetUp]
        public void Setup()
        {
            _connectionString =
                $"Host=localhost;" +
                $"Port=5432;" +
                $"Database=inventory_management;" +
                $"Username=postgres;" +
                $"Password={Environment.GetEnvironmentVariable("PG_PASSWORD")}";
            _authService = new AuthenticationService(_connectionString);
        }

        [Test, Order(1)]
        public void TestLogin_Valid()
        {
            Assert.That(_authService.Login(validUserEmail, validUserPass), Is.EqualTo(true));
            Assert.That(_authService.Login(validAdminEmail, validAdminPass), Is.EqualTo(true));
        }

        [Test, Order(2)]
        public void TestLogin_Invalid()
        {
            string invalidUserEmail = "exampleUserWrong@gmail.com";
            string invalidUserPass = "123";
            string invalidAdminEmail = "exampleAdminWrong@gmail.com";
            string invalidAdminPass = "159";

            Assert.That(_authService.Login(invalidUserEmail, invalidUserPass), Is.EqualTo(false));
            Assert.That(_authService.Login(invalidAdminEmail, invalidAdminPass), Is.EqualTo(false));
        }

        [Test, Order(3)]
        public void TestSignup_valid()
        {
            Assert.That(_authService.Register(userUsername, userEmail, userPassword, userAddress, UserType.User), Is.EqualTo(true));
            Assert.That(_authService.Register(adminUsername, adminEmail, adminPassword, adminAddress, UserType.Admin), Is.EqualTo(true));
        }

        [Test, Order(4)]
        public void TestSignup_Invalid()
        {
            Assert.That(_authService.Register(userUsername, userEmail, userPassword, userAddress, UserType.User), Is.EqualTo(false));
            Assert.That(_authService.Register(adminUsername, adminEmail, adminPassword, adminAddress, UserType.Admin), Is.EqualTo(false));
        }

        [Test, Order(5)]
        public void TestRestPass_valid()
        {

            User user = new User { userEmail = userEmail };
            User admin = new User { userEmail = adminEmail };

            _authService.GetUserInfo(ref user);
            Assert.That(_authService.resetPassword(ref user, "456"), Is.EqualTo("Success"));

            _authService.GetUserInfo(ref admin);
            Assert.That(_authService.resetPassword(ref admin, "1234"), Is.EqualTo("Success"));
        }

        [Test, Order(6)]
        public void TestRestPass_invalid()
        {

            User user = new User { userEmail = userEmail };
            User admin = new User { userEmail = adminEmail };

            _authService.GetUserInfo(ref user);
            Assert.That(_authService.resetPassword(ref user, "456"), Is.EqualTo("Same"));
            
            _authService.GetUserInfo(ref admin);
            Assert.That(_authService.resetPassword(ref admin, "1234"), Is.EqualTo("Same"));
        }
    }
}
