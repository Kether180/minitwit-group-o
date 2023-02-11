using System.Data;
using System.Data.Common;
using System;
using System.Data.SQLite;

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



}