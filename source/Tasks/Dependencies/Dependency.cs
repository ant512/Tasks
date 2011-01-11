using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasks
{
	/// <summary>
	/// Base class for all other dependency classes.
	/// </summary>
	public abstract class Dependency : IDependency
	{
		#region Properties

		/// <summary>
		/// Gets the priority of the dependency.  Higher priority dependencies
		/// will override dates set by lower-priority dependencies if necessary.
		/// </summary>
		public DependencyPriority Priority { get; private set; }

		/// <summary>
		/// Gets the task that contains this dependency.
		/// </summary>
		public ITask Owner { get; set; }

		/// <summary>
		/// Gets the task on which the owner is dependent.
		/// </summary>
		public ITask DependentOn { get; private set; }

		/// <summary>
		/// Gets or sets the lag time of the dependency.  Lag is an optional period
		/// that is added to the dependency to adjust the calculated date of the
		/// dependent task.  For example, a task could start one hour after another
		/// task ends by setting the lag to an hour.
		/// </summary>
		public TimeSpan Lag { get; set; }

		/// <summary>
		/// Gets the start date of the dependency, using the supplied working week
		/// definition to identify working vs. non-working times.
		/// </summary>
		/// <param name="week">The working week definition.</param>
		/// <returns>The start date of the dependency.</returns>
		public abstract DateTime StartDate(WorkingWeek.Week week);

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Dependency class.
		/// </summary>
		/// <param name="dependentOn">The task on which the dependency is dependent.</param>
		/// <param name="priority">The priority of the dependency.</param>
		/// <param name="lag">The lag of the dependency.</param>
		public Dependency(ITask dependentOn, DependencyPriority priority, TimeSpan lag)
		{
			DependentOn = dependentOn;
			Priority = priority;
			Lag = lag;
		}

		#endregion
	}
}
