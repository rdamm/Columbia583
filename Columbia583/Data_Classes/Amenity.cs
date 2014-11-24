using System;

namespace Columbia583
{
	public class Amenity
	{
		private int id;
		private string amenityName;
		private string amenityIcon;
		private DateTime timestamp;

		public Amenity (int id, string amenityName, string amenityIcon, DateTime timestamp)
		{
			this.id = id;
			this.amenityName = amenityName;
			this.amenityIcon = amenityIcon;
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

		public string AmenityName {
			get {
				return amenityName;
			}
			set {
				amenityName = value;
			}
		}

		public string AmenityIcon {
			get {
				return amenityIcon;
			}
			set {
				amenityIcon = value;
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

