using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	public class DateCalculator
	{
		private class TaskDependencies
		{
			public ITask Task { get; set; }
			public List<ITask> Dependencies { get; set; }

			public TaskDependencies(ITask task, List<ITask> dependencies)
			{
				Task = task;
				Dependencies = dependencies;
			}
		}

		private List<TaskDependencies> GetFlatList(List<ITask> tasks)
		{
			var list = new List<TaskDependencies>();
			var stack = new Stack<ITask>();

			foreach (var task in tasks)
			{
				stack.Push(task);
			}

			while (stack.Count > 0)
			{
				var task = stack.Pop();

				if (task.HasChildren)
				{

					// Do not add phases to the list - only add their childen.  We
					// do not want to try and work with phases as they make the
					// calculations horrendous due to the need to introduce circular
					// dependencies, so instead we substitute a phase's children
					// for the phase itself
					foreach (var child in task.Children)
					{
						stack.Push(child);
					}
				}
				else
				{

					// Get a list of all dependencies that this task has.  This list
					// will include its own dependencies and the dependencies of its
					// ancestors, as all of these affect the task
					var dependencies = task.GetAllTaskDependencies();

					list.Add(new TaskDependencies(task, dependencies));
				}
			}

			return list;
		}

		private List<ITask> GetSortedList(List<ITask> tasks)
		{
			var rootTasks = new List<TaskDependencies>();
			var removedDependencyCounts = new List<int>();
			var sortedTasks = new List<ITask>();
			var unsortedTasks = GetFlatList(tasks);

			// Create list of tasks that are not dependent on any other
			foreach (var task in unsortedTasks)
			{
				if (task.Dependencies.Count == 0)
				{
					rootTasks.Add(task);
				}

				removedDependencyCounts.Add(0);
			}

			// Sort the graph
			while (rootTasks.Count > 0)
			{

				// Remove the first task from the root list
				var task = rootTasks[0];
				rootTasks.RemoveAt(0);

				// Add the task to the sorted list
				sortedTasks.Add(task.Task);

				// Loop through the tasks that are dependent on this, reducing their
				// counts and, for each task with no subsequent dependencies, adding it
				// to the sorted list
				for (var i = 0; i < unsortedTasks.Count; ++i)
				{
					var dependencies = unsortedTasks[i].Dependencies;

					foreach (var dependency in dependencies)
					{
						if (dependency == task.Task)
						{
							removedDependencyCounts[i]++;

							if (dependencies.Count == removedDependencyCounts[i])
							{
								rootTasks.Add(unsortedTasks[i]);
							}
						}
					}
				}
			}

			return sortedTasks;
		}

		public void RecalculateDates(List<ITask> tasks, DateTime earliestDate, Week week) {
	
			// Ensure that the list is sorted topologically before we begin.  This means
			// that all tasks will have the correct start and end dates before any
			// dependent tasks have their dates calculated
			var list = GetSortedList(tasks);
	
			// Recalculate the start date for all tasks in the list
			foreach (var task in list) {
				task.RecalculateDates(earliestDate, week);
			}
		}
	}
}
