using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;

namespace Columbia583
{
	public class TrailsToActivities : IEquatable<TrailsToActivities>
	{
		// Keys and references.
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }
		[ForeignKey(typeof(Trail))]
		public int trailId { get; set; }
		[ForeignKey(typeof(Activity))]
		public int activityId { get; set; }

		public TrailsToActivities ()
		{

		}

		public TrailsToActivities(int trailId, int activityId)
		{
			this.trailId = trailId;
			this.activityId = activityId;
		}


		/// <param name="other">The object to be compared to this.  Needed for equatable.</param>
		/// <summary>
		/// Checks whether this and the given object are equal by comparing IDs.
		/// </summary>
		/// <returns>True if the IDs are the same, false otherwise.</returns>
		public bool Equals(TrailsToActivities other)
		{
			// Check whether the compared object is null. 
			if (Object.ReferenceEquals(other, null)) return false;

			// Check whether the compared object references the same data. 
			if (Object.ReferenceEquals(this, other)) return true;

			// Check whether the IDs are equal.
			return trailId.Equals(other.trailId) && activityId.Equals(other.activityId);
		}


		/// <summary>
		/// Gets the hash code for this object.  Needed for equatable.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			return trailId ^ activityId;
		}
	}
}

