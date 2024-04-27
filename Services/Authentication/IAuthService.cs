using InventoryManagementSystem.Models;
using System.Runtime.InteropServices;

namespace InventoryManagementSystem.Services.Authentication
{
    /// <summary>
    /// Interface for authentication service.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Hashes a password using a salt.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="currentSalt">Optional current salt.</param>
        /// <returns>A tuple containing the hashed password and the salt.</returns>
        (byte[], string) HashPasword(string password, [Optional] string currentSalt);

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="user">The user object containing registration details.</param>
        /// <returns>True if registration is successful, otherwise false.</returns>
        bool Register(User user);

        /// <summary>
        /// Authenticates a user login.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>True if login is successful, otherwise false.</returns>
        bool Login(string email, string password);

        /// <summary>
        /// Resets the password of a user.
        /// </summary>
        /// <param name="user">The user object to reset password for.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if password reset is successful, otherwise false.</returns>
        bool resetPassword(ref User user, string newPassword);

        /// <summary>
        /// Retrieves information about a user.
        /// </summary>
        /// <param name="user">The user object to populate with information.</param>
        void GetUserInfo(ref User user);
    }
}
