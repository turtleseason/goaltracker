using System;
using System.Collections.Generic;
using static GoalTracker.Util.DateUtil;
using static GoalTracker.Goal;

namespace GoalTracker
{
    // Todo: split save/load methods into another class?
    public interface IGoalTrackerService
    {
        event EventHandler DaysChanged;
        event EventHandler WeeksChanged;

        bool IsUserDataLoaded { get; }
        string SaveFilePath { get; }
        List<DailyGoal> DailyGoals { get; }
        List<WeeklyGoal> WeeklyGoals { get; }

        void LoadData(UserData userData);
        UserData LoadOrCreateDataFromDefaultPath();

        void CreateDataAtPath(string filePath);
        void LoadDataFromPath(string filePath);
        void MoveSaveFilePath(string filePath);

        void Save();

        Day GetDayForDate(DateTime date);
        Week GetWeekForDate(DateTime date);

        bool HasRemovedGoal(string name, GoalType type);

        DailyGoal CreateDailyGoal(string name);
        WeeklyGoal CreateWeeklyGoal(string name, int target);

        void AddGoalToDays(DailyGoal goal, DateTime start);
        void AddGoalToWeeks(WeeklyGoal goal, DateTime start);

        void DeleteGoal(Goal goal);
        void DeleteGoalData(Goal goal, DateTime start);

        Day CreateDay(DateTime date);
        Week CreateWeek(DateTime weekStart);
    }

    class GoalTrackerService : IGoalTrackerService
    {
        private SaveDataManager saveManager;
        private UserData userData;

        
        public GoalTrackerService()
        { 
            saveManager = new SaveDataManager(); 
        }

        
        public event EventHandler DaysChanged;
        public event EventHandler WeeksChanged;

        public bool IsUserDataLoaded { get => userData != null; }
        public string SaveFilePath { get => saveManager.SaveFilePath; }
        public List<DailyGoal> DailyGoals { get => userData?.DailyGoals; }
        public List<WeeklyGoal> WeeklyGoals { get => userData?.WeeklyGoals; }

        
        // Loads UserData from the given object.
        public void LoadData(UserData userData)
        {
            SwitchProfile(userData);
        }

        // Loads UserData from the file at the default save path, or creates it if it doesn't exist.
        // Returns the resulting UserData, or null if failed to load or create data.
        public UserData LoadOrCreateDataFromDefaultPath()
        {
            UserData userData = saveManager.LoadOrCreateSaveData(saveManager.SaveFilePath);
            if (userData != null)
            {
                SwitchProfile(userData);
            }
            return userData;
        }

        // Creates and loads a new, empty UserData at the given path.
        public void CreateDataAtPath(string filePath)
        {
            UserData newUserData = saveManager.CreateSaveData(filePath);
            if (newUserData != null)
            {
                SwitchProfile(newUserData);
            }
        }

        // Loads UserData from the file at the given path.
        public void LoadDataFromPath(string filePath)
        {
            UserData newUserData = saveManager.LoadSaveData(filePath);
            if (newUserData != null)
            {
                SwitchProfile(newUserData);
            }
        }

        // Moves the loaded UserData, if any, from its current location to the given path.
        public void MoveSaveFilePath(string filePath)
        {
            if (userData != null)
            {
                saveManager.MoveSaveData(filePath);
            }
        }

        // Saves the loaded UserData to the default save file path.
        public void Save()
        {
            if (userData != null)
            {
                saveManager.SerializeData(SaveFilePath, userData);
            }
        }

        // Updates userData and invokes any necessary callbacks (could probably be replaced with a property setter?)
        protected void SwitchProfile(UserData newUserData)
        {
            userData = newUserData;
            DaysChanged?.Invoke(this, EventArgs.Empty);
            WeeksChanged?.Invoke(this, EventArgs.Empty);
        }


        public Day GetDayForDate(DateTime date)
        {
            Day day = null;
            if (IsUserDataLoaded)
            {
                userData.Days.TryGetValue(date.Date, out day);
            }
            return day;
        }

        public Week GetWeekForDate(DateTime date)
        {
            Week week = null;
            if (IsUserDataLoaded)
            {
                userData.Weeks.TryGetValue(FirstDayOfWeek(date), out week);
            }
            return week;
        }

        public bool HasRemovedGoal(string name, GoalType type)
        {
            Goal goal = type == GoalType.Daily ? (Goal)new DailyGoal(name) : (Goal)new WeeklyGoal(name, 0);
            return userData?.RemovedGoals.Contains(goal) ?? false;
        }

        public DailyGoal CreateDailyGoal(string name)
        {
            return AddGoal(new DailyGoal(name));
        }

        public WeeklyGoal CreateWeeklyGoal(string name, int target)
        {
            return AddGoal(new WeeklyGoal(name, target));
        }

