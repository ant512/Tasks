﻿using System;
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
			// Anything more complex than a single task has to be part of a project for its
			// dates to work correctly
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
			// Anything more complex than a single task has to be part of a project for its
			// dates to work correctly
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
	}
}
