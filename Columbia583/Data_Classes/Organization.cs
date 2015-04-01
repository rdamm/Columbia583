using System;
using SQLite;

namespace Columbia583
{
	public class Organization : IEquatable<Organization>
	{
		// Keys and references.
		[PrimaryKey]
		public int id { get; set; }

		// Data.
		public string organization { get; set; }
		public DateTime timestamp { get; set; }
		
		public Organization()
		{

		}

		public Organization (int id, string organization, DateTime timestamp)
		{
			this.id = id;
			this.organization = organization;
			this.timestamp = timestamp;
		}


		/// <param name="other">The object to be compared to this.  Needed for equatable.</param>
		/// <summary>
		/// Checks whether this and the given object are equal by comparing IDs.
		/// </summary>
		/// <returns>True if the IDs are the same, false otherwise.</returns>
		public bool Equals(Organization other)
		{
			// Check whether the compared object is null. 
			if (Object.ReferenceEquals(other, null)) return false;

			// Check whether the compared object references the same data. 
			if (Object.ReferenceEquals(this, other)) return true;

			// Check whether the IDs are equal.
			return id.Equals(other.id);
		}


		/// <summary>
		/// Gets the hash code for this object.  Needed for equatable.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			return id;
		}
	}
}

