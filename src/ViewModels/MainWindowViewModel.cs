using GoalTracker.Util;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using static GoalTracker.Util.DateUtil;

namespace GoalTracker.ViewModels
{
    // PropertyChanged raised for: DisplayMonth
    public class MainWindowViewModel : ObservableObject
    {
        private readonly IGoalTrackerService gtService;
        private readonly IWindowService windowService;
        
        public MainWindowViewModel(IGoalTrackerService gtService, IWindowService windowService, DateTime displayDate)
        {
            this.gtService = gtService;
            this.windowService = windowService;
            
            PreviousMonthCommand = new DelegateCommand(SwitchToPreviousMonth);
            NextMonthCommand = new DelegateCommand(SwitchToNextMonth);

            NewFileCommand = new DelegateCommand(NewFile);
            LoadFileCommand = new DelegateCommand(LoadFile);
            MoveFileCommand = new DelegateCommand(MoveFile, IsSaveDataLoaded);
            ShowFilePathCommand = new DelegateCommand(RequestShowSaveFilePath);

            AddGoalCommand = new DelegateCommand(AddGoal, IsSaveDataLoaded);
            RemoveGoalCommand = new DelegateCommand(RemoveGoal, IsSaveDataLoaded);

            ShowEditorCommand = new DelegateCommand(ShowSkinEditor, () => !IsEditorOpen);
            ChangeSkinCommand = new DelegateCommand<string>(skinName => RequestChangeSkin?.Invoke(skinName),
                                                            x => !IsEditorOpen);

            CloseCommand = new DelegateCommand(() => RequestClose?.Invoke());

            DisplayMonth = displayDate;

            PopulateCalendarDaysAndWeeks();
        }

        public event Action<bool> NoDayDataToShow;
        public event Action NoWeekDataToShow;

        public event Action<string> ShowSaveFilePath;

        public event Action<string> RequestChangeSkin;

        public event Action RequestClose;

        public DelegateCommand PreviousMonthCommand { get; }
        public DelegateCommand NextMonthCommand { get; }

        public DelegateCommand NewFileCommand { get; }
        public DelegateCommand LoadFileCommand { get; }
        public DelegateCommand MoveFileCommand { get; }
        public DelegateCommand ShowFilePathCommand { get; }

        public DelegateCommand AddGoalCommand { get; }
        public DelegateCommand RemoveGoalCommand { get; }

        public DelegateCommand ShowEditorCommand { get; }
        public DelegateCommand<string> ChangeSkinCommand { get; }

        public DelegateCommand CloseCommand { get; }

        // The currently displayed calendar month; the Day is always set to 1.
        public DateTime DisplayMonth
        {
            get => displayMonth;
            set
            {
                displayMonth = new DateTime(value.Year, value.Month, 1);
                NotifyPropertyChanged();
            }
        }
        private DateTime displayMonth;

        public ObservableCollection<CalendarDayButtonViewModel> CalendarDays { get; private set; }
            = new ObservableCollection<CalendarDayButtonViewModel>();
        public ObservableCollection<CalendarWeekButtonViewModel> CalendarWeeks { get; private set; }
            = new ObservableCollection<CalendarWeekButtonViewModel>();

        public bool IsEditorOpen { get; protected set; } = false;


        private bool IsSaveDataLoaded()
        {
            return gtService.IsUserDataLoaded;
        }

        // (for commands that require a parameter)
        private bool IsSaveDataLoaded(object parameter)
        {
            return gtService.IsUserDataLoaded;
        }

        private void SwitchToPreviousMonth()
        {
            DisplayMonth = DisplayMonth.AddMonths(-1);
            PopulateCalendarDaysAndWeeks();
        }

        private void SwitchToNextMonth()
        {
            DisplayMonth = DisplayMonth.AddMonths(1);
            PopulateCalendarDaysAndWeeks();
        }

        private void NewFile()
        {
            string filePath = windowService.PromptForSaveFilePath();
            if (filePath != null)
            {
                gtService.CreateDataAtPath(filePath);
            }
        }

