using InventoryManagementSystem.Models;
using Npgsql;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace InventoryManagementSystem.Services.Authentication
{
    /// <summary>
    /// Service for user authentication and related operations.
    /// </summary>
    public class AuthenticationService : IAuthService
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the AuthenticationService class with the specified database connection string.
        /// </summary>
        /// <param name="connectionString">The connection string for the database.</param>
        public AuthenticationService(string connectionString)
        {
            this._connectionString = connectionString;
        }

        /// <summary>
        /// Hashes a password using a salt.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="currentSalt">Optional current salt.</param>
        /// <returns>A tuple containing the hashed password and the salt.</returns>
        public (byte[], string) HashPasword(string password, [Optional] string currentSalt)
        {
            byte[] salt;
            if (currentSalt == null)
            {
                salt = new byte[16];
                new RNGCryptoServiceProvider().GetBytes(salt);
            }
            else
            {
                salt = Convert.FromBase64String(currentSalt);
            }
            // Compute hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Combine salt and hash
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Convert to base64
            return (salt, Convert.ToBase64String(hashBytes));
        }

        /// <summary>
        /// Authenticates a user login.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>True if login is successful, otherwise false.</returns>
        public bool Login(string email, string password)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT user_password, salt FROM users WHERE user_email = @UserEmail";
                    cmd.Parameters.AddWithValue("UserEmail", email);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                string currentPassword = reader.GetString(0);
                                string currentSalt = reader.GetString(1);
                                (byte[] _, string computedHash) = HashPasword(password, currentSalt);
                                return currentPassword == computedHash;
                            }
                        }
                        else return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="user">The user object containing registration details.</param>
        /// <returns>True if registration is successful, otherwise false.</returns>
        public bool Register(User user)
        {
            (byte[] hashSalt, string hashedPassword) = HashPasword(user.userPassword);

            using (var conn = new NpgsqlConnection(this._connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                        INSERT INTO users (user_id, user_email, user_username, user_password, user_address, user_type, salt)
                        SELECT @UserID, @UserEmail, @UserName, @Password, @UserAddress, @UserType, @Salt
                        WHERE NOT EXISTS (SELECT 1 FROM users WHERE user_email = @UserEmail)
                    ";
                    cmd.Parameters.AddWithValue("UserID", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("UserEmail", user.userEmail);
                    cmd.Parameters.AddWithValue("UserName", user.userName);
                    cmd.Parameters.AddWithValue("Password", hashedPassword);
                    cmd.Parameters.AddWithValue("UserAddress", user.userAddress);
                    cmd.Parameters.AddWithValue("UserType", user.userType.ToString());
                    cmd.Parameters.AddWithValue("Salt", Convert.ToBase64String(hashSalt));

                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        Console.WriteLine("Succefully added {0}", user.userEmail);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Email {0} already exists", user.userEmail);
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Resets the password of a user.
        /// </summary>
        /// <param name="user">The user object to reset password for.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if password reset is successful, otherwise false.</returns>
        public bool resetPassword(ref User user, string newPassword)
        {
            (byte[] hashSalt, string hashedPassword) = HashPasword(newPassword, user.userSalt);
            string stringSalt = Convert.ToBase64String(hashSalt);

            if (user.userPassword == hashedPassword && user.userSalt == stringSalt) return false;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"UPDATE users
                        SET user_password = @NewPassword, salt = @Salt
                        WHERE user_email = @UserEmail
                    ";
                    cmd.Parameters.AddWithValue("NewPassword", hashedPassword);
                    cmd.Parameters.AddWithValue("UserEmail", user.userEmail);
                    cmd.Parameters.AddWithValue("Salt", stringSalt);
                    if (cmd.ExecuteNonQuery() != -1)
                    {
                        user.userPassword = hashedPassword;
                        user.userSalt = stringSalt;
                        return true;
                    }
                    else return false;
                }
            }
        }

        /// <summary>
        /// Retrieves information about a user.
        /// </summary>
        /// <param name="user">The user object to populate with information.</param>
        public void GetUserInfo(ref User user)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM users WHERE user_email = @UserEmail";
                    cmd.Parameters.AddWithValue("UserEmail", user.userEmail);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user.id = reader.GetGuid(0);
                            user.userEmail = reader.GetString(1);
                            user.userName = reader.GetString(2);
                            user.userPassword = reader.GetString(3);
                            user.userAddress = reader.GetString(4);
                            user.userType = reader.GetString(5) == "user" ? UserType.User : UserType.Admin;
                            user.userSalt = reader.GetString(6);
                        }
                    }
                }
            }
            return;
        }
    }
}
