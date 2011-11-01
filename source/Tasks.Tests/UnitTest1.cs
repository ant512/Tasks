using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkingWeek;

namespace Tasks.Tests
{
	[TestClass]
	public class UnitTest1
	{
		private Week NewWeek()
		{
			var week = new WorkingWeek.Week();

			week.AddShift(DayOfWeek.Monday, 9, 30, 0, 0, new TimeSpan(0, 3, 0, 0, 0));
			week.AddShift(DayOfWeek.Monday, 13, 30, 0, 0, new TimeSpan(0, 4, 0, 0, 0));

			week.AddShift(DayOfWeek.Tuesday, 9, 30, 0, 0, new TimeSpan(0, 3, 0, 0, 0));
			week.AddShift(DayOfWeek.Tuesday, 13, 30, 0, 0, new TimeSpan(0, 4, 0, 0, 0));

			week.AddShift(DayOfWeek.Wednesday, 9, 30, 0, 0, new TimeSpan(0, 3, 0, 0, 0));
			week.AddShift(DayOfWeek.Wednesday, 13, 30, 0, 0, new TimeSpan(0, 4, 0, 0, 0));

			week.AddShift(DayOfWeek.Thursday, 9, 30, 0, 0, new TimeSpan(0, 3, 0, 0, 0));
			week.AddShift(DayOfWeek.Thursday, 13, 30, 0, 0, new TimeSpan(0, 4, 0, 0, 0));

			week.AddShift(DayOfWeek.Friday, 9, 30, 0, 0, new TimeSpan(0, 3, 0, 0, 0));
			week.AddShift(DayOfWeek.Friday, 13, 30, 0, 0, new TimeSpan(0, 4, 0, 0, 0));

			return week;
		}

		private Project NewProject()
		{
			return new Project("Project", new DateTime(2011, 09, 08, 10, 0, 0, 0), NewWeek());
		}

		[TestMethod]
		public void TestInheritedDependencies()
		{
			var week = new Week();
			week.AddShift(DayOfWeek.Monday, 9, 30, 0, 0, TimeSpan.FromHours(4));
			week.AddShift(DayOfWeek.Monday, 14, 30, 0, 0, TimeSpan.FromHours(3));

			DateTime end = week.DateAdd(new DateTime(2011, 11, 1, 9, 30, 0), TimeSpan.FromHours(7));
			Console.WriteLine(end);

			var project = new Project("Some project", new DateTime(2011, 11, 1, 9, 30, 0), week);
			project.AddTask(new Task("Phase 1"));
			project.Tasks[0].AddChild(new Task("Task 1", TimeSpan.FromHours(10)));
			project.Tasks[0].AddChild(new Task("Task 2", TimeSpan.FromHours(15)));

			project.AddTask(new Task("Phase 2"));
			project.Tasks[1].AddChild(new Task("Task 1", TimeSpan.FromHours(12)));

			project.Tasks[1].AddDependency(new FinishToStartDependency(project.Tasks[0]));
			project.RecalculateDates();

			Assert.AreNotEqual(project.Tasks[0].StartDate, project.Tasks[1].StartDate);
		}

		[TestMethod]
		public void TestTaskStartEndDates()
		{
			var week = NewWeek();
			var task = new Task("Task 1", new TimeSpan(10, 10, 10));
			task.RecalculateDates(new DateTime(2011, 09, 08, 10, 0, 0, 0), week);

			Assert.AreEqual(new DateTime(2011, 09, 08, 10, 0, 0, 0), task.StartDate);
			Assert.AreEqual(new DateTime(2011, 09, 09, 14, 10, 10, 0), task.EndDate);
		}

		[TestMethod]
		public void TestPhaseStartEndDates()
		{
			// Anything more complex than a single task has to be part of a project for its
			// dates to work correctly
			var project = NewProject();

			var task1 = new Task("Task 1", new TimeSpan(10, 10, 10));
			var task2 = new Task("Task 2", new TimeSpan(12, 10, 10));
			var phase = new Task("Phase 1");

			phase.AddChild(task1);
			phase.AddChild(task2);

			project.AddTask(phase);

			project.RecalculateDates();

			Assert.AreEqual(new DateTime(2011, 09, 08, 10, 0, 0, 0), phase.StartDate);
			Assert.AreEqual(new DateTime(2011, 09, 09, 16, 10, 10, 0), phase.EndDate);
		}

