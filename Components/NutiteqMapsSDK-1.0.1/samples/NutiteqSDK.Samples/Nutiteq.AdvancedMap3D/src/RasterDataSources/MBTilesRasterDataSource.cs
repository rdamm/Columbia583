using System;
using System.Data;
using Mono.Data.Sqlite;
using Nutiteq.SDK;

/// <summary>
/// Offline MapBox raster data source. Tiles are stored in sqlite 'tiles' table as compressed bitmaps
/// </summary>
namespace Nutiteq.AdvancedMap3D
{
	/// <summary>
	/// Offline MapBox tiles raster data source.
	/// </summary>
	public class MBTilesRasterDataSource : AbstractRasterDataSource
	{
		private const String TILE_TABLE = "tiles";
		private const String KEY_ZOOM = "zoom_level";
		private const String KEY_X = "tile_column";
		private const String KEY_Y = "tile_row";
		private const String KEY_TILE_DATA = "tile_data";

		private IDbConnection _connection;

		public MBTilesRasterDataSource (Projection projection, int minZoom, int maxZoom, String path) : base (projection, minZoom, maxZoom)
		{
			_connection = new SqliteConnection ("Data Source=" + path);
			_connection.Open ();
		}

		public override TileBitmap LoadTile (MapTile tile)
		{
			// Query compressed tile bitmap using given tile coordinates and zoom
			IDbCommand dbcmd = _connection.CreateCommand ();
			// TMS/MBTiles requires flipped y
			int yFlipped = (1 << tile.Zoom) - 1 - tile.Y;
			dbcmd.CommandText = "SELECT LENGTH(" + KEY_TILE_DATA + ")," + KEY_TILE_DATA + " FROM " + TILE_TABLE + " WHERE " + KEY_ZOOM + "=" + tile.Zoom + " AND " + KEY_X + "=" + tile.X + " AND " + KEY_Y + "=" + yFlipped;

			IDataReader reader = dbcmd.ExecuteReader ();
			byte[] img = null;
			if (reader.Read ()) {
				int len = reader.GetInt32 (0);
				img = new byte[len];
				reader.GetBytes (1, 0, img, 0, len);
			}
			reader.Close ();
			dbcmd.Dispose ();

			// If succeeded, return compressed tile bitmap
			if (img == null)
				return null;
			return new TileBitmap(img);
		}
	}
}

