using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasks
{
	/// <summary>
	/// Base class for dependencies that include a lag time between the start/end of one task
	/// and the start/end of the next.
	/// </summary>
	public abstract class LaggedDependency : Dependency
	{
		#region Properties

		/// <summary>
		/// Gets or sets the lag time of the dependency.  Lag is an optional period
		/// that is added to the dependency to adjust the calculated date of the
		/// dependent task.  For example, a task could start one hour after another
		/// task ends by setting the lag to an hour.
		/// </summary>
		public TimeSpan Lag { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Dependency class.
		/// </summary>
		/// <param name="dependentOn">The task on which the dependency is dependent.</param>
		/// <param name="priority">The priority of the dependency.</param>
		/// <param name="lag">The lag of the dependency.</param>
		public LaggedDependency(ITask dependentOn, DependencyPriority priority, TimeSpan lag)
			: base(dependentOn, priority)
		{
			Lag = lag;
		}

		#endregion
	}
}
