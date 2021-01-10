using GoalTracker.Util;
using System;
using System.Runtime.Serialization;

namespace GoalTracker
{
    // A Goal is uniquely identified by its name and type, so two Goals with the same name and type
    // will be considered equal regardless of their other properties.
    [DataContract]
    [KnownType(typeof(DailyGoal))]
    [KnownType(typeof(WeeklyGoal))]
    public abstract class Goal : ObservableObject, IEquatable<Goal>
    {
        public enum GoalType { Daily, Weekly }


        public Goal(string name, GoalType type)
        {
            Name = name;
            Type = type;
        }


        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public GoalType Type { get; private set; }  // Todo: is this still necessary now that DailyGoal and WeeklyGoal are separate types?

        public abstract bool Done { get; }


        public Goal Copy()
        {
            return MakeCopy();
        }

        protected virtual Goal MakeCopy()
        {
            return (Goal)MemberwiseClone();
        }

        
        // Object method overrides //

        public override bool Equals(object obj)
        {
            return Equals(obj as Goal);
        }

        public bool Equals(Goal other)
        {
            return other != null
                && Name == other.Name
                && Type == other.Type;
        }

        public override int GetHashCode()
        {
            int hashCode = -243844509;
            hashCode = hashCode * -1521134295 + Name.GetHashCode();
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"Goal \"{Name}\" ({Type}): {(Done ? "Done" : "X")}";
        }
    }
}
