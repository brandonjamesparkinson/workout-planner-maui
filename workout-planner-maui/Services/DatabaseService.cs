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
        
        // Phase 1 Tables
        await _database.CreateTableAsync<ExerciseLog>();
        await _database.CreateTableAsync<Settings>();

        // Phase 2 Tables
        await _database.CreateTableAsync<ExerciseDefinition>();
        await _database.CreateTableAsync<WorkoutTemplate>();
        await _database.CreateTableAsync<WorkoutSession>();
        await _database.CreateTableAsync<SetLog>();

        await SeedDataAsync();
    }

    private async Task SeedDataAsync()
    {
        // Check if we need to seed exercises
        var count = await _database.Table<ExerciseDefinition>().CountAsync();
        if (count == 0)
        {
            var initialExercises = new List<ExerciseDefinition>
            {
                new ExerciseDefinition { Name = "SQUAT", MuscleGroup = "LEGS" },
                new ExerciseDefinition { Name = "BENCH PRESS", MuscleGroup = "CHEST" },
                new ExerciseDefinition { Name = "DEADLIFT", MuscleGroup = "BACK" },
                new ExerciseDefinition { Name = "OVERHEAD PRESS", MuscleGroup = "SHOULDERS" },
                new ExerciseDefinition { Name = "BARBELL ROW", MuscleGroup = "BACK" },
                new ExerciseDefinition { Name = "PULL UP", MuscleGroup = "BACK" },
                new ExerciseDefinition { Name = "DIP", MuscleGroup = "CHEST" },
                new ExerciseDefinition { Name = "LUNGE", MuscleGroup = "LEGS" },
                new ExerciseDefinition { Name = "CURL", MuscleGroup = "ARMS" },
                new ExerciseDefinition { Name = "EXTENSION", MuscleGroup = "ARMS" }
            };
            await _database.InsertAllAsync(initialExercises);
        }
    }

    // Phase 1 Methods (Legacy/Backup)
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

    // Phase 2 Methods
    public async Task<List<ExerciseDefinition>> GetExercisesAsync()
    {
        await Init();
        return await _database.Table<ExerciseDefinition>().OrderBy(e => e.Name).ToListAsync();
    }

    public async Task<int> SaveSessionAsync(WorkoutSession session, List<SetLog> sets)
    {
        await Init();
        
        // Save Session first to get ID
        if (session.Id != 0)
            await _database.UpdateAsync(session);
        else
            await _database.InsertAsync(session);

        // Assign SessionId to sets
        foreach (var set in sets)
        {
            set.SessionId = session.Id;
        }

        // Save Sets
        return await _database.InsertAllAsync(sets);
    }

    public async Task<List<WorkoutSession>> GetHistoryAsync()
    {
        await Init();
        return await _database.Table<WorkoutSession>().OrderByDescending(s => s.Date).ToListAsync();
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
