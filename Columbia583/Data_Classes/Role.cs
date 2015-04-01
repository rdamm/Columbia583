using System;
using SQLite;

namespace Columbia583
{
	public class Role
	{
		// Keys and references.
		[PrimaryKey]
		public int id { get; set; }

		// Data.
		public string name { get; set; }
		public DateTime timestamp { get; set; }

		public Role()
		{

		}

		public Role (int id, string name, DateTime timestamp)
		{
			this.id = id;
			this.name = name;
			this.timestamp = timestamp;
		}
	}
}

