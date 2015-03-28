using System;
using SQLite;

namespace Columbia583
{
	public class Media
	{
		// TODO: Update the last accessed timestamp whenever a row is retrieved.

		// Keys and references.
		[PrimaryKey]
		public int id { get; set; }

		// Data.
		public string title { get; set; }
		public string mediaType { get; set; }
		public string mediaUrl { get; set; }
		public byte[] mediaImage { get; set; }
		public DateTime lastAccessed { get; set; }
		public DateTime timestamp { get; set; }

		public Media()
		{

		}

		public Media (int id, string title, string mediaType, string mediaUrl, byte[] mediaImage, DateTime lastAccessed, DateTime timestamp)
		{
			this.id = id;
			this.title = title;
			this.mediaType = mediaType;
			this.mediaUrl = mediaUrl;
			this.mediaImage = mediaImage;
			this.lastAccessed = lastAccessed;
			this.timestamp = timestamp;
		}
	}
}

