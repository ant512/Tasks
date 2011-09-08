using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	/// <summary>
	/// Task class.
	/// </summary>
	public class Task : ITask
	{
		#region Members

		/// <summary>
		/// Cached start date of the task.
		/// </summary>
		private DateTime mStartDate;

		/// <summary>
		/// Cached end date of the task.
		/// </summary>
		private DateTime mEndDate;

		/// <summary>
		/// Duration of the task.  This represents the working time.  The actual period
		/// that the task spans (from start date to end date) may exceed the duration if
		/// this period includes non-working times.
		/// </summary>
		private TimeSpan mDuration;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a value representing the name of the task.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets a value indicating whether or not the task has child tasks.
		/// </summary>
		public bool HasChildren
		{
			get { return Children.Count > 0; }
		}

		/// <summary>
		/// Gets a value indicating whether or not the task has dependencies.
		/// </summary>
		public bool HasDependencies
		{
			get { return Dependencies.Count > 0; }
		}

		/// <summary>
		/// Gets the start date of the task.  If the task has children, the start
		/// date will be the start of the earliest child.  If not, the start date
		/// will be the value calculated by the RecalculateDates() method.
		/// </summary>
		public DateTime StartDate
		{
			get
			{
				// If we have no children, we return the start date of the task
				if (!HasChildren) return mStartDate;
	
				// Otherwise, we look at our children to find the earliest start date and
				// return that
	
				// Set earliest date to the maximum date C# can represent
				var earliestDate = DateTime.MaxValue;
	
				foreach (var child in Children) {
					if (child.StartDate < earliestDate) {
						earliestDate = child.StartDate;
					}
				}
	
				return earliestDate;
			}
		}

		/// <summary>
		/// Gets the end date of the task.  If the task has children, the start
		/// date will be the end of the latest child.  If not, the end date
		/// will be the value calculated by the RecalculateDates() method.
		/// </summary>
		public DateTime EndDate
		{
			get
			{
				// If we have no children, we return the end date of the task
				if (!HasChildren) return mEndDate;
	
				// Otherwise, we look at our children to find the latest end date and
				// return that
	
				// Set latestDate date to the minimum date C# can represent
				var latestDate = DateTime.MinValue;
	
				foreach (var child in Children) {
					if (child.EndDate > latestDate) {
						latestDate = child.EndDate;
					}
				}
	
				return latestDate;
			}
		}

		/// <summary>
		/// Gets the list of dependencies.
		/// </summary>
		public List<IDependency> Dependencies { get; private set; }

		/// <summary>
		/// Gets or sets the duration of the task.  This represents the working time. 
		/// The actual period that the task spans (from start date to end date) may
		/// exceed the duration if this period includes non-working times.
		/// </summary>
		public TimeSpan Duration
		{
			get
			{
				// If we don't have children, just return the duration
				if (!HasChildren) return mDuration;

				// If we do have children, the duration is defined as the difference between
				// the start and end dates
				return EndDate.Subtract(StartDate);
			}
			set
			{
				if (HasChildren) {
					throw new InvalidOperationException("Cannot manually set the duration of tasks with children.");
				}

				mDuration = value;
			}
		}

		/// <summary>
		/// Gets or sets the task's parent task.  If null, the task has no parent.
		/// </summary>
		public ITask Parent { get; set; }

		/// <summary>
		/// Gets the list of the task's children.
		/// </summary>
		public List<ITask> Children { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Task class.
		/// </summary>
		/// <param name="name">Name of the task.</param>
		/// <param name="duration">Duration of the task as expressed as a working period.</param>
		public Task(string name, TimeSpan duration)
		{
			Children = new List<ITask>();
			Dependencies = new List<IDependency>();
	
			Name = name;
			Duration = duration;

			Parent = null;
			mStartDate = DateTime.Now;
			mEndDate = DateTime.Now;
		}

		/// <summary>
		/// Initializes a new instance of the Task class with duration set to 0.
		/// Useful when creating phases.
		/// </summary>
		/// <param name="name">Name of the task.</param>
		public Task(string name)
			: this(name, new TimeSpan(0))
		{
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add a child to the task.  The new child's parent is automatically set to the
		/// current task.
		/// </summary>
		/// <param name="task">The task to add as a child.</param>
		public void AddChild(ITask task)
		{
			if (task.IsAncestorOf(this))
			{
				throw new ArgumentException("Tasks cannot contain their own ancestors as children.");
			}

			task.Parent = this;
			Children.Add(task);
		}

		/// <summary>
		/// Add a dependency to the task.  The dependency's owner is automatically set
		/// to the current task.
		/// </summary>
		/// <param name="dependency"></param>
		public void AddDependency(IDependency dependency)
		{
			if (dependency.DependentOn != null)
			{
				if (IsAncestorOf(dependency.DependentOn))
				{
					throw new ArgumentException("Tasks cannot be dependent on their children or successors.");
				}

				if (dependency.DependentOn.IsAncestorOf(this))
				{
					throw new ArgumentException("Child tasks cannot be dependent on their ancestors.");
				}
			}

			dependency.Owner = this;
			Dependencies.Add(dependency);
		}

		/// <summary>
		/// Check if this task is an ancestor of the specified task.
		/// </summary>
		/// <param name="task">Task to check.</param>
		/// <returns>True if this task is an ancestor of the specified task..</returns>
		public bool IsAncestorOf(ITask task)
		{
			var ancestor = task.Parent;

			while (ancestor != null)
			{
				if (ancestor == this)
				{
					return true;
				}

				ancestor = ancestor.Parent;
			}

			return false;
		}

		/// <summary>
		/// Gets all tasks on which this task is dependent, including all tasks that its
		/// ancestors are dependent on.  This gives a complete list of all tasks that
		/// directly affect the dates of this task.
		/// Any phases that are encountered as dependencies are replaced with their
		/// children (and if any of those is a phase, it is replaced with its children,
		/// etc).
		/// </summary>
		/// <returns>An array of all dependencies.</returns>
		public List<ITask> GetAllTaskDependencies()
		{
			var list = new List<ITask>();

			// Add all dependencies of this task and its ancestors
			ITask currentTask = this;
			while (currentTask != null)
			{
				foreach (var dependency in currentTask.Dependencies)
				{
					var dependentOn = dependency.DependentOn;

					if (dependentOn != null)
					{
						list.Add(dependentOn);
					}
				}
				currentTask = currentTask.Parent;
			}

			// Iterate over the list and make sure it contains no chilren
			for (var i = 0; i < list.Count; ++i)
			{
				if (list[i].HasChildren)
				{

					var phase = list[i];

					// Remove the phase
					list.RemoveAt(i);
					i--;

					// Add the phase's children to the list
					foreach (var child in phase.Children)
					{
						list.Add(child);
					}
				}
			}

			return list;
		}

		/// <summary>
		/// Recalculates the dates of the task based on the start dates of its
		/// dependencies.  The earliest date that the task will use as its start date
		/// (in the situation where it has no dependencies, for example) is the value
		/// passed as earliestDate.
		/// </summary>
		/// <param name="earliestDate">The earliest date that the task can use as its start
		/// date.</param>
		/// <param name="week">The working week to base calculations on.</param>
		public void RecalculateDates(DateTime earliestDate, Week week)
		{
			var latestDate = earliestDate;
			var highestPriority = Tasks.DependencyPriority.Low;

			foreach (var dependency in Dependencies)
			{
				var dependencyDate = dependency.StartDate(week);
				var dependencyPriority = dependency.Priority;

				// Check the priority of the dependency.  Dependencies which are of a
				// lower priority than we're currently basing our date on can be
				// ignored.
				if (dependencyPriority == highestPriority)
				{

					// Only update the date if the dependency starts later than the
					// currently calculated date
					if (dependencyDate > latestDate)
					{
						latestDate = dependencyDate;
					}
				}
				else if (dependencyPriority > highestPriority)
				{

					// This dependency has a higher priority than anything we've seen
					// so far, so we force the date to change
					latestDate = dependencyDate;
					highestPriority = dependencyPriority;
				}
			}

			// Ensure that the start date falls within a working shift
			mStartDate = week.AscendingShifts(latestDate, DateTime.MaxValue).ElementAt(0).StartTime;

			// Work out end date using working week.  We subtract 1 from the duration
			// to ensure that, should the end date be at the end of a working shift,
			// we retrieve the correct shift from the working week later
			mEndDate = week.DateAdd(mStartDate, mDuration.Subtract(new TimeSpan(0, 0, 0, 0, 1)));

			// Ensure that the end date falls within a working shift
			mEndDate = week.AscendingShifts(mEndDate, DateTime.MaxValue).ElementAt(0).StartTime;

			// Add the millisecond back on to the date that we subtracted earlier
			mEndDate = mEndDate.Add(new TimeSpan(0, 0, 0, 0, 1));
		}

		#endregion
	}
}
