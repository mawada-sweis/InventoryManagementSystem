using Npgsql;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace InventoryManagementSystem
{
    public class User
    {
        private readonly string _connectionString;
        public string _id { get; set; }

        public User(string connectionString)
        {
            this._connectionString = connectionString;
        }

        private (byte[], string) HashPasword(string password, [ Optional ] string currentSalt)
        {
            byte[] salt;
            if (currentSalt==null) 
            { 
                salt = new byte[16];
                new RNGCryptoServiceProvider().GetBytes(salt);
            }
            else {
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
        
        public bool IsEmailExist(string email)
        {
            bool isExist = false;
            using (var conn = new NpgsqlConnection(this._connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT COUNT(*) FROM users WHERE user_email = @UserEmail";
                    cmd.Parameters.AddWithValue("UserEmail", email);

                    object result = cmd.ExecuteScalar();
                    if (result != null) if (Convert.ToInt32(result) > 0) isExist = true;
                }
            }
            return isExist;
        }


        public bool RegisterUser(string username, string email,
                                string password, string address, string usertype = "user")
        {
            if (IsEmailExist(email)) return false;
            (byte[] hashSalt, string hashedPassword) = HashPasword(password);
            
            using (var conn = new NpgsqlConnection(this._connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO users (user_id, user_email, user_username, user_password, user_address, user_type, salt) " +
                        "VALUES (@UserID, @UserEmail, @UserName, @Password, @UserAddress, @UserType, @Salt)" +
                        "ON CONFLICT (user_email) DO NOTHING";
                    cmd.Parameters.AddWithValue("UserID", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("UserEmail", email);
                    cmd.Parameters.AddWithValue("UserName", username);
                    cmd.Parameters.AddWithValue("Password", hashedPassword);
                    cmd.Parameters.AddWithValue("UserAddress", address);
                    cmd.Parameters.AddWithValue("UserType", usertype);
                    cmd.Parameters.AddWithValue("Salt", Convert.ToBase64String(hashSalt));
                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine("Succefully added {0}", email);
                }
                conn.Close();
            }
            return true;
        }


        public bool Login(string email, string password, bool reset=false)
        {
            if(!reset) if (IsEmailExist(email) == false) return false;
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
                        if (reader.Read())
                        {
                            string currentPassword = reader.GetString(0);
                            string currentSalt = reader.GetString(1);

                            (byte[] _salt, string computedHash) = HashPasword(password, currentSalt);


                            return currentPassword == computedHash;
                        }
                    }
                }
            }
            return false;
        }

        public bool resetPassword(string email, string newPassword)
        {
            if (Login(email, newPassword, true)) return false;

            (byte[] hashSalt, string hashedPassword) = HashPasword(newPassword);

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE users " +
                        "SET user_password = @NewPassword, salt = @Salt " +
                        "WHERE user_email = @UserEmail";
                    cmd.Parameters.AddWithValue("NewPassword", hashedPassword);
                    cmd.Parameters.AddWithValue("UserEmail", email);
                    cmd.Parameters.AddWithValue("Salt", Convert.ToBase64String(hashSalt));
                    return cmd.ExecuteNonQuery()!=-1;
                }
            }
        }
    }
}
