using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingWeek;

namespace Tasks
{
	/// <summary>
	/// Defines a "finished finish dependency", in which the task will
	/// finish on a user-specified date.  This is a high priority
	/// dependency.
	/// </summary>
	public class FixedFinishDependency : Dependency
	{
		#region Members

		/// <summary>
		/// The date on which the dependency will end.
		/// </summary>
		private DateTime mEndDate;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the FixedFinishDependency class.
		/// </summary>
		/// <param name="endDate">The date on which the dependency will end.</param>
		public FixedFinishDependency(DateTime endDate)
			: base(null)
		{
			mEndDate = endDate;
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
			return week.DateAdd(mEndDate, Owner.Duration.Negate());
		}

		#endregion
	}
}
