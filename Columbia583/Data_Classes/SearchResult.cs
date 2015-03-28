using System;

namespace Columbia583
{
	public class SearchResult
	{
		public Trail trail { get; set; }
		public Point[] points { get; set; }
		public activity[] activities { get; set; }
		public Amenity[] amenities { get; set; }

		public SearchResult ()
		{

		}

		public SearchResult (Trail trail, Point[] points, activity[] activities, Amenity[] amenities)
		{
			this.trail = trail;
			this.points = points;
			this.activities = activities;
			this.amenities = amenities;
		}
	}
}

