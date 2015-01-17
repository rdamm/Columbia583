using System;

namespace Columbia583
{
	public class Trail
	{
		public int id { get; set; }
		public int userId { get; set; }
		public int orgId { get; set; }
		public string name { get; set; }
		public string location { get; set; }
		public string kmlUrl { get; set; }
		public string kmlContent { get; set; }
		public string distance { get; set; }
		public string duration { get; set; }
		public string description { get; set; }
		public string directions { get; set; }
		public Difficulty difficulty { get; set; }
		public int rating { get; set; }
		public int[] activityIDs { get; set; }
		public int[] amenityIDs { get; set; }
		public string hazards { get; set; }
		public string surface { get; set; }
		public string landAccess { get; set; }
		public string maintenance { get; set; }
		public string season { get; set; }
		public bool open { get; set; }
		public bool active { get; set; }
		public DateTime timestamp { get; set; }

		public Trail()
		{

		}

		public Trail (int id, int userId, int orgId, string name, string location, string kmlUrl,
			string kmlContent, string distance, string duration, string description, string directions,
			Difficulty difficulty, int rating, int[] activityIDs, int[] amenityIDs, string hazards, string surface, string landAccess, string maintenance, string season,
			bool open, bool active, DateTime timestamp)
		{
			this.id = id;
			this.userId = userId;
			this.orgId = orgId;
			this.name = name;
			this.location = location;
			this.kmlUrl = kmlUrl;
			this.kmlContent = kmlContent;
			this.distance = distance;
			this.duration = duration;
			this.description = description;
			this.directions = directions;
			this.difficulty = difficulty;
			this.rating = rating;
			this.activityIDs = activityIDs;
			this.amenityIDs = amenityIDs;
			this.hazards = hazards;
			this.surface = surface;
			this.landAccess = landAccess;
			this.maintenance = maintenance;
			this.season = season;
			this.open = open;
			this.active = active;
			this.timestamp = timestamp;
		}
	}
}

