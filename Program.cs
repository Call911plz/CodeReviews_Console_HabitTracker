﻿namespace CodeReviews_Console_HabitTracker;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;

class Program
{
    class LogData
    {
        public DateTime date;
        public int quantity;
        public LogData(){}
        public LogData(DateTime date, int quantity)
        {
            this.date = date;
            this.quantity = quantity;
        }
    }
    static void Main(string[] args)
    {
        string connectionString = @"Data Source=habit-Tracker.db";
        int userInput = 0;
        bool shouldContinue = true;

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            // Create table
            CreateTable(connection);

            // Do while not exit
            do
            {
                // Offer menu to user
                PrintUI();

                // Get user input
                Console.Write("\nInput: ");
                userInput = GetUserInput("Invalid Input");

                // Handle input
                switch (userInput)
                {
                    case 0:
                        LogData data = GetLogData();
                        AddLog(data, connection);
                        break;
                    case 1:
                        PrintAllLog(connection);
                        Console.WriteLine("\nPress Enter to continue");
                        Console.Read();
                        break;
                    case 2:
                        PrintAllLog(connection);
                        Console.Write("Select Id:");
                        int id = GetUserInput("Invalid Id");
                        LogData newLog = GetLogData();
                        UpdateLog(connection, id, newLog);
                        break;
                    case 3:
                        DeleteLog(connection);
                        break;
                    // Exit
                    case 4:
                        shouldContinue = false;
                        break;
                    default:
                        break;
                }
                Console.Clear();
            } while(shouldContinue);
            
            connection.Close();
        }
    }

    // Creates table if existing table does not exist
    static void CreateTable(SqliteConnection connection)
    {
        var tableCmd = connection.CreateCommand();

        tableCmd.CommandText = 
        @"CREATE TABLE IF NOT EXISTS drinking_water (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Date TEXT,
            Quantity INTEGER
            )";

        tableCmd.ExecuteNonQuery();
    }

    static void AddLog(LogData data, SqliteConnection connection)
    {
        SqliteCommand command = connection.CreateCommand();
        
        try
        {
            command.CommandText = 
            $@"INSERT 
                INTO drinking_water (Date, Quantity)
                VALUES ('{data.date.Date:d}', {data.quantity})";
        
            command.ExecuteNonQuery();
        }
        catch (Exception e) { Console.WriteLine($"{e}\nPress Enter to continue"); Console.Read(); }
    }

    static void PrintAllLog(SqliteConnection connection)
    {
        SqliteCommand command = connection.CreateCommand();

        command.CommandText = $@"SELECT * FROM drinking_water";
        ReadLogData(command);
    }

    // Reads data gotten from passed command
    static void ReadLogData(SqliteCommand command)
    {
        try
        {
            SqliteDataReader reader = command.ExecuteReader();

            if (!reader.HasRows)
                Console.WriteLine("Empty DB");
            
            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetValue(0)}\t{reader.GetValue(1)}\t{reader.GetValue(2)}");
            }
        }
        catch (Exception e) { Console.WriteLine($"{e}\nPress Enter to continue"); Console.Read(); }
    }

    static void UpdateLog(SqliteConnection connection, int id, LogData newLog)
    {
        SqliteCommand command = connection.CreateCommand();

        try
        {
            command.CommandText = $@"
            UPDATE drinking_water
            SET Date = '{newLog.date.Date:d}',
                Quantity = {newLog.quantity}
            WHERE Id = {id}";

            command.ExecuteNonQuery();
        }
        catch (Exception e) { Console.WriteLine($"{e}\nPress Enter to continue"); Console.Read(); }
            
    }

    static void DeleteLog(SqliteConnection connection)
    {
        SqliteCommand command = connection.CreateCommand();

        Console.Clear();
        Console.WriteLine("Deleting Log\n");
        PrintAllLog(connection);
        Console.Write("Input ID: ");
        int id = GetUserInput("Invalid Input");

        command.CommandText = $@"DELETE FROM drinking_water WHERE Id = {id}";
        command.ExecuteNonQuery();

        Console.WriteLine("Log Deleted. Press Enter to continue.");
        Console.Read();
        
    }

    static void PrintUI()
    {
        Console.WriteLine("Habit Log Program: ");
        Console.WriteLine("\t[0] - Create log");
        Console.WriteLine("\t[1] - Retrieve all logs");
        Console.WriteLine("\t[2] - Update log");
        Console.WriteLine("\t[3] - Delete log");
        Console.WriteLine("\t[4] - Exit");
    }

    static int GetUserInput(string errorMessage)
    {
        int parsedInt;
        while (!Int32.TryParse(Console.ReadLine(), out parsedInt))
        {
            Console.Write($"{errorMessage}: ");
        }

        return parsedInt;
    }

    static LogData GetLogData()
    {
        LogData newLog = new();

        Console.WriteLine("Adding log to DB\n");
        Console.WriteLine("Enter Date: ");

        do { newLog.date = GetDate(); } while (newLog.date == default);
        do { newLog.quantity = GetQuantity(); } while (newLog.quantity == -1);

        Console.WriteLine($"Log ({newLog.date.Date:d}, {newLog.quantity} glasses) added. Press enter to continue.");
        Console.Read();
        
        return newLog;
    }

    // Get a valid date from user
    // TODO: fancy error handling
    static DateTime GetDate()
    {
        DateTime returnDate = new();
        string date = "";

        Console.Write("Month: ");
        date += Console.ReadLine().PadLeft(2, '0') + '/';
        Console.Write("Day: ");
        date += Console.ReadLine().PadLeft(2, '0') + '/';
        Console.Write("Year: ");
        date += Console.ReadLine().PadLeft(4, '0');
        
        try { returnDate = DateTime.Parse(date); }
        catch (Exception e) { Console.WriteLine($"{e}\nPress Enter to continue"); Console.Read(); return default; }

        return returnDate;
    }

    static int GetQuantity()
    {
        int quantity = 0;

        Console.Write("Quantity (# of glasses): ");
        try { quantity = int.Parse(Console.ReadLine()); }
        catch (Exception e) { Console.Write($"{e}\nPress Enter to continue"); Console.Read(); return -1; }

        return quantity;
    }

}