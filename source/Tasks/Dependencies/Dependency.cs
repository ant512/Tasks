using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasks
{
	public abstract class Dependency : IDependency
	{
		#region Properties

		public DependencyPriority Priority { get; private set; }

		public ITask Owner { get; set; }

		public ITask DependentOn { get; private set; }

		public TimeSpan Lag { get; set; }

		public abstract DateTime StartDate(WorkingWeek.Week week);

		#endregion

		#region Constructors

		public Dependency(ITask dependentOn, DependencyPriority priority, TimeSpan lag)
		{
			DependentOn = dependentOn;
			Priority = priority;
			Lag = lag;
		}

		#endregion
	}
}