		[TestMethod]
		public void TestDependentTaskStartEndDates()
		{
			var project = NewProject();

			var task1 = new Task("Task 1", new TimeSpan(10, 10, 10));
			var task2 = new Task("Task 2", new TimeSpan(12, 10, 10));
			var phase = new Task("Phase 1");

			task2.AddDependency(new FinishToStartDependency(task1));

			phase.AddChild(task1);
			phase.AddChild(task2);

			project.AddTask(phase);

			project.RecalculateDates();

			Assert.AreEqual(new DateTime(2011, 09, 08, 10, 0, 0, 0), phase.StartDate);
			Assert.AreEqual(new DateTime(2011, 09, 13, 11, 20, 20, 0), phase.EndDate);
		}

		[TestMethod]
		public void TestDependentLaggedTaskStartEndDates()
		{
			var project = NewProject();

			var task1 = new Task("Task 1", new TimeSpan(10, 10, 10));
			var task2 = new Task("Task 2", new TimeSpan(12, 10, 10));
			var phase = new Task("Phase 1");

			task2.AddDependency(new FinishToStartDependency(task1, new TimeSpan(1, 0, 0)));

			phase.AddChild(task1);
			phase.AddChild(task2);

			project.AddTask(phase);

			project.RecalculateDates();

			Assert.AreEqual(new DateTime(2011, 09, 08, 10, 0, 0, 0), phase.StartDate);
			Assert.AreEqual(new DateTime(2011, 09, 13, 12, 20, 20, 0), phase.EndDate);
		}

		[TestMethod]
		public void TestNegativeLag()
		{
			var project = NewProject();

			var task1 = new Task("Task 1", new TimeSpan(10, 10, 10));
			var task2 = new Task("Task 2", new TimeSpan(12, 10, 10));
			var phase = new Task("Phase 1");

			task2.AddDependency(new FinishToStartDependency(task1, new TimeSpan(-1, 0, 0)));

			phase.AddChild(task1);
			phase.AddChild(task2);

			project.AddTask(phase);

			project.RecalculateDates();

			Assert.AreEqual(new DateTime(2011, 09, 08, 10, 0, 0, 0), phase.StartDate);
			Assert.AreEqual(new DateTime(2011, 09, 13, 10, 20, 20, 0), phase.EndDate);
		}

		[TestMethod]
		public void TestIneffectualFinishToFinishDependency()
		{
			var project = NewProject();

			var task1 = new Task("Task 1", new TimeSpan(10, 10, 10));
			var task2 = new Task("Task 2", new TimeSpan(12, 10, 10));
			var phase = new Task("Phase 1");

			task2.AddDependency(new FinishToFinishDependency(task1));

			phase.AddChild(task1);
			phase.AddChild(task2);

			project.AddTask(phase);

			project.RecalculateDates();

			// Since task1 starts at the beginning of the project and task2 is longer,
			// the dependency can have no effect - task2 cannot start before the beginning
			// of the project and task1 cannot be moved by the dependency.  This is
			// expected behaviour.
			Assert.IsTrue(task1.EndDate < task2.EndDate);
		}

		[TestMethod]
		public void TestFinishToFinishDependency()
		{
			var project = NewProject();

			var task1 = new Task("Task 1", new TimeSpan(10, 10, 10));
			var task2 = new Task("Task 2", new TimeSpan(12, 10, 10));
			var phase = new Task("Phase 1");

			task1.AddDependency(new FinishToFinishDependency(task2));

			phase.AddChild(task1);
			phase.AddChild(task2);

			project.AddTask(phase);

			project.RecalculateDates();

			Assert.AreEqual(task1.EndDate, task2.EndDate);
		}

		[TestMethod]
		public void TestFixedStartDependency()
		{
			var project = NewProject();

			var task1 = new Task("Task 1", new TimeSpan(10, 10, 10));
			var task2 = new Task("Task 2", new TimeSpan(12, 10, 10));
			var phase = new Task("Phase 1");

			task1.AddDependency(new FixedStartDependency(new DateTime(2011, 09, 10)));

			phase.AddChild(task1);
			phase.AddChild(task2);

			project.AddTask(phase);

			project.RecalculateDates();

			// Task starts ASAP after 10/09/2011, based on the working week.  10th
			// and 11th are Sat and Sun, so task starts at 9:30 on Monday 12th.
			Assert.AreEqual(new DateTime(2011, 09, 12, 9, 30, 0), task1.StartDate);
		}

