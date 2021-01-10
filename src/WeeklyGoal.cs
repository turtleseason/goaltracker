using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace GoalTracker
{
    [DataContract]
    public class WeeklyGoal : Goal
    {
        private static bool[] EmptyWeek = { false, false, false, false, false, false, false };

        
        public WeeklyGoal(string name, int requiredCount) : base (name, GoalType.Weekly)
        {
            Target = requiredCount;
            DaysCompleted = new ObservableCollection<bool>(EmptyWeek);
        }


        [DataMember]
        public int Target { get; private set; }
        
        public int Count { get => DaysCompleted.Where(x => x).Count(); }

        public override bool Done { get => Count >= Target; }

        [DataMember]
        // A goal is complete for e.g. Monday if DaysCompleted[(int)DayOfWeek.Monday] == true.
        // Access the collection by index to get/set values; don't use Add()/Remove(), and don't use an index greater than 6.
        public ObservableCollection<bool> DaysCompleted
        {
            get => daysCompleted;
            private set
            {
                if (daysCompleted != value)
                {
                    daysCompleted = value;
                    daysCompleted.CollectionChanged += DaysCompleted_CollectionChanged;
                }
            } 
        }
        private ObservableCollection<bool> daysCompleted;


        public new WeeklyGoal Copy()
        {
            return (WeeklyGoal)MakeCopy();
        }

        protected override Goal MakeCopy()
        {
            WeeklyGoal copy = (WeeklyGoal)MemberwiseClone();
            copy.DaysCompleted = new ObservableCollection<bool>(DaysCompleted);
            return copy;
        }

        private void DaysCompleted_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(DaysCompleted));
            NotifyPropertyChanged(nameof(Count));
            NotifyPropertyChanged(nameof(Done));
        }
    }
}
