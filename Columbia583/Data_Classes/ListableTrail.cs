using System;

namespace Columbia583
{
	public class ListableTrail
	{
		public Trail trail { get; set; }
		public Point[] points { get; set; }
		public Activity[] activities { get; set; }
		public Amenity[] amenities { get; set; }

		public ListableTrail ()
		{

		}

		public ListableTrail (Trail trail, Point[] points, Activity[] activities, Amenity[] amenities)
		{
			this.trail = trail;
			this.points = points;
			this.activities = activities;
			this.amenities = amenities;
		}
	}
}

