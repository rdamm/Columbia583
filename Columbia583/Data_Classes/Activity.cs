using System;
using SQLite;

namespace Columbia583
{
	public class Activity
	{
		// Keys and references.
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }

		// Data.
		public string activityName { get; set; }
		public string activityIcon { get; set; }
		public DateTime timestamp { get; set; }

		public Activity()
		{

		}

		public Activity (int id, string activityName, string activityIcon, DateTime timestamp)
		{
			this.id = id;
			this.activityName = activityName;
			this.activityIcon = activityIcon;
			this.timestamp = timestamp;
		}
	}
}

