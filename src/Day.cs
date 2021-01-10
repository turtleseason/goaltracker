using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GoalTracker
{
    [DataContract]
    public class Day : GoalPeriod<DailyGoal>
    {
        [DataMember]
        public override DateTime Date { get => date; protected set => date = value.Date; }

        public Day(DateTime date) : base(date) { }
    }

    //// A comparer that considers two Day objects equal if they refer to the same date (month, day, and year).
    //class DayComparer : Comparer<Day>
    //{
    //    public override int Compare(Day x, Day y)
    //    {
    //        return x.Date.CompareTo(y.Date);
    //    }
    //}
}
