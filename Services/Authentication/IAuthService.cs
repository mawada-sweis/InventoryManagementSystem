using InventoryManagementSystem.Models;
using System.Runtime.InteropServices;

namespace InventoryManagementSystem.Services.Authentication
{
    public interface IAuthService
    {
        (byte[], string) HashPasword(string password, [Optional] string currentSalt);
        bool Register(User user);
        bool Login(string email, string password);
        bool resetPassword(ref User user, string newPassword);
        void GetUserInfo(ref User user);
    }
}
