using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GoalTracker
{
    // Contains all user the data that will be serialized into a save file.
    [DataContract]
    public class UserData
    {
        [DataMember]
        public List<DailyGoal> DailyGoals { get; set; } = new List<DailyGoal>();

        [DataMember]
        public List<WeeklyGoal> WeeklyGoals { get; set; } = new List<WeeklyGoal>();

        [DataMember]
        // Deleted goals for which user data still exists on one or more Days/Weeks
        public List<Goal> RemovedGoals { get; set; } = new List<Goal>();

        [DataMember]
        List<Day> serializableDays = new List<Day>();
        
        public Dictionary<DateTime, Day> Days { get; set; } = new Dictionary<DateTime, Day>();

        [DataMember]
        List<Week> serializableWeeks = new List<Week>();
        
        public Dictionary<DateTime, Week> Weeks { get; set; } = new Dictionary<DateTime, Week>();


        public List<T> GoalsList<T>(T goal) where T : Goal
        {
            if (goal is WeeklyGoal)
            {
                return WeeklyGoals as List<T>;
            }
            else
            {
                return DailyGoals as List<T>;
            }
        }

        [OnSerializing]
        private void Serialize(StreamingContext context)
        {
            serializableDays = new List<Day>();
            foreach (Day d in Days.Values)
            {
                serializableDays.Add(d);
            }

            serializableWeeks = new List<Week>();
            foreach (Week w in Weeks.Values)
            {
                serializableWeeks.Add(w);
            }
        }

        [OnDeserialized]
        private void Deserialize(StreamingContext context)
        {
            Days = new Dictionary<DateTime, Day>();
            foreach (Day d in serializableDays)
            {
                Days.Add(d.Date, d);
            }

            Weeks = new Dictionary<DateTime, Week>();
            foreach (Week w in serializableWeeks)
            {
                Weeks.Add(w.Date, w);
            }
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool printGoals)
        {
            string output = "";
            if (printGoals)
            {
                output = "Daily goals:\n";
                foreach (Goal g in DailyGoals)
                {
                    output += $" {g}\n";
                }

                output += "Weekly goals:\n";
                foreach (Goal g in WeeklyGoals)
                {
                    output += $" {g}\n";
                }

                output += "Removed goals:\n";
                foreach (Goal g in RemovedGoals)
                {
                    output += $" {g}\n";
                }
            }

            output += "Per day:\n";
            foreach (DateTime d in Days.Keys)
            {
                output += " " + Days[d];
            }

            output += "Per week:\n";
            foreach (DateTime d in Weeks.Keys)
            {
                output += " " + Weeks[d];
            }

            return output;
        }
    }
}
