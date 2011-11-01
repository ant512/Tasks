using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	/// <summary>
	/// Defines the interface for task-like objects.
	/// </summary>
	public interface ITask
	{
		#region Properties

		/// <summary>
		/// Gets or sets the name of the task.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Gets the start date of the task.
		/// </summary>
		DateTime StartDate { get; }

		/// <summary>
		/// Gets the end date of the task.
		/// </summary>
		DateTime EndDate { get; }

		/// <summary>
		/// Gets the list of the task's dependencies.
		/// </summary>
		IList<IDependency> Dependencies { get; }

		/// <summary>
		/// Gets or sets the duration of the task.
		/// </summary>
		TimeSpan Duration { get; set; }

		/// <summary>
		/// Gets or sets the parent of the task.
		/// </summary>
		ITask Parent { get; set; }

		/// <summary>
		/// Gets the list of the task's children.
		/// </summary>
		IList<ITask> Children { get; }

		/// <summary>
		/// Gets a value indicating whether or not the task has children.
		/// </summary>
		bool HasChildren { get; }

		/// <summary>
		/// Gets a value indicating whether or not the task has dependencies.
		/// </summary>
		bool HasDependencies { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Check if the specified task is an ancestor of this task.
		/// </summary>
		/// <param name="task">Task to check.</param>
		/// <returns>True if the specified task is an ancestor of this task.</returns>
		bool IsAncestorOf(ITask task);

		/// <summary>
		/// Recalculates the dates of the task based on the start dates of its
		/// dependencies.  The earliest date that the task will use as its start date
		/// (in the situation where it has no dependencies, for example) is the value
		/// passed as earliestDate.
		/// </summary>
		void RecalculateDates(DateTime earliestDate, Week week);

		/// <summary>
		/// Gets all tasks on which this task is dependent, including all tasks that its
		/// ancestors are dependent on.  This gives a complete list of all tasks that
		/// directly affect the dates of this task.
		/// Any phases that are encountered as dependencies are replaced with their
		/// children (and if any of those is a phase, it is replaced with its children,
		/// etc).
		/// </summary>
		List<ITask> GetAllTaskDependencies();

		/// <summary>
		/// Add a child to the task.  The new child's parent is automatically set to the
		/// current task.
		/// </summary>
		/// <param name="task">The task to add as a child.</param>
		void AddChild(ITask child);

		/// <summary>
		/// Add a dependency to the task.  The dependency's owner is automatically set
		/// to the current task.
		/// </summary>
		/// <param name="dependency">The dependency to add.</param>
		void AddDependency(IDependency dependency);

		#endregion
	}
}
