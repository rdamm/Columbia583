using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;

namespace Columbia583
{
	public class Comment
	{
		// Keys
		[PrimaryKey]
		public int id { get; set; }
		[ForeignKey(typeof(Trail))]
		public int trailId { get; set; }

		// Data
		public string text { get; set; }
		public int rating { get; set; }
		public string username { get; set; }
		public DateTime createdAt { get; set; }
		public DateTime updatedAt { get; set; }
		public bool pushToServer { get; set; }

		public Comment ()
		{
			
		}

		public Comment(int id, int trailId, string text, int rating, string username, DateTime createdAt, DateTime updatedAt, bool pushToServer)
		{
			this.id = id;
			this.trailId = trailId;
			this.text = text;
			this.rating = rating;
			this.username = username;

			this.createdAt = createdAt;
			this.updatedAt = updatedAt;
			this.pushToServer = pushToServer;
		}
	}
}
