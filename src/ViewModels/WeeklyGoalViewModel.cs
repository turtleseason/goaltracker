using System.Collections.Generic;
using System.ComponentModel;

namespace GoalTracker.ViewModels
{
    // Raises PropertyChanged for: Count
    class WeeklyGoalViewModel : INotifyPropertyChanged
    {
        private readonly WeeklyGoal goal;

        public WeeklyGoalViewModel(WeeklyGoal goal)
        {
            this.goal = goal;

            WeekDays = new List<WeeklyGoalDayViewModel>();
            for (int i = 0; i < goal.DaysCompleted.Count; i++)
            {
                WeekDays.Add(new WeeklyGoalDayViewModel(goal, i));
            }

            PropertyChangedEventManager.AddHandler(goal, CountChanged, nameof(WeeklyGoal.Count));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get => goal.Name; }

        public int Target { get => goal.Target; }

        public int Count { get => goal.Count; }

        public List<WeeklyGoalDayViewModel> WeekDays { get; }

        public void CountChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(WeeklyGoal.Count))
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
        }
    }
}
