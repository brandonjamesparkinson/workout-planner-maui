using Microsoft.Extensions.Logging;

namespace workout_planner_maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

// Register Services
            builder.Services.AddSingleton<Services.DatabaseService>();

            // Register ViewModels
            builder.Services.AddTransient<ViewModels.WorkoutViewModel>();

            // Register Pages
            builder.Services.AddTransient<Views.WorkoutPage>();
            builder.Services.AddTransient<Views.ExerciseSelectionPage>();

            // Register ViewModels
            builder.Services.AddTransient<ViewModels.WorkoutViewModel>();
            builder.Services.AddTransient<ViewModels.ExerciseSelectionViewModel>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            
            return builder.Build();
        }
    }
}
