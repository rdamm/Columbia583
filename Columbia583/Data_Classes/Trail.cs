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
		private Rating rating;
		private Activity[] activities;
		private Amenity[] amenities;
		private string hazards;
		private string surface;
		private string landAccess;
		private string maintenance;
		private bool open;
		private bool active;
		private DateTime timestamp;

		public Trail (int id, int userId, int orgId, string name, string location, string kmlUrl,
			string kmlContent, string distance, string duration, string description, string directions,
			Difficulty difficulty, Rating rating, Activity[] activities, Amenity[] amenities, string hazards, string surface, string landAccess, string maintenance,
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
			this.activities = activities;
			this.hazards = hazards;
			this.surface = surface;
			this.landAccess = landAccess;
			this.maintenance = maintenance;
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

		public Rating Rating {
			get {
				return rating;
			}
			set {
				rating = value;
			}
		}

		public Activity[] Activities {
			get {
				return activities;
			}
			set {
				activities = value;
			}
		}

		public Amenity[] Amenities {
			get {
				return amenities;
			}
			set {
				amenities = value;
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

