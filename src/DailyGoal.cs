using System.Runtime.Serialization;

namespace GoalTracker
{
    [DataContract]
    public class DailyGoal : Goal
    {
        [DataMember]
        protected bool done;


        public DailyGoal(string name, bool done=false) : base(name, GoalType.Daily)
        {
            SetCompleted(done);
        }


        public override bool Done { get => done; }


        public void SetCompleted(bool isDone)
        {
            if (isDone != done)
            {
                done = isDone;
                NotifyPropertyChanged(nameof(Done));
            }
        }

        public new DailyGoal Copy()
        {
            return (DailyGoal)MakeCopy();
        }

        protected override Goal MakeCopy()
        {
            DailyGoal copy = (DailyGoal)MemberwiseClone();
            return copy;
        }
    }
}
