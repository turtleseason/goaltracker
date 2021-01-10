using GoalTracker;
using GoalTracker.ViewModels;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Windows;

namespace Tests
{
    public class AddGoalWindowVMTests
    {
        private AddGoalWindowViewModel GetVM()
        {
            IGoalTrackerService gts = new GoalTrackerService();
            return GetVM(gts);
        }

        private AddGoalWindowViewModel GetVM(IGoalTrackerService gts)
        {
            Mock<IWindowService> mockWinService = new Mock<IWindowService>();
            mockWinService.SetReturnsDefault(MessageBoxResult.OK);
            return GetVM(gts, mockWinService.Object);
        }

        private AddGoalWindowViewModel GetVM(IGoalTrackerService gts, IWindowService winService)
        {
            gts.LoadData(new UserData());
            return new AddGoalWindowViewModel(gts, winService);
        }


        [Test]
        public void AddGoalCommand_CanExecute_ReturnsFalse_BeforeSettingName()
        {
            AddGoalWindowViewModel vm = GetVM();
            Assert.That(vm.AddGoalCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void AddGoalCommand_CanExecute_ReturnsFalse_WhenNameIsEmpty()
        {
            AddGoalWindowViewModel vm = GetVM();
            vm.Name = "";
            Assert.That(vm.AddGoalCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void AddGoalCommand_CanExecute_ReturnsTrue_WhenNameIsNotEmpty()
        {
            AddGoalWindowViewModel vm = GetVM();
            vm.Name = "name";
            Assert.That(vm.AddGoalCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void AddGoalCommand_Execute_AddsDailyGoal_WhenIsWeeklyIsFalse()
        {
            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            AddGoalWindowViewModel vm = GetVM(mockGts.Object);
            vm.Name = "name";

            vm.AddGoalCommand.Execute(null);

            mockGts.Verify(x => x.CreateDailyGoal(vm.Name));
        }

        [Test]
        public void AddGoalCommand_Execute_AddsWeeklyGoal_WhenIsWeeklyIsTrue()
        {
            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            AddGoalWindowViewModel vm = GetVM(mockGts.Object);
            vm.Name = "name";
            vm.IsWeekly = true;

            vm.AddGoalCommand.Execute(null);

            mockGts.Verify(x => x.CreateWeeklyGoal(vm.Name, vm.WeeklyTarget));
        }

        [Test]
        public void AddGoalCommand_Execute_SetsCorrectWeeklyTarget()
        {
            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            AddGoalWindowViewModel vm = GetVM(mockGts.Object);
            vm.Name = "name";
            vm.IsWeekly = true;
            vm.WeeklyTarget = 3;

            vm.AddGoalCommand.Execute(null);

            mockGts.Verify(x => x.CreateWeeklyGoal(vm.Name, vm.WeeklyTarget));
        }

        [Test]
        public void AddGoalCommand_Execute_AddsDayData_WhenAddDataIsTrue()
        {
            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            mockGts.Setup(x => x.CreateDailyGoal("name")).Returns(new DailyGoal("name"));
            AddGoalWindowViewModel vm = GetVM(mockGts.Object);
            vm.Name = "name";
            vm.AddData = true;

            vm.AddGoalCommand.Execute(null);

            mockGts.Verify(x => x.AddGoalToDays(It.IsAny<DailyGoal>(), vm.AddDataStartDate));
        }

        [Test]
        public void AddGoalCommand_Execute_AddsWeekData_WhenAddDataIsTrue()
        {
            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            AddGoalWindowViewModel vm = GetVM(mockGts.Object);
            vm.Name = "name";
            vm.IsWeekly = true;
            vm.AddData = true;
            mockGts.Setup(x => x.CreateWeeklyGoal(vm.Name, vm.WeeklyTarget))
                .Returns(new WeeklyGoal(vm.Name, vm.WeeklyTarget));

            vm.AddGoalCommand.Execute(null);

            mockGts.Verify(x => x.AddGoalToWeeks(It.IsAny<WeeklyGoal>(), vm.AddDataStartDate));
        }

        [Test]
        public void AddGoalCommand_Execute_UsesCorrectStartDate_WhenAddDataStartDateIsChanged()
        {
            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            mockGts.Setup(x => x.CreateDailyGoal("name")).Returns(new DailyGoal("name"));
            AddGoalWindowViewModel vm = GetVM(mockGts.Object);
            vm.Name = "name";
            vm.AddData = true;
            vm.AddDataStartDate = new DateTime(2017, 03, 07);

            vm.AddGoalCommand.Execute(null);

            mockGts.Verify(x => x.AddGoalToDays(It.IsAny<DailyGoal>(), vm.AddDataStartDate));
        }

        [Test]
        public void AddGoalCommand_Execute_NotifyMerge_WhenRemovedGoalWithSameNameExists()
        {
            Mock<IWindowService> mockWinService = new Mock<IWindowService>();
            mockWinService.SetReturnsDefault(MessageBoxResult.Cancel);

            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            mockGts.Setup(x => x.HasRemovedGoal("name", Goal.GoalType.Daily)).Returns(true);

            AddGoalWindowViewModel vm = GetVM(mockGts.Object, mockWinService.Object);
            vm.Name = "name";

            vm.AddGoalCommand.Execute(null);

            mockWinService.Verify(x => x.ShowMessage(It.IsAny<string>(), "Merge data?", It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>()));
        }

        [Test]
        public void AddGoalCommand_Execute_NotifyDuplicate_WhenGoalWithSameNameExists()
        {
            Mock<IWindowService> mockWinService = new Mock<IWindowService>();
            mockWinService.SetReturnsDefault(MessageBoxResult.Cancel);

            Mock<IGoalTrackerService> mockGts = new Mock<IGoalTrackerService>();
            mockGts.Setup(x => x.CreateDailyGoal("name")).Returns((DailyGoal)null);

            AddGoalWindowViewModel vm = GetVM(mockGts.Object, mockWinService.Object);
            vm.Name = "name";

            vm.AddGoalCommand.Execute(null);

            mockWinService.Verify(x => x.ShowMessage(It.IsAny<string>(), "Goal already exists", It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>()));
        }
    }
}