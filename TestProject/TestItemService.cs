using InventoryManagementSystem.Services.Authentication;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.TestProject
{
    [TestFixture]
    public class TestItemService
    {
        private IAuthService _authService;
        private string _connectionString;

        [SetUp] public void SetUp()
        {
            _connectionString =
                $"Host=localhost;" +
                $"Port=5432;" +
                $"Database=inventory_management;" +
                $"Username=postgres;" +
                $"Password={Environment.GetEnvironmentVariable("PG_PASSWORD")}";
            _authService = new AuthenticationService(_connectionString);
        }
    }
}
