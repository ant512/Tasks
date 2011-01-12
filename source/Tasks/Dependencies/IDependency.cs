using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	#region Enums

	/// <summary>
	/// List of all possible dependency priorities.
	/// </summary>
	public enum DependencyPriority
	{
		/// <summary>
		/// Dependency is low priority.
		/// </summary>
		Low = 0,

		/// <summary>
		/// Dependency is medium priority.
		/// </summary>
		Medium = 1,

		/// <summary>
		/// Dependency is high priority.
		/// </summary>
		High = 2
	}

	#endregion

	/// <summary>
	/// Interface defining dependency-like objects.
	/// </summary>
	public interface IDependency
	{
		#region Properties

		/// <summary>
		/// Gets the priority of the dependency.  Higher priority dependencies
		/// will override dates set by lower-priority dependencies if necessary.
		/// </summary>
		DependencyPriority Priority { get; }

		/// <summary>
		/// Gets the task that contains this dependency.
		/// </summary>
		ITask Owner { get; set; }

		/// <summary>
		/// Gets the task on which the owner is dependent.
		/// </summary>
		ITask DependentOn { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Gets the start date of the dependency, using the supplied working week
		/// definition to identify working vs. non-working times.
		/// </summary>
		/// <param name="week">The working week definition.</param>
		/// <returns>The start date of the dependency.</returns>
		DateTime StartDate(Week week);

		#endregion
	}
}
