using System;
using SQLite;

namespace Columbia583
{
	public class DatabaseLastUpdated
	{
		// Keys and references.
		[PrimaryKey]
		public DateTime timestamp { get; set; }

		public DatabaseLastUpdated ()
		{

		}


		public DatabaseLastUpdated(DateTime timestamp)
		{
			this.timestamp = timestamp;
		}
	}
}

