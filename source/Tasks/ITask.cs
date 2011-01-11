using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	public interface ITask
	{
		string Name { get; set; }
		DateTime StartDate { get; }
		DateTime EndDate { get; }
		List<IDependency> Dependencies { get; }
		TimeSpan Duration { get; set; }
		int Order { get; set; }
		ITask Parent { get; set; }
		List<ITask> Children { get; }
		bool HasChildren { get; }
		bool HasDependencies { get; }
		void RecalculateDates(DateTime earliestDate, Week week);
		List<ITask> GetAllTaskDependencies();
	}
}
