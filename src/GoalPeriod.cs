using GoalTracker.Util;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace GoalTracker
{
    // GoalPeriod<T> doesn't really work as a common base class for Day and Week because of generics,
    // so this gives them a common interface (allowing for some code reuse, like in CalendarButtonViewModel).
    public interface IGoalPeriod : INotifyPropertyChanged
    {
        DateTime Date { get; }

        int GoalsCount { get; }

        int CompletedGoalsCount { get; }
    }

    [DataContract]
    public abstract class GoalPeriod<T> : ObservableObject, IGoalPeriod where T : Goal
    {
        public GoalPeriod(DateTime date)
        {
            Date = date;
            Goals = new ObservableCollection<T>();
        }


        [DataMember]
        public virtual DateTime Date { get => date; protected set => date = value; }
        protected DateTime date = new DateTime();

        [DataMember]
        public ObservableCollection<T> Goals
        {
            get => goals;
            protected set
            {
                if (goals != value)
                {
                    // This should only be assigned once, via constructor or serialization,
                    // so no need to unregister event handlers on the old value
                    Debug.Assert(goals == null);

                    goals = value;
                    goals.CollectionChanged += Goals_CollectionChanged;
                    UpdatePropertyChangedEventHandlers(goals, Goal_PropertyChanged);
                    NotifyGoalsListChanged();
                }
            }
        }
        protected ObservableCollection<T> goals;

        public int GoalsCount { get => Goals.Count; }

        public int CompletedGoalsCount { get => Goals.Where(x => x.Done).Count(); }


        protected void UpdatePropertyChangedEventHandlers(IList e, PropertyChangedEventHandler childHandler, bool remove = false)
        {
            if (e != null)
            {
                foreach (INotifyPropertyChanged child in e)
                {
                    if (remove)
                        child.PropertyChanged -= childHandler;
                    else
                        child.PropertyChanged += childHandler;
                }
            }
        }

        protected void NotifyGoalsListChanged()
        {
            NotifyPropertyChanged(nameof(Goals));
            NotifyPropertyChanged(nameof(GoalsCount));
            NotifyPropertyChanged(nameof(CompletedGoalsCount));
        }

        protected void Goals_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdatePropertyChangedEventHandlers(e.NewItems, Goal_PropertyChanged);
            UpdatePropertyChangedEventHandlers(e.OldItems, Goal_PropertyChanged, true);

            NotifyGoalsListChanged();
        }

        protected void Goal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(CompletedGoalsCount));
        }

        //public override string ToString()
        //{
        //    string res = $"{Date:MMMM dd yyyy}:\n";

        //    foreach (var g in Goals)
        //    {
        //        res += $"  {g}\n";
        //    }

        //    return res;
        //}
    }
}
