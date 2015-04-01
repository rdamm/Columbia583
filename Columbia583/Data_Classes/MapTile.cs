using System;
using SQLite;

namespace Columbia583
{
	public class MapTile
	{
		// TODO: Update the last accessed timestamp whenever a row is retrieved.

		// Keys and references.
		[PrimaryKey]
		public int id { get; set; }

		// Data.
		public double originLon { get; set; }
		public double originLat { get; set; }
		public double sizeLon { get; set; }
		public double sizeLat { get; set; }
		public string tileFilename { get; set; }
		public byte[] tileImage { get; set; }
		public DateTime lastAccessed { get; set; }
		public DateTime timestamp { get; set; }

		public MapTile()
		{

		}

		public MapTile (int id, double originLon, double originLat, double sizeLon, double sizeLat, string tileFilename, byte[] tileImage, DateTime lastAccessed, DateTime timestamp)
		{
			this.id = id;
			this.originLon = originLon;
			this.originLat = originLat;
			this.sizeLon = sizeLon;
			this.sizeLat = sizeLat;
			this.tileFilename = tileFilename;
			this.tileImage = tileImage;
			this.lastAccessed = lastAccessed;
			this.timestamp = timestamp;
		}
	}
}

