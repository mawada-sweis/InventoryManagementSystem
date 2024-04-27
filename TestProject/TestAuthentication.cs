using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Authentication;
using NUnit.Framework;
using System;

namespace InventoryManagementSystem.TestProject
{
    [TestFixture]
    public class TestAuthentication
    {
        private IAuthService _authService;
        private string _connectionString;

        // Sample user data for testing
        User existUser = new User
        {
            userEmail = "mawada@gmail.com",
            userPassword = "123321"
        };

        User existAdmin = new User
        {
            userEmail = "sweis@gmail.com",
            userPassword = "159951"
        };
        User newUser = new User
        {
            userName = "TestUserUsername",
            userEmail = "TestRunUser@gmail.com",
            userPassword = "123",
            userAddress = "Test user address",
            userType = UserType.User,
        };
        User newAdmin = new User
        {
            userName = "TestAdminUsername",
            userEmail = "TestRunAdmin@gmail.com",
            userPassword = "123",
            userAddress = "Test Admin address",
            userType = UserType.Admin
        };

        [SetUp]
        public void Setup()
        {
            // Set up the connection string and initialize the authentication service
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
            // Test valid login credentials
            Assert.That(_authService.Login(existUser.userEmail, existUser.userPassword), Is.EqualTo(true));
            Assert.That(_authService.Login(existAdmin.userEmail, existAdmin.userPassword), Is.EqualTo(true));
        }

        [Test]
        public void TestRole()
        {
            // Test user roles after login
            _authService.GetUserInfo(ref existUser);
            Assert.That(existUser.userType, Is.EqualTo(UserType.User));
            _authService.GetUserInfo(ref existAdmin);
            Assert.That(existAdmin.userType, Is.EqualTo(UserType.Admin));
        }

        [Test, Order(2)]
        public void TestLogin_Invalid()
        {
            // Test invalid login credentials
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
            // Test valid user registration
            Assert.That(_authService.Register(newUser), Is.EqualTo(true));
            Assert.That(_authService.Register(newAdmin), Is.EqualTo(true));
        }

        [Test, Order(4)]
        public void TestSignup_Invalid()
        {
            // Test invalid user registration (duplicate)
            Assert.That(_authService.Register(newUser), Is.EqualTo(false));
            Assert.That(_authService.Register(newAdmin), Is.EqualTo(false));
        }

        [Test, Order(5)]
        public void TestRestPass_valid()
        {
            // Test valid password reset
            _authService.GetUserInfo(ref newUser);
            Assert.That(_authService.resetPassword(ref newUser, "456"), Is.EqualTo(true));

            _authService.GetUserInfo(ref newAdmin);
            Assert.That(_authService.resetPassword(ref newAdmin, "1234"), Is.EqualTo(true));
        }

        [Test, Order(6)]
        public void TestRestPass_invalid()
        {
            // Test invalid password reset (incorrect previous password)
            _authService.GetUserInfo(ref newUser);
            Assert.That(_authService.resetPassword(ref newUser, "456"), Is.EqualTo(false));

            _authService.GetUserInfo(ref newAdmin);
            Assert.That(_authService.resetPassword(ref newAdmin, "1234"), Is.EqualTo(false));
        }
    }
}
