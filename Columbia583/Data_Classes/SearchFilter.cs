using System;

namespace Columbia583
{
	public class SearchFilter
	{
		private string[] activities;
		private Difficulty[] difficulties;
		private int minDuration;
		private int maxDuration;
		private int minDistance;
		private int maxDistance;

		public SearchFilter (string[] activities, Difficulty[] difficulties, int minDuration, int maxDuration, int minDistance, int maxDistance)
		{
			this.activities = activities;
			this.difficulties = difficulties;
			this.minDuration = minDuration;
			this.maxDuration = maxDuration;
			this.minDistance = minDistance;
			this.maxDistance = maxDistance;
		}

		public string[] Activities {
			get {
				return activities;
			}
			set {
				activities = value;
			}
		}

		public Difficulty[] Difficulties {
			get {
				return difficulties;
			}
			set {
				difficulties = value;
			}
		}

		public int MinDuration {
			get {
				return minDuration;
			}
			set {
				minDuration = value;
			}
		}

		public int MaxDuration {
			get {
				return maxDuration;
			}
			set {
				maxDuration = value;
			}
		}

		public int MinDistance {
			get {
				return minDistance;
			}
			set {
				minDistance = value;
			}
		}

		public int MaxDistance {
			get {
				return maxDistance;
			}
			set {
				maxDistance = value;
			}
		}
	}
}

