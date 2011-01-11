using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	public class StartToStartDependency : Dependency
	{
		public StartToStartDependency(ITask dependentOn, TimeSpan lag)
			: base(dependentOn, DependencyPriority.Low, lag)
		{
		}

		public override DateTime StartDate(Week week)
		{
			return week.DateAdd(DependentOn.StartDate, Lag);
		}
	}
}
