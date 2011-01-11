using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	/// <summary>
	/// The Project class represents a single project.  A project consists of a task/dependency
	/// structure and a working week definition.
	/// </summary>
	public class Project
	{
		#region Properties

		/// <summary>
		/// Gets the list of tasks.
		/// </summary>
		public List<ITask> Tasks { get; private set; }

		/// <summary>
		/// Gets or sets the project name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the start date of the project.
		/// </summary>
		public DateTime StartDate { get; set; }

		/// <summary>
		/// Gets or sets the working week definition used by the project when calculating
		/// task dates.
		/// </summary>
		public Week Week { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Project class.
		/// </summary>
		/// <param name="name">The name of the project.</param>
		/// <param name="startDate">The start date of the project.</param>
		/// <param name="week">The working week definition to use when calculating task dates.</param>
		public Project(string name, DateTime startDate, Week week)
		{
			Name = name;
			StartDate = startDate;
			Week = week;

			Tasks = new List<ITask>();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds a new task to the project.
		/// </summary>
		/// <param name="task">The task to add to the project.</param>
		public void AddTask(ITask task)
		{
			Tasks.Add(task);
		}

		/// <summary>
		/// Recalculates all dates of the project tasks.  Should be called
		/// when changes have been made to any tasks.
		/// </summary>
		public void RecalculateDates()
		{
			var calc = new Tasks.DateCalculator();
			calc.RecalculateDates(Tasks, StartDate, Week);
		}

		#endregion
	}
}
