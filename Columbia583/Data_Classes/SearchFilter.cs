using System;

namespace Columbia583
{
	public class SearchFilter
	{

		public int[] activities { get; set; }
		public int[] amenities { get; set; }
		public int difficulty { get; set; }
		public int rating { get; set; }
		public int minDuration { get; set; }
		public int maxDuration { get; set; }
		public int minDistance { get; set; }
		public int maxDistance { get; set; }

		public SearchFilter()
		{

		}

		public SearchFilter (int[] activities, int[] amenities, int difficulty, int rating, int minDuration, int maxDuration, int minDistance, int maxDistance)
		{
			this.activities = activities;
			this.amenities = amenities;
			this.difficulty = difficulty;
			this.rating = rating;
			this.minDuration = minDuration;
			this.maxDuration = maxDuration;
			this.minDistance = minDistance;
			this.maxDistance = maxDistance;
		}
	}
}

