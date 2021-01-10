// This DelegateCommand implementation is taken from the code for this Intro to MVVM webinar by Miguel Castro/DevExpress:
// https://www.youtube.com/watch?v=tKfpvs7ZIyo

using System;
using System.Windows.Input;

namespace GoalTracker.Util
{
    public class DelegateCommand<T> : ICommand
    {
        public DelegateCommand(Action<T> execute) : this(execute, null) { }
        
        public DelegateCommand(Action<T> execute, Predicate<T> canExecute) : this(execute, canExecute, String.Empty) { }

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute, string label)
        {
            this.execute = execute;
            this.canExecute = canExecute;

            Label = label;
        }

        readonly Action<T> execute = null;
        readonly Predicate<T> canExecute = null;

        public string Label { get; set; }

        public void Execute(object parameter)
        {
            execute((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                // lock?
                if (canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }
    }

    // Parameterless version
    public class DelegateCommand : ICommand
    {
        public delegate bool BoolFunc();


        public DelegateCommand(Action execute) : this(execute, null) { }

        public DelegateCommand(Action execute, BoolFunc canExecute) : this(execute, canExecute, String.Empty) { }

        public DelegateCommand(Action execute, BoolFunc canExecute, string label)
        {
            this.execute = execute;
            this.canExecute = canExecute;

            Label = label;
        }

        readonly Action execute = null;
        readonly BoolFunc canExecute = null;

        public string Label { get; set; }

        public void Execute(object parameter)
        {
            execute();
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }
    }
}
