using System;
using SQLite;

namespace Columbia583
{
	public class Organization
	{
		// Keys and references.
		[PrimaryKey, AutoIncrement]
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
	}
}

