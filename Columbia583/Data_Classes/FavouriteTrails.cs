using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;

namespace Columbia583
{
	public class FavouriteTrails : IEquatable<FavouriteTrails>
	{
		// Keys and references.
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }
		[ForeignKey(typeof(User))]
		public int userId { get; set; }
		[ForeignKey(typeof(Trail))]
		public int trailId { get; set; }

		public FavouriteTrails ()
		{
			
		}

		public FavouriteTrails (int userId, int trailId)
		{
			this.userId = userId;
			this.trailId = trailId;
		}


		/// <param name="other">The object to be compared to this.  Needed for equatable.</param>
		/// <summary>
		/// Checks whether this and the given object are equal by comparing IDs.
		/// </summary>
		/// <returns>True if the IDs are the same, false otherwise.</returns>
		public bool Equals(FavouriteTrails other)
		{
			// Check whether the compared object is null. 
			if (Object.ReferenceEquals(other, null)) return false;

			// Check whether the compared object references the same data. 
			if (Object.ReferenceEquals(this, other)) return true;

			// Check whether the IDs are equal.
			return userId.Equals(other.userId) && trailId.Equals(other.trailId);
		}


		/// <summary>
		/// Gets the hash code for this object.  Needed for equatable.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			return userId ^ trailId;
		}
	}
}

