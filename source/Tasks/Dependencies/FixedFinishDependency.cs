using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	public class FixedFinishDependency : Dependency
	{
		private DateTime mEndDate;

		public FixedFinishDependency(DateTime endDate)
			: base(null, DependencyPriority.High, new TimeSpan(0))
		{
			mEndDate = endDate;
		}

		public override DateTime StartDate(Week week)
		{
			return week.DateAdd(mEndDate, Owner.Duration.Negate());
		}
	}
}
