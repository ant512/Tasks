﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	/// <summary>
	/// Defines a "start to start dependency", in which task A (dependent on)
	/// and task B (owner) will start simultaneously.  This is a low priority
	/// dependency.
	/// </summary>
	public class StartToStartDependency : LaggedDependency
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the StartToStartDependency class.
		/// </summary>
		/// <param name="dependentOn">The task on which the dependency is dependent.</param>
		/// <param name="lag">The lag of the dependency.</param>
		public StartToStartDependency(ITask dependentOn, TimeSpan lag)
			: base(dependentOn, lag)
		{
		}

		/// <summary>
		/// Initializes a new instance of the StartToStartDependency class.  No
		/// lag time is set.
		/// </summary>
		/// <param name="dependentOn">The task on which the dependency is dependent.</param>
		public StartToStartDependency(ITask dependentOn)
			: base(dependentOn, new TimeSpan(0))
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
			return week.DateAdd(DependentOn.StartDate, Lag);
		}

		#endregion
	}
}
