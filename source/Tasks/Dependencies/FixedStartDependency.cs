using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	/// <summary>
	/// Defines a "finished start dependency", in which the task will
	/// start on a user-specified date.  This is a high priority
	/// dependency.
	/// </summary>
	public class FixedStartDependency : Dependency
	{
		#region Members

		/// <summary>
		/// The date on which the dependency will start.
		/// </summary>
		private DateTime mStartDate;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the FixedStartDependency class.
		/// </summary>
		/// <param name="startDate">The date on which the dependency will start.</param>
		public FixedStartDependency(DateTime startDate)
			: base(null)
		{
			mStartDate = startDate;
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
			return mStartDate;
		}

		#endregion
	}
}
