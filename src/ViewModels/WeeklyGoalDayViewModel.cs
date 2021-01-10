namespace GoalTracker.ViewModels
{
    class WeeklyGoalDayViewModel
    {
        private readonly WeeklyGoal goal;

        public WeeklyGoalDayViewModel(WeeklyGoal goal, int dayOfWeek)
        {
            this.goal = goal;
            DayOfWeek = dayOfWeek;
        }

        public string Name { get => goal.Name; }

        public int DayOfWeek { get; }

        public bool Done
        {
            get => goal.DaysCompleted[DayOfWeek];
            set => goal.DaysCompleted[DayOfWeek] = value;
        }
    }
}
