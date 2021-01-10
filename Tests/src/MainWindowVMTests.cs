using GoalTracker;
using GoalTracker.ViewModels;
using Moq;
using NUnit.Framework;
using System;

namespace Tests
{
    class MainWindowVMTests
    {
        private static DateTime testDate = new DateTime(2020, 9, 17);

        private static DateTime[] testDates =
        {
            new DateTime(2020, 9, 17),
            new DateTime(2020, 10, 1),
            new DateTime(2020, 12, 31),
        };

        [TestCaseSource(nameof(testDates))]
        public void PreviousMonthCommand_UpdatesDisplayMonth(DateTime date)
        {
            MainWindowViewModel vm = new MainWindowViewModel(new Mock<IGoalTrackerService>().Object, new Mock<IWindowService>().Object, date);

            vm.PreviousMonthCommand.Execute(null);

            Assert.That(vm.DisplayMonth.Month, Is.EqualTo(date.AddMonths(-1).Month));
        }

        [TestCaseSource(nameof(testDates))]
        public void NextMonthCommand_UpdatesDisplayMonth(DateTime date)
        {
            MainWindowViewModel vm = new MainWindowViewModel(new Mock<IGoalTrackerService>().Object, new Mock<IWindowService>().Object, date);

            vm.NextMonthCommand.Execute(null);

            Assert.That(vm.DisplayMonth.Month, Is.EqualTo(date.AddMonths(1).Month));
        }

        [Test]
        public void MoveFileCommand_CanExecute_IsFalse_WhenNoUserDataLoaded()
        {
            MainWindowViewModel vm = new MainWindowViewModel(new Mock<IGoalTrackerService>().Object, new Mock<IWindowService>().Object, testDate);

            Assert.That(vm.MoveFileCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void GoalCommands_CanExecute_IsFalse_WhenNoUserDataLoaded()
        {
            MainWindowViewModel vm = new MainWindowViewModel(new Mock<IGoalTrackerService>().Object, new Mock<IWindowService>().Object, testDate);

            Assert.That(vm.AddGoalCommand.CanExecute(null), Is.False);
            Assert.That(vm.RemoveGoalCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void SkinCommands_CanExecute_IsFalse_WhenEditorIsOpen()
        {
            MainWindowViewModel vm = new MainWindowViewModel(new Mock<IGoalTrackerService>().Object, new Mock<IWindowService>().Object, testDate);

            vm.ShowEditorCommand.Execute(null);

            Assert.That(vm.ShowEditorCommand.CanExecute(null), Is.False);
            Assert.That(vm.ChangeSkinCommand.CanExecute(null), Is.False);
        }
    }
}
