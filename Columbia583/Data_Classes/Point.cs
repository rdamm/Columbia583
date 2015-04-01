using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;

namespace Columbia583
{
	public class Point
	{
		// Keys and references.
		[PrimaryKey]
		public int id { get; set; }
		[ForeignKey(typeof(Trail))]
		public int trailId { get; set; }
		[ForeignKey(typeof(MapTile))]
		public int mapTileId { get; set; }
		[ForeignKey(typeof(Point))]
		public int nextPointId { get; set; }

		// Data.
		public string title { get; set; }
		public string description { get; set; }
		public double lat { get; set; }
		public double lon { get; set; }
		public bool primaryPoint { get; set; }
		public DateTime timestamp { get; set; }

		public Point()
		{

		}

		public Point (int id, int trailId, int mapTileId, int nextPointId, string title, string description, double lat, double lon, bool primaryPoint, DateTime timestamp)
		{
			this.id = id;
			this.trailId = trailId;
			this.mapTileId = mapTileId;
			this.title = title;
			this.description = description;
			this.nextPointId = nextPointId;
			this.lat = lat;
			this.lon = lon;
			this.primaryPoint = primaryPoint;
			this.timestamp = timestamp;
		}
	}
}

