using InventoryManagementSystem.Models;
using System.Runtime.InteropServices;

namespace InventoryManagementSystem.Services
{
    public interface IAuthService
    {
        (byte[], string) HashPasword(string password, [Optional] string currentSalt);
        bool Register(string username, string email, string password, string address, UserType usertype = UserType.User);
        bool IsEmailExist(string email);
        bool Login(string email, string password, bool reset = false);
        string resetPassword(ref User user, string newPassword);
        void GetUserInfo(ref User user);
    }
}
