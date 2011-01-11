using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	public class Project
	{
		public List<ITask> Tasks { get; private set; }
		public string Name { get; set; }
		public DateTime StartDate { get; set; }
		public Week Week { get; set; }

		public Project(string name, DateTime startDate, Week week)
		{
			Name = name;
			StartDate = startDate;
			Week = week;

			Tasks = new List<ITask>();
		}

		public void AddTask(ITask task)
		{
			Tasks.Add(task);
		}

		public void RecalculateDates()
		{
			var calc = new Tasks.DateCalculator();
			calc.RecalculateDates(Tasks, StartDate, Week);
		}
	}
}
