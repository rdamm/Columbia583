﻿using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;

namespace Columbia583
{
	public class TrailsToActivities
	{
		// Keys and references.
		[ForeignKey(typeof(Trail))]
		public int trailId { get; set; }
		[ForeignKey(typeof(Activity))]
		public int activityId { get; set; }

		public TrailsToActivities ()
		{

		}

		public TrailsToActivities(int trailId, int activityId)
		{
			this.trailId = trailId;
			this.activityId = activityId;
		}
	}
}

