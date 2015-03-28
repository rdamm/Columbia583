using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;

namespace Columbia583
{
	public class Trail : IEquatable<Trail>
	{
		// Keys and references.
		[PrimaryKey]
		public int id { get; set; }
		[ForeignKey(typeof(User))]
		public int userId { get; set; }
		[ForeignKey(typeof(Organization))]
		public int orgId { get; set; }

		// Data.
		public string name { get; set; }
		public string location { get; set; }
		public string kmlName { get; set; }
		public string kmlContent { get; set; }
		public string distance { get; set; }
		public string duration { get; set; }
		public string description { get; set; }
		public string directions { get; set; }
		public Difficulty difficulty { get; set; }
		public int rating { get; set; }
		public string hazards { get; set; }
		public string surface { get; set; }
		public string landAccess { get; set; }
		public string maintenance { get; set; }
		public string season { get; set; }
		public bool open { get; set; }
		public bool active { get; set; }
		//public DateTime timestamp { get; set; }
		public DateTime date_uploaded { get; set; }
		public DateTime date_created{ get; set; }
		public bool flag_upload{ get; set; }

		public Trail()
		{

		}

		public Trail (int id, int userId, int orgId, string name, string location, string kmlName, string kmlContent, string distance, string duration, string description, string directions,
			Difficulty difficulty, int rating, string hazards, string surface, string landAccess, string maintenance, string season, bool open, bool active, DateTime date_uploaded, DateTime date_created, bool flag_upload)
		{
			this.id = id;
			this.userId = userId;
			this.orgId = orgId;
			this.name = name;
			this.location = location;
			this.kmlName = kmlName;
			this.kmlContent = kmlContent;
			this.distance = distance;
			this.duration = duration;
			this.description = description;
			this.directions = directions;
			this.difficulty = difficulty;
			this.rating = rating;
			this.hazards = hazards;
			this.surface = surface;
			this.landAccess = landAccess;
			this.maintenance = maintenance;
			this.season = season;
			this.open = open;
			this.active = active;
			//this.timestamp = timestamp;
			this.date_created = date_created;
			this.date_uploaded = date_uploaded;
			this.flag_upload = flag_upload;
		}


		/// <param name="other">The object to be compared to this.  Needed for equatable.</param>
		/// <summary>
		/// Checks whether this and the given object are equal by comparing IDs.
		/// </summary>
		/// <returns>True if the IDs are the same, false otherwise.</returns>
		public bool Equals(Trail other)
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

