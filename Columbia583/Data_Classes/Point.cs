using System;

namespace Columbia583
{
	public class Point
	{
		public int id { get; set; }
		public int trailId { get; set; }
		public int mapTileId { get; set; }
		public string title { get; set; }
		public string description { get; set; }
		public Point point { get; set; }
		public double lat { get; set; }
		public double lon { get; set; }
		public bool primary { get; set; }
		public DateTime timestamp { get; set; }

		public Point()
		{

		}

		public Point (int id, int trailId, int mapTileId, string title, string description, Point point, double lat, double lon, bool primary, DateTime timestamp)
		{
			this.id = id;
			this.trailId = trailId;
			this.mapTileId = mapTileId;
			this.title = title;
			this.description = description;
			this.point = point;
			this.lat = lat;
			this.lon = lon;
			this.primary = primary;
			this.timestamp = timestamp;
		}
	}
}

