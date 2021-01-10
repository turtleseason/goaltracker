using System;
using System.Runtime.Serialization;
using static GoalTracker.Util.DateUtil;

namespace GoalTracker
{
    [DataContract]
    public class Week : GoalPeriod<WeeklyGoal>
    {
        [DataMember]
        public override DateTime Date { get => date; protected set => date = FirstDayOfWeek(value); }

        public Week(DateTime date) : base (date) { }

        //public override string ToString()
        //{
        //    string res = $"Week of {Date:MMMM dd yyyy}:\n";

        //    foreach (var g in Goals)
        //    {
        //        res += $"  {g}\n";
        //    }

        //    return res;
        //}
    }
}
