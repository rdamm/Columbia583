using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;

namespace Columbia583
{
	public class Media
	{
		// TODO: Update the last accessed timestamp whenever a row is retrieved.

		// Keys and references.
		[PrimaryKey]
		public int id { get; set; }
		[ForeignKey(typeof(Trail))]
		public int trailId { get; set; }

		// Data.
		public string title { get; set; }
		public string mediaType { get; set; }
		public string mediaUrl { get; set; }
		public byte[] mediaImage { get; set; }
		public DateTime lastAccessed { get; set; }
		public DateTime createdAt { get; set; }
		public DateTime updatedAt { get; set; }
		public bool pushToServer { get; set; }

		public Media()
		{

		}

		public Media (int id, int trailId, string title, string mediaType, string mediaUrl, byte[] mediaImage, DateTime lastAccessed, DateTime createdAt, DateTime updatedAt, bool pushToServer)
		{
			this.id = id;
			this.trailId = trailId;
			this.title = title;
			this.mediaType = mediaType;
			this.mediaUrl = mediaUrl;
			this.mediaImage = mediaImage;
			this.lastAccessed = lastAccessed;
			this.createdAt = createdAt;
			this.updatedAt = updatedAt;
			this.pushToServer = pushToServer;
		}
	}
}

