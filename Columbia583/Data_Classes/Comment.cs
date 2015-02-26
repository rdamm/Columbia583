﻿using System;
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
		[ForeignKey(typeof(User))]
		public int userId { get; set; }
		[ForeignKey(typeof(Trail))]
		public int trailId { get; set; }

		// Data
		public string text { get; set; }
		public DateTime date { get; set; }
		public int rating { get; set; }

		public Comment ()
		{
		}

		public Comment(int id, int userId, int trailId, string text, int rating, DateTime dt)
		{
			this.id = id;
			this.userId = userId;
			this.trailId = trailId;
			this.text = text;
			this.rating = rating;
			date = dt;
		}
	}
}