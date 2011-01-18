using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	/// <summary>
	/// Used to recalculate the dates of all tasks in a project.
	/// </summary>
	public class DateCalculator
	{
		#region Classes

		/// <summary>
		/// Class that stores a task object along side a list of the tasks that
		/// it is dependent on.
		/// </summary>
		private class TaskDependencies
		{
			/// <summary>
			/// Gets the task.
			/// </summary>
			public ITask Task { get; private set; }

			/// <summary>
			/// Gets the list of tasks on which the task is dependent.
			/// </summary>
			public List<ITask> Dependencies { get; private set; }

			/// <summary>
			/// Initializes a new instance of the TaskDependencies class.
			/// </summary>
			/// <param name="task">The task.</param>
			/// <param name="dependencies">The tasks on which the task is dependent.</param>
			public TaskDependencies(ITask task, List<ITask> dependencies)
			{
				Task = task;
				Dependencies = dependencies;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Get a flattened array of the tasks in the current tree array.  Each item in
		/// the array in an anonymous object consisting of a task and an array of
		/// dependencies.  Dependencies are represented solely by the task on which the
		/// current task is dependent.  Phases (tasks with children) are not represented
		/// in the array.  Dependencies on phases are replaced with dependencies on all
		/// children within the phase.  For example, suppose we have this:
		///
		/// +-----+
		/// |  1  |---------+
		/// +-----+         |
		///          +--------------------+
		///          |          2         |----+
		///            +-----+                 |
		///            |  3  |-------+         |
		///            +-----+       |         |
		///                       +------+     |
		///                       |  4   |     |
		///                       +------+     |
		///                                 +-----+
		///                                 |  5  |
		///                                 +-----+
		///
		/// The resultant array looks like this:
		///   1
		///   3 (dependent on 1)
		///   4 (dependent on 3 and 1)
		///   5 (dependent on 3 and 4)
		///
		/// We've basically substituted phase 2 with (task 3 and 4).  This means that
		/// we don't run into a cyclic dependency issue.  In the diagram above, the start
		/// of 3 is dependent on the start of 2.  However, if 3 had a dependency on
		/// another task, then the start of 2 would also be dependent on the start of 3.
		/// Cyclic dependency = impossible to resolve without some nasty, hacky
		/// workarounds.
		/// </summary>
		/// <param name="tasks">The array/tree hybrid data structure that contains the
		/// tasks to recalculate.</param>
		/// <returns>An array of task/dependencies.</returns>
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

		/// <summary>
		/// Gets a topologically sorted array of the task list.  Each task is ordered so
		/// that the tasks that it depends on come before it in the list.  Therefore, the
		/// first task(s) in the list will always be those tasks that are not dependent
		/// on any other.
		/// Unlike the getFlatList() function, this returns an array of plain task
		/// objects.
		/// </summary>
		/// <param name="tasks">The array/tree hybrid data structure that contains the
		/// tasks to recalculate.</param>
		/// <returns>An array of sorted task objects.</returns>
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

			// If there is a cycle in the data, we will end up with a sorted list that
			// is shorter than the flat list
			if (sortedTasks.Count < unsortedTasks.Count)
			{
				throw new ArgumentException("Cycle detected in task dependencies.");
			}

			return sortedTasks;
		}

		/// <summary>
		/// Recalculates the start date of all tasks in the list.  Relies on the GetSortedList()
		/// function to ensure that all tasks on which task T is dependent on are
		/// calculated before T is reached.  Recalculating dates becomes a simple matter
		/// of asking the task's dependencies for their start dates, and choosing the
		/// latest date.
		/// </summary>
		/// <param name="tasks">The array/tree hybrid data structure that contains the
		/// tasks to recalculate.</param>
		/// <param name="earliestDate">The earliest date for any task.</param>
		/// <param name="week">The working week to base date calculations on.</param>
		public void RecalculateDates(List<ITask> tasks, DateTime earliestDate, Week week)
		{
			// Ensure that the list is sorted topologically before we begin.  This means
			// that all tasks will have the correct start and end dates before any
			// dependent tasks have their dates calculated
			var list = GetSortedList(tasks);

			// Recalculate the start date for all tasks in the list
			foreach (var task in list)
			{
				task.RecalculateDates(earliestDate, week);
			}
		}

		#endregion
	}
}
