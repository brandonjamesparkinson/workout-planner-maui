using SQLite;
using workout_planner_maui.Models.IronLog;

namespace workout_planner_maui.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection _database;

    public DatabaseService()
    {
    }

    async Task Init()
    {
        if (_database is not null)
            return;

        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        await _database.CreateTableAsync<ExerciseLog>();
        await _database.CreateTableAsync<Settings>();
    }

    public async Task<List<ExerciseLog>> GetLogsAsync()
    {
        await Init();
        return await _database.Table<ExerciseLog>().ToListAsync();
    }

    public async Task<int> SaveLogAsync(ExerciseLog item)
    {
        await Init();
        if (item.Id != 0)
            return await _database.UpdateAsync(item);
        else
            return await _database.InsertAsync(item);
    }

    public async Task<int> DeleteLogAsync(ExerciseLog item)
    {
        await Init();
        return await _database.DeleteAsync(item);
    }
    
    // Generic settings retrieval
    public async Task<string?> GetSettingAsync(string key)
    {
        await Init();
        var setting = await _database.Table<Settings>().Where(s => s.Key == key).FirstOrDefaultAsync();
        return setting?.Value;
    }

    public async Task SaveSettingAsync(string key, string value)
    {
        await Init();
        var setting = new Settings { Key = key, Value = value };
        await _database.InsertOrReplaceAsync(setting);
    }
}

public static class Constants
{
    public const string DatabaseFilename = "IronLog.db3";

    public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

    public static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
}
