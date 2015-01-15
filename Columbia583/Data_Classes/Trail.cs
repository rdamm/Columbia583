using System;

namespace Columbia583
{
	public class Trail
	{
		private int id;
		private int userId;
		private int orgId;
		private string name;
		private string location;
		private string kmlUrl;
		private string kmlContent;
		private string distance;
		private string duration;
		private string description;
		private string directions;
		private Difficulty difficulty;
		private int rating;
		private int[] activityIDs;
		private int[] amenityIDs;
		private string hazards;
		private string surface;
		private string landAccess;
		private string maintenance;
		private string season;
		private bool open;
		private bool active;
		private DateTime timestamp;

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

		public int Id {
			get {
				return id;
			}
			set {
				id = value;
			}
		}

		public int UserId {
			get {
				return userId;
			}
			set {
				userId = value;
			}
		}

		public int OrgId {
			get {
				return orgId;
			}
			set {
				orgId = value;
			}
		}

		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}

		public string Location {
			get {
				return location;
			}
			set {
				location = value;
			}
		}

		public string KmlUrl {
			get {
				return kmlUrl;
			}
			set {
				kmlUrl = value;
			}
		}

		public string KmlContent {
			get {
				return kmlContent;
			}
			set {
				kmlContent = value;
			}
		}

		public string Distance {
			get {
				return distance;
			}
			set {
				distance = value;
			}
		}

		public string Duration {
			get {
				return duration;
			}
			set {
				duration = value;
			}
		}

		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}

		public string Directions {
			get {
				return directions;
			}
			set {
				directions = value;
			}
		}

		public Difficulty Difficulty {
			get {
				return difficulty;
			}
			set {
				difficulty = value;
			}
		}

		public int Rating {
			get {
				return rating;
			}
			set {
				rating = value;
			}
		}

		public int[] ActivityIDs {
			get {
				return activityIDs;
			}
			set {
				activityIDs = value;
			}
		}

		public int[] AmenityIDs {
			get {
				return amenityIDs;
			}
			set {
				amenityIDs = value;
			}
		}

		public string Hazards {
			get {
				return hazards;
			}
			set {
				hazards = value;
			}
		}

		public string Surface {
			get {
				return surface;
			}
			set {
				surface = value;
			}
		}

		public string LandAccess {
			get {
				return landAccess;
			}
			set {
				landAccess = value;
			}
		}

		public string Maintenance {
			get {
				return maintenance;
			}
			set {
				maintenance = value;
			}
		}

		public string Season {
			get {
				return season;
			}
			set {
				season = value;
			}
		}

		public bool Open {
			get {
				return open;
			}
			set {
				open = value;
			}
		}

		public bool Active {
			get {
				return active;
			}
			set {
				active = value;
			}
		}

		public DateTime Timestamp {
			get {
				return timestamp;
			}
			set {
				timestamp = value;
			}
		}
	}
}

