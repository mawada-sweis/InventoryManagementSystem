using InventoryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public interface IAuthService
    {
        (byte[], string) HashPasword(string password, [Optional] string currentSalt);
        bool Register(string username, string email, string password, string address, UserType usertype = UserType.User);
        bool IsEmailExist(string email);
        bool Login(string email, string password, bool reset = false);
        string resetPassword(User user, string newPassword);
    }
}