        private void LoadFile()
        {
            string filePath = windowService.PromptForOpenFilePath();
            if (filePath != null)
            {
                gtService.LoadDataFromPath(filePath);
            }
        }

        private void MoveFile()
        {
            string filePath = windowService.PromptForSaveFilePath();
            if (filePath != null)
            {
                gtService.MoveSaveFilePath(filePath);
            }
        }

        private void RequestShowSaveFilePath()
        {
            ShowSaveFilePath?.Invoke(gtService.SaveFilePath);
        }

        private void AddGoal()
        {
            if (gtService.IsUserDataLoaded)
            {
                windowService.ShowAddGoalWindow(gtService);
            }
        }

        private void RemoveGoal()
        {
            if (gtService.IsUserDataLoaded)
            {
                windowService.ShowRemoveGoalWindow(gtService);
            }
        }

        private void ShowDayWindow(CalendarButtonViewModel vm)
        {
            if (!gtService.IsUserDataLoaded)
            {
                return;
            }

            DateTime date = vm.Date;

            if (!HasDataToShowForDate(date))
            {
                bool hasUntrackedWeeklyGoals = gtService.WeeklyGoals.Count > 0;
                NoDayDataToShow?.Invoke(hasUntrackedWeeklyGoals);
                return;
            }

            windowService.ShowDailyGoalsWindow(date, gtService);

            gtService.Save();  // Save when the popup closes
        }

        private bool HasDataToShowForDate(DateTime date)
        {
            return gtService.DailyGoals.Count > 0
                || gtService.GetDayForDate(date) != null
                || gtService.GetWeekForDate(date) != null;
        }

        private void ShowWeekWindow(CalendarButtonViewModel vm)
        {
            if (!gtService.IsUserDataLoaded)
            {
                return;
            }

            windowService.ShowWeeklyGoalsWindow(vm.Date, gtService);

            gtService.Save();
        }

        private void TrackWeek(CalendarButtonViewModel vm)
        {
            if (!gtService.IsUserDataLoaded)
            {
                return;
            }

            DateTime date = vm.Date;

            // The button to call this method should only be visible/clickable when no data exists for this week
            Debug.Assert(gtService.GetWeekForDate(date) == null);

            if (gtService.WeeklyGoals.Count > 0)
            {
                gtService.CreateWeek(FirstDayOfWeek(date));
            }
            else
            {
                NoWeekDataToShow?.Invoke();
            }
        }

        private void ShowSkinEditor()
        {
            IsEditorOpen = true;
            windowService.ShowSkinEditorWindow((sender, e) => IsEditorOpen = false);
        }

        private void PopulateCalendarDaysAndWeeks()
        {
            CalendarDays.Clear();
            CalendarWeeks.Clear();

            DateTime calendarFirst = FirstDayOfWeek(DisplayMonth);
            DateTime calendarLast = LastDayOfWeek(LastDayOfMonth(DisplayMonth));

            int totalCalendarDays = (calendarLast - calendarFirst).Days + 1;

            for (int i = 0; i < totalCalendarDays; i++)
            {
                DateTime date = calendarFirst.AddDays(i);

                AddCalendarDayButton(date);

                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    AddCalendarWeekButton(date);
                }
            }
        }

        private void AddCalendarDayButton(DateTime date)
        {
            var vm = new CalendarDayButtonViewModel(date, gtService)
            {
                ShowDayWindowCommand = new DelegateCommand<CalendarButtonViewModel>(ShowDayWindow, IsSaveDataLoaded)
            };
            CalendarDays.Add(vm);
        }

        private void AddCalendarWeekButton(DateTime date)
        {
            var vm = new CalendarWeekButtonViewModel(date, gtService)
            {
                ShowWeekWindowCommand = new DelegateCommand<CalendarButtonViewModel>(ShowWeekWindow, IsSaveDataLoaded),
                TrackWeekCommand = new DelegateCommand<CalendarButtonViewModel>(TrackWeek, IsSaveDataLoaded)
            };
            CalendarWeeks.Add(vm);
        }
    }
}
