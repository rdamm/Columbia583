using System;
using SQLite;

namespace Columbia583
{
	public class Media
	{
		// Keys and references.
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }

		// Data.
		public string title { get; set; }
		public string mediaType { get; set; }
		public string mediaUrl { get; set; }
		public DateTime timestamp { get; set; }

		public Media()
		{

		}

		public Media (int id, string title, string mediaType, string mediaUrl, DateTime timestamp)
		{
			this.id = id;
			this.title = title;
			this.mediaType = mediaType;
			this.mediaUrl = mediaUrl;
			this.timestamp = timestamp;
		}
	}
}

