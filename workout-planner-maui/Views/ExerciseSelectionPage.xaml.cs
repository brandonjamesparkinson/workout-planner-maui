using workout_planner_maui.ViewModels;

namespace workout_planner_maui.Views;

public partial class ExerciseSelectionPage : ContentPage
{
	public ExerciseSelectionPage(ExerciseSelectionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
