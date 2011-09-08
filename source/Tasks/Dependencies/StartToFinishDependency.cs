using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	/// <summary>
	/// Defines a "start to finish dependency", in which task B (owner)
	/// will end when task A (dependent on) starts.  This is a low priority
	/// dependency.
	/// </summary>
	public class StartToFinishDependency : LaggedDependency
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the StartToFinishDependency class.
		/// </summary>
		/// <param name="dependentOn">The task on which the dependency is dependent.</param>
		/// <param name="lag">The lag of the dependency.</param>
		public StartToFinishDependency(ITask dependentOn, TimeSpan lag)
			: base(dependentOn, DependencyPriority.Low, lag)
		{
		}

		/// <summary>
		/// Initializes a new instance of the StartToFinishDependency class.  No
		/// lag time is set.
		/// </summary>
		/// <param name="dependentOn">The task on which the dependency is dependent.</param>
		public StartToFinishDependency(ITask dependentOn)
			: base(dependentOn, DependencyPriority.Low, new TimeSpan(0))
		{
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the start date of the dependency, using the supplied working week
		/// definition to identify working vs. non-working times.
		/// </summary>
		/// <param name="week">The working week definition.</param>
		/// <returns>The start date of the dependency.</returns>
		public override DateTime StartDate(Week week)
		{
			return week.DateAdd(DependentOn.StartDate, Owner.Duration.Add(Lag));
		}

		#endregion
	}
}
