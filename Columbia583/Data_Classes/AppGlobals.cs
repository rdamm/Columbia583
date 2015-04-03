using System;
using SQLite;

namespace Columbia583
{
	public class AppGlobals
	{
		// Define the constant for the app globals row ID.
		public const int AppGlobalsRowId = 1;

		// Keys and references.
		[PrimaryKey]
		public int id { get; set; }

		// Data
		public int activeUserId { get; set; }
		public DateTime databaseLastUpdated { get; set; }


		public AppGlobals ()
		{
			this.id = AppGlobalsRowId;
		}


		public AppGlobals (int activeUserId, DateTime databaseLastUpdated)
		{
			this.id = AppGlobalsRowId;
			this.activeUserId = activeUserId;
			this.databaseLastUpdated = databaseLastUpdated;
		}
	}
}

