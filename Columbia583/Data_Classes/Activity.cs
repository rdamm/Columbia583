using System;

namespace Columbia583
{
	public class Activity
	{
		public int id { get; set; }
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

