using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	public enum DependencyPriority
	{
		Low = 0,
		Normal = 1,
		High = 2
	}

	public interface IDependency
	{
		DependencyPriority Priority { get; }
		ITask Owner { get; set; }
		ITask DependentOn { get; }
		TimeSpan Lag { get; set; }

		DateTime StartDate(Week week);
	}
}