		[TestMethod]
		public void TestFixedFinishDependency()
		{
			var project = NewProject();

			var task1 = new Task("Task 1", new TimeSpan(10, 10, 10));
			var task2 = new Task("Task 2", new TimeSpan(12, 10, 10));
			var phase = new Task("Phase 1");

			task1.AddDependency(new FixedFinishDependency(new DateTime(2011, 09, 12, 9, 45, 0)));

			phase.AddChild(task1);
			phase.AddChild(task2);

			project.AddTask(phase);

			project.RecalculateDates();

			Assert.AreEqual(new DateTime(2011, 09, 12, 9, 45, 0), task1.EndDate);
		}

		[TestMethod]
		public void TestStartToStartDependency()
		{
			var project = NewProject();

			var task1 = new Task("Task 1", new TimeSpan(10, 10, 10));
			var task2 = new Task("Task 2", new TimeSpan(12, 10, 10));
			var phase = new Task("Phase 1");

			task1.AddDependency(new FixedStartDependency(new DateTime(2011, 09, 12, 9, 45, 0)));
			task2.AddDependency(new StartToStartDependency(task1));

			phase.AddChild(task1);
			phase.AddChild(task2);

			project.AddTask(phase);

			project.RecalculateDates();

			Assert.AreEqual(task1.StartDate, task2.StartDate);
		}

		[TestMethod]
		public void TestStartToFinishDependency()
		{
			var project = NewProject();

			var task1 = new Task("Task 1", new TimeSpan(1, 0, 0));
			var task2 = new Task("Task 2", new TimeSpan(1, 0, 0));
			var phase = new Task("Phase 1");

			task1.AddDependency(new FixedStartDependency(new DateTime(2011, 09, 14, 9, 45, 0)));
			task2.AddDependency(new StartToFinishDependency(task1));

			phase.AddChild(task1);
			phase.AddChild(task2);

			project.AddTask(phase);

			project.RecalculateDates();

			Assert.AreEqual(task1.StartDate, task2.EndDate);
		}

		[TestMethod]
		public void TestTaskAncestorProtection()
		{
			var task1 = new Task("Task 1", new TimeSpan(10, 10, 10));
			var phase = new Task("Phase 1");

			phase.AddChild(task1);

			try
			{
				task1.AddChild(phase);
				Assert.Fail("Should not be possible for parents to own their own child tasks.");
			}
			catch (ArgumentException)
			{
			}
		}

		[TestMethod]
		public void TestTaskChildDependencyProtection()
		{
			var task1 = new Task("Task 1", new TimeSpan(10, 10, 10));
			var phase = new Task("Phase 1");

			phase.AddChild(task1);

			try
			{
				phase.AddDependency(new StartToStartDependency(task1));
				Assert.Fail("Should not be possible for parents to be dependent on their own child tasks.");
			}
			catch (ArgumentException)
			{
			}
		}

		[TestMethod]
		public void TestTaskAncestorDependencyProtection()
		{
			var task1 = new Task("Task 1", new TimeSpan(10, 10, 10));
			var phase = new Task("Phase 1");

			phase.AddChild(task1);

			try
			{
				task1.AddDependency(new StartToStartDependency(phase));
				Assert.Fail("Should not be possible for children to be dependent on their parents.");
			}
			catch (ArgumentException)
			{
			}
		}

		[TestMethod]
		public void TestTaskAncestorDetection()
		{
			var task1 = new Task("Task 1", new TimeSpan(10, 10, 10));
			var phase = new Task("Phase 1");

			phase.AddChild(task1);

			Assert.IsTrue(phase.IsAncestorOf(task1));
			Assert.IsFalse(task1.IsAncestorOf(phase));
		}

		[TestMethod]
		public void TestTaskDependencyList()
		{
			var task1 = new Task("Task 1", new TimeSpan(10, 10, 10));
			var task2 = new Task("Task 2", new TimeSpan(10, 10, 10));
			var task3 = new Task("Task 3", new TimeSpan(10, 10, 10));
			var task4 = new Task("Task 4", new TimeSpan(10, 10, 10));
			var phase = new Task("Phase 1");

			phase.AddChild(task1);
			phase.AddChild(task2);
			phase.AddChild(task3);

			task4.AddDependency(new StartToStartDependency(phase));

			var dependencies = task4.GetAllTaskDependencies();

			Assert.AreEqual(3, dependencies.Count);
		}

		[TestMethod]
		public void TestPhaseDurationSet()
		{
			var task1 = new Task("Task 1", new TimeSpan(10, 10, 10));
			var phase = new Task("Phase 1");

			phase.AddChild(task1);

			try
			{
				phase.Duration = new TimeSpan(10);
				Assert.Fail("Should not be able to set the duration of tasks with children.");
			}
			catch (InvalidOperationException)
			{
			}
		}
	}
}
