using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tasks;
using WorkingWeek;

namespace Tasks.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			// Define a working week
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


			var task1 = new Tasks.Task("Research and report");
			var task2 = new Tasks.Task("Phase 2");

			var task1_1 = new Tasks.Task("Initial Meeting", new TimeSpan(4 * 7, 0, 0));
			var task1_2 = new Tasks.Task("Project Management", new TimeSpan(10 * 7, 0, 0));
			var task1_3 = new Tasks.Task("Research", new TimeSpan(10 * 7, 0, 0));
			var task1_4 = new Tasks.Task("Review documentation", new TimeSpan(4 * 7, 0, 0));
			var task1_5 = new Tasks.Task("Draft Report", new TimeSpan(3 * 7, 0, 0));
			var task1_6 = new Tasks.Task("Final Report", new TimeSpan(1 * 7, 0, 0));

			var task2_1 = new Tasks.Task("Task 1", new TimeSpan(1 * 7, 0, 0));
			var task2_2 = new Tasks.Task("Task 2", new TimeSpan(1 * 7, 0, 0));
			var task2_3 = new Tasks.Task("Task 3", new TimeSpan(1 * 7, 0, 0));

			task1.AddChild(task1_1);
			task1.AddChild(task1_2);
			task1.AddChild(task1_3);
			task1.AddChild(task1_4);
			task1.AddChild(task1_5);
			task1.AddChild(task1_6);

			task2.AddChild(task2_1);
			task2.AddChild(task2_2);
			task2.AddChild(task2_3);

			task1_2.AddDependency(new Tasks.StartToStartDependency(task1_1));
			task1_3.AddDependency(new Tasks.FinishToStartDependency(task1_1));
			task1_4.AddDependency(new Tasks.StartToStartDependency(task1_3));
			task1_5.AddDependency(new Tasks.FinishToStartDependency(task1_3));
			task1_5.AddDependency(new Tasks.StartToStartDependency(task1_4));
			task1_6.AddDependency(new Tasks.FinishToStartDependency(task1_5));

			task2_2.AddDependency(new Tasks.FinishToStartDependency(task2_1));
			task2_3.AddDependency(new Tasks.FixedFinishDependency(new DateTime(2011, 1, 31, 0, 0, 0, 0)));

			var project = new Project("Test Project", new DateTime(2011, 1, 4, 9, 30, 0, 0), week);

			project.AddTask(task1);
			project.AddTask(task2);

			project.RecalculateDates();

			AlertTaskArray(project.Tasks);
		}

		private static void AlertTaskArray(List<ITask> tasks) {
			var str = "";
			foreach (var task in tasks) {
				str += task.Name + ": " + task.StartDate + " - " + task.EndDate + '\n';
		
				foreach (var child in task.Children) {
					str += child.Name + ": " + child.StartDate + " - " + child.EndDate + '\n';
				}
			}
			Console.WriteLine(str);
		}
	}
}
