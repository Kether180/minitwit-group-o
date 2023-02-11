using System.Data;
using System.Data.Common;
using System;
using System.Data.SQLite;
using System.Data.SqlClient;

namespace Database;


public class Database
{
    private SQLiteConnection Db;

    private string DATABASE = "/tmp/minitwit.db";
    //private int PER_PAGE = 30;  // user timeline 
    //private bool DEBUG = true;
    //private string SECRET_KEY = "development key";


    public Database()
    {
        Db = new SQLiteConnection("Data Source=" + DATABASE + ";Version=3;New=True;Compress=True;");
    }

    public void ConnectToDb() // Connect to the database 
    {
        try
        {
            Db.Open();
            Console.WriteLine("Successfully connected to the database");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    public void InitializeDb() // Initialize the database and Creates the database tables 

    {
        try
        {
            Db.Open();
            using (var streamReader = new StreamReader("schema.sql"))
            {
                var sqlScript = streamReader.ReadToEnd();
                SQLiteCommand cmd = new SQLiteCommand(sqlScript, Db);
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Database initialized successfully");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
        finally
        {
            Db.Close();
        }
    }

    public List<Dictionary<string, object>> QueryDb(string query, List<SQLiteParameter> parameters)

    // Queries the database and returns a list of dictionaries
    {
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

        using (SQLiteCommand cmd = new SQLiteCommand(query, Db))
        {
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters.ToArray());
            }

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader.GetName(i), reader.GetValue(i));
                    }
                    rows.Add(row);
                }
            }
        }

        return rows;
    }


    public int GetUserId(string username) // convenience method to look up the id for a username.
    {
        int userId = 0;

        using (SQLiteCommand cmd = new SQLiteCommand("select user_id from user where username = @username", Db))
        {
            cmd.Parameters.AddWithValue("@username", username);

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    userId = reader.GetInt32(0);
                }
            }
        }

        return userId;
    }


    public string FormatDatetime(int timestamp) // Format a timestamp for display.
    {
        return DateTime.FromFileTimeUtc(timestamp).ToString("yyyy-MM-dd @ HH:mm");
    }

}


// no clue if we are using the md5 library -- ? 
/*
    public static string GravatarUrl(string email, int size = 80)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            var emailBytes = System.Text.Encoding.UTF8.GetBytes(email.Trim().ToLower());
            var hashBytes = md5.ComputeHash(emailBytes);
            var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
            return $"http://www.gravatar.com/avatar/{hash}?d=identicon&s={size}";
        }
    }
}

*/
/*

    public string GravatarUrl(string email, int size = 80)
    {
        using (var md5 = MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(email.Trim().ToLower());
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }

            return $"http://www.gravatar.com/avatar/{sb.ToString().ToLower()}?d=identicon&s={size}";
        }
    }
}

*/