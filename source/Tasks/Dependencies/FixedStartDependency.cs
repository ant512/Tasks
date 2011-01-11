using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	public class FixedStartDependency : Dependency
	{
		private DateTime mStartDate;

		public FixedStartDependency(DateTime startDate)
			: base(null, DependencyPriority.High, new TimeSpan(0))
		{
			mStartDate = startDate;
		}

		public override DateTime StartDate(Week week)
		{
			return mStartDate;
		}
	}
}
