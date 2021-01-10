
namespace GoalTracker.ViewModels
{
    class DailyGoalViewModel
    {
        private readonly DailyGoal goal;

        public DailyGoalViewModel(DailyGoal goal)
        {
            this.goal = goal;
        }

        public string Name { get => goal.Name; }

        public bool Done { get => goal.Done; set => goal.SetCompleted(value); }
    }
}
