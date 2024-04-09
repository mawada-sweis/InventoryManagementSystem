using InventoryManagementSystem.Models;
using Npgsql;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace InventoryManagementSystem.Services.Authentication
{
    public class AuthenticationService : IAuthService
    {
        private readonly string _connectionString;

        public AuthenticationService(string connectionString)
        {
            this._connectionString = connectionString;
        }

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
