using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	public class FinishToFinishDependency : Dependency
	{
		public FinishToFinishDependency(ITask dependentOn, TimeSpan lag)
			: base(dependentOn, DependencyPriority.Low, lag)
		{
		}

		public override DateTime StartDate(Week week)
		{
			return week.DateAdd(DependentOn.EndDate, Owner.Duration.Negate().Add(Lag));
		}
	}
}
