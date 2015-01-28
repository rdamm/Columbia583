using System;
using SQLite;

namespace Columbia583
{
	public class Amenity
	{
		// Keys and references.
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }

		// Data.
		public string amenityName { get; set; }
		public string amenityIcon { get; set; }
		public DateTime timestamp { get; set; }

		public Amenity()
		{

		}

		public Amenity (int id, string amenityName, string amenityIcon, DateTime timestamp)
		{
			this.id = id;
			this.amenityName = amenityName;
			this.amenityIcon = amenityIcon;
			this.timestamp = timestamp;
		}
	}
}

