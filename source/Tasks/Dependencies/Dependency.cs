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
		/// Gets the task that contains this dependency.
		/// </summary>
		public ITask Owner { get; set; }

		/// <summary>
		/// Gets the task on which the owner is dependent.
		/// </summary>
		public ITask DependentOn { get; private set; }

		#endregion

		#region Methods

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
		public Dependency(ITask dependentOn)
		{
			DependentOn = dependentOn;
		}

		#endregion
	}
}