        public void AddGoalToDays(DailyGoal goal, DateTime start)
        {
            foreach (Day day in userData.Days.Values)
            {
                if (day.Date >= start.Date && !day.Goals.Contains(goal))
                {
                    day.Goals.Add(goal.Copy());
                }
            }

            Save();
        }

        // The goal will be added to the week "start" is in, as well as any weeks after.
        public void AddGoalToWeeks(WeeklyGoal goal, DateTime start)
        {
            start = FirstDayOfWeek(start);
            foreach (Week week in userData.Weeks.Values)
            {
                if (week.Date >= start.Date && !week.Goals.Contains(goal))
                {
                    week.Goals.Add(goal.Copy());
                }
            }

            Save();
        }

        // Deletes a goal from the list of current goals.
        // (Does not delete any existing data related to this goal in userData.Days or userData.Weeks)
        public void DeleteGoal(Goal goal)
        {
            if (goal is WeeklyGoal)
            {
                userData.WeeklyGoals.Remove((WeeklyGoal)goal);
            }
            else
            {
                userData.DailyGoals.Remove((DailyGoal)goal);
            }

            if (HasRemainingData(goal))
            {
                userData.RemovedGoals.Add(goal);
            }

            Save();
        }

        // If goal is a WeeklyGoal, goal data will only be deleted from weeks starting exactly on or after the given start date
        // (i.e. if "start" is not exactly 0:00:00 on a Sunday, data will not be deleted from the week containing "start").
        public void DeleteGoalData(Goal goal, DateTime start)
        {
            if (goal is WeeklyGoal)
            {
                RemoveGoalFromWeeks((WeeklyGoal)goal, start);
            }
            else
            {
                RemoveGoalFromDays((DailyGoal)goal, start);
            }
        }

        public Day CreateDay(DateTime date)
        {
            if (userData.DailyGoals.Count == 0 || userData.Days.ContainsKey(date.Date))
            {
                return null;
            }

            var day = new Day(date);

            foreach (DailyGoal goal in userData.DailyGoals)
            {
                day.Goals.Add(goal.Copy());
            }

            userData.Days.Add(day.Date, day);

            DaysChanged?.Invoke(this, EventArgs.Empty);

            Save();

            return day;
        }

        public Week CreateWeek(DateTime weekStart)
        {
            Week week = new Week(weekStart);

            if (userData.WeeklyGoals.Count == 0 || userData.Weeks.ContainsKey(week.Date))
            {
                return null;
            }

            foreach (WeeklyGoal goal in userData.WeeklyGoals)
            {
                week.Goals.Add(goal.Copy());
            }

            userData.Weeks.Add(week.Date, week);

            WeeksChanged?.Invoke(this, EventArgs.Empty);

            Save();

            return week;
        }


        private T AddGoal<T>(T goal) where T : Goal
        {
            if (userData.RemovedGoals.Contains(goal))
            {
                userData.RemovedGoals.Remove(goal);
            }

            List<T> goalsList = userData.GoalsList(goal);

            if (goalsList.Contains(goal))
            {
                return null;
            }

            goalsList.Add(goal);
            Save();
            return goal;
        }

        private void RemoveGoalFromDays(DailyGoal goal, DateTime start)
        {
            List<Day> emptyDays = new List<Day>();

            foreach (Day day in userData.Days.Values)
            {
                if (day.Date >= start.Date)
                {
                    day.Goals.Remove(goal);

                    if (day.Goals.Count == 0)
                        emptyDays.Add(day);
                }
            }

            foreach (Day day in emptyDays)
            {
                RemoveDay(day);
            }

            if (!HasRemainingData(goal))
            {
                userData.RemovedGoals.Remove(goal);
            }

            Save();
        }

        // This only deletes data for weeks that start (exactly) on or after the start date;
        // if start date is not 0:00:00 on a Sunday, the earliest data to be deleted will be
        // for the week starting on the following Sunday.
        private void RemoveGoalFromWeeks(WeeklyGoal goal, DateTime start)
        {
            List<Week> empty = new List<Week>();

            foreach (Week week in userData.Weeks.Values)
            {
                if (week.Date >= start.Date)
                {
                    week.Goals.Remove(goal);

                    if (week.GoalsCount == 0)
                    {
                        empty.Add(week);
                    }
                }
            }

            foreach (Week week in empty)
            {
                RemoveWeek(week);
            }

            if (!HasRemainingData(goal))
            {
                userData.RemovedGoals.Remove(goal);
            }

            Save();
        }

        private bool HasRemainingData(Goal goal)
        {
            if (goal is WeeklyGoal)
            {
                foreach (Week week in userData.Weeks.Values)
                {
                    if (week.Goals.Contains((WeeklyGoal)goal))
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (Day day in userData.Days.Values)
                {
                    if (day.Goals.Contains((DailyGoal)goal))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void RemoveDay(Day day)
        {
            userData.Days.Remove(day.Date);
            DaysChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RemoveWeek(Week week)
        {
            userData.Weeks.Remove(week.Date);
            WeeksChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
