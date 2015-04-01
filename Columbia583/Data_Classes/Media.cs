using System;

namespace Columbia583
{
	public class Media
	{
		public int id { get; set; }
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

