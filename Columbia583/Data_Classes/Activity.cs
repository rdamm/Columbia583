using System;

namespace Columbia583
{
	public class Activity
	{
		private int id;
		private string activityName;
		private string activityIcon;
		private DateTime timestamp;

		public Activity (int id, string activityName, string activityIcon, DateTime timestamp)
		{
			this.id = id;
			this.activityName = activityName;
			this.activityIcon = activityIcon;
			this.timestamp = timestamp;
		}

		public int Id {
			get {
				return id;
			}
			set {
				id = value;
			}
		}

		public string ActivityName {
			get {
				return activityName;
			}
			set {
				activityName = value;
			}
		}

		public string ActivityIcon {
			get {
				return activityIcon;
			}
			set {
				activityIcon = value;
			}
		}

		public DateTime Timestamp {
			get {
				return timestamp;
			}
			set {
				timestamp = value;
			}
		}
	}
}

