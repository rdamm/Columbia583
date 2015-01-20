using System;
using SQLite;

namespace Columbia583
{
	public class MapTile
	{
		// Keys and references.
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }

		// Data.
		public double originLon { get; set; }
		public double originLat { get; set; }
		public double sizeLon { get; set; }
		public double sizeLat { get; set; }
		public string tileFilename { get; set; }
		public DateTime timestamp { get; set; }

		public MapTile()
		{

		}

		public MapTile (int id, double originLon, double originLat, double sizeLon, double sizeLat, string tileFilename, DateTime timestamp)
		{
			this.id = id;
			this.originLon = originLon;
			this.originLat = originLat;
			this.sizeLon = sizeLon;
			this.sizeLat = sizeLat;
			this.tileFilename = tileFilename;
			this.timestamp = timestamp;
		}
	}
}

