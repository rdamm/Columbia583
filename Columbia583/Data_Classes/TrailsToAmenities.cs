using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;

namespace Columbia583
{
	public class TrailsToAmenities : IEquatable<TrailsToAmenities>
	{
		// Keys and references.
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }
		[ForeignKey(typeof(Trail))]
		public int trailId { get; set; }
		[ForeignKey(typeof(Amenity))]
		public int amenityId { get; set; }

		public TrailsToAmenities ()
		{

		}

		public TrailsToAmenities(int trailId, int amenityId)
		{
			this.trailId = trailId;
			this.amenityId = amenityId;
		}


		/// <param name="other">The object to be compared to this.  Needed for equatable.</param>
		/// <summary>
		/// Checks whether this and the given object are equal by comparing IDs.
		/// </summary>
		/// <returns>True if the IDs are the same, false otherwise.</returns>
		public bool Equals(TrailsToAmenities other)
		{
			// Check whether the compared object is null. 
			if (Object.ReferenceEquals(other, null)) return false;

			// Check whether the compared object references the same data. 
			if (Object.ReferenceEquals(this, other)) return true;

			// Check whether the IDs are equal.
			return trailId.Equals(other.trailId) && amenityId.Equals(other.amenityId);
		}


		/// <summary>
		/// Gets the hash code for this object.  Needed for equatable.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			return trailId ^ amenityId;
		}
	}
}

