using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace Moss.ViewModels;
public static class DataManager
{
    static SQLiteConnection db = new SQLiteConnection("Data Source=localDB.sqlite;Version=3;");
    private static bool newDb = false;
    public static void InitializeDatabase()
    {
        if (!File.Exists("localDB.sqlite"))
        {
            newDb = true;
            SQLiteConnection.CreateFile("localDB.sqlite");
            Debug.WriteLine("Database created");
        }
        try
        {
            db.Open();
            Debug.WriteLine("Database connected");
        }
        catch (Exception e)
        {
            Debug.WriteLine("Database connect error");
        }
        try
        {
            string createTableQuery = "CREATE TABLE IF NOT EXISTS Users (UserId INTEGER PRIMARY KEY AUTOINCREMENT, UserName TEXT NOT NULL, Password TEXT NOT NULL, UserData TEXT, UserToken TEXT)";
            var createTableCommand = new SQLiteCommand(createTableQuery, db);
            createTableCommand.ExecuteNonQuery();
            Debug.WriteLine("Table created");
        }
        catch (Exception e)
        {
            Debug.WriteLine("Table not created");
        }

        if (newDb)
        {
            string query = "INSERT INTO Users (UserName, Password) VALUES (@UserName, @Password)";
            var insertUserDataCommand = new SQLiteCommand(query, db);
            insertUserDataCommand.Parameters.AddWithValue("@UserName", "begin");
            insertUserDataCommand.Parameters.AddWithValue("@Password", "motherland");
            try
            {
                insertUserDataCommand.ExecuteNonQuery();
                Debug.WriteLine("Root user Added");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Root user adding error");
            }
        }
    }

    /// <summary>
    /// Insert user into database. *root user: UserName: begin ; Password: motherland
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    public static bool InsertUser(string userName, string password)
    {
        if (UsernameExists(userName))
        {
            Debug.WriteLine("Username exists");
            return false;
        }
        string query = "INSERT INTO Users (UserName, Password) VALUES (@UserName, @Password)";
        var insertUserDataCommand = new SQLiteCommand(query, db);
        insertUserDataCommand.Parameters.AddWithValue("@UserName", userName);
        insertUserDataCommand.Parameters.AddWithValue("@Password", password);
        try
        {
            insertUserDataCommand.ExecuteNonQuery();
            Debug.WriteLine("User Added");
        }
        catch (Exception e)
        {
            Debug.WriteLine("User Adding error");
        }
        LocalTransfer.UserID = GetUserId(userName);
        return true;
        /*
        SQLiteCommand command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@UserName", userName);
        command.Parameters.AddWithValue("@Password", password);
        command.ExecuteNonQuery();*/
        /*using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Users (UserName, Password) VALUES (@UserName, @Password)";
            SQLiteCommand command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@UserName", userName);
            command.Parameters.AddWithValue("@Password", password);
            command.ExecuteNonQuery();
            connection.Close();
        }*/
    }
    public static bool UsernameExists(string userName)
    {
        string query = "SELECT EXISTS(SELECT 1 FROM Users WHERE UserName = @userName);";
        var command = new SQLiteCommand(query, db);
        command.Parameters.AddWithValue("@userName", userName);
        return Convert.ToBoolean(command.ExecuteScalar());
    }
    public static bool PasswordIsValid(string userName, string password)
    {
        string query = "SELECT EXISTS(SELECT 1 FROM Users WHERE UserName = @userName AND Password = @password)";
        var command = new SQLiteCommand(query, db);
        command.Parameters.AddWithValue("@userName", userName);
        command.Parameters.AddWithValue("@password", password);
        LocalTransfer.UserID = GetUserId(userName);
        return Convert.ToBoolean(command.ExecuteScalar());
    }
    public static bool CheckUserToken(int userId)
    {
        string query = "SELECT EXISTS(SELECT 4 FROM Users WHERE UserId = @userId AND UserToken IS NOT NULL);";
        var command = new SQLiteCommand(query, db);
        command.Parameters.AddWithValue("@userId", userId);
        return Convert.ToBoolean(command.ExecuteScalar());
    }
    public static int GetUserId(string userName)
    {
        string query = "SELECT UserId FROM users WHERE UserName = @userName";
        var command = new SQLiteCommand(query, db);
        command.Parameters.AddWithValue("@userName", userName);
        var reader = command.ExecuteReader();
        reader.Read();
        return Convert.ToInt32(reader["UserId"]);
    }
    public static string GetUserData(int userID)
    {
        string query = "SELECT UserData FROM users WHERE UserId = @userID";
        var command = new SQLiteCommand(query, db);
        command.Parameters.AddWithValue("@userID", userID);
        var reader = command.ExecuteReader();
        reader.Read();
        return String.Format($"{reader["UserData"]}");
    }
    public static bool ChangePassword(int UserId, string newPassword)
    {
        string query = "UPDATE users SET Password = @password WHERE UserId = @userID";
        var command = new SQLiteCommand(query, db);
        command.Parameters.AddWithValue("@userID", UserId);
        command.Parameters.AddWithValue("@password", newPassword);
        return Convert.ToBoolean(command.ExecuteScalar());
    }
    public static bool DeleteUser(int UserId)
    {
        string query = "DELETE FROM users WHERE UserId = @userID";
        var command = new SQLiteCommand(query, db);
        command.Parameters.AddWithValue("@userID", UserId);
        return Convert.ToBoolean(command.ExecuteScalar());
    }
    public static bool InsertUserData(int UserId, string userData)
    {
        string query = "UPDATE users SET UserData = @userData WHERE UserId = @userID";
        var command = new SQLiteCommand(query, db);
        command.Parameters.AddWithValue("@userID", UserId);
        command.Parameters.AddWithValue("@userData", userData);
        return Convert.ToBoolean(command.ExecuteScalar());
    }
    public static bool SetUserToken(int UserId, string userToken)
    {
        string query = "UPDATE users SET UserToken = @userToken WHERE UserId = @userID";
        var command = new SQLiteCommand(query, db);
        command.Parameters.AddWithValue("@userID", UserId);
        command.Parameters.AddWithValue("@userToken", userToken);
        return Convert.ToBoolean(command.ExecuteScalar());
    }
    public static string GetUserToken(int userId)
    {
        string query = "SELECT UserToken FROM users WHERE UserId = @userId";
        var command = new SQLiteCommand(query, db);
        command.Parameters.AddWithValue("@userId", userId);
        var reader = command.ExecuteReader();
        reader.Read();
        return Convert.ToString(reader["UserToken"]);
    }
    public static bool ChangeUserName(int UserId, string newUserName)
    {
        string query = "UPDATE users SET UserName = @userName WHERE UserId = @userID";
        var command = new SQLiteCommand(query, db);
        command.Parameters.AddWithValue("@userID", UserId);
        command.Parameters.AddWithValue("@userName", newUserName);
        return Convert.ToBoolean(command.ExecuteScalar());
    }
    public static string DisplayUsers()
    {
        string toReturn = "";
        string query = "SELECT * FROM Users";
        var getNames = new SQLiteCommand(query, db);
        var reader = getNames.ExecuteReader();
        while (reader.Read())
        {
            toReturn += $"Id: { reader["UserId"]}, UserName: {reader["UserName"]}, Password: {reader["Password"]}, UserData: {reader["UserData"]} ";
        }
        return toReturn;
    }
}