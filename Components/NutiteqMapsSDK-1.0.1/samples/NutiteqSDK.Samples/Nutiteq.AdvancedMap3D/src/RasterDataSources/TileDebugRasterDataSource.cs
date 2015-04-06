using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Nutiteq.SDK;

/// <summary>
/// Virtual raster data source that generates tile bitmap containing tile information (x/y/z) for each tile.
/// </summary>
namespace Nutiteq.AdvancedMap3D
{
	/// <summary>
	/// Tile debug raster data source.
	/// </summary>
	public class TileDebugRasterDataSource : AbstractRasterDataSource
	{
		private int tileSize;

		public TileDebugRasterDataSource (Projection projection, int minZoom, int maxZoom, int tileSize) : base (projection, minZoom, maxZoom)
		{
			this.tileSize = tileSize;
		}

		public override TileBitmap LoadTile (MapTile tile)
		{
			string tileId = "" + tile.Zoom + "/" + tile.X + "/" + tile.Y;
			return new TileBitmap (DrawTextToBitmap (tileId));
		}

		private Bitmap DrawTextToBitmap (String text)
		{
			Bitmap bitmap = Bitmap.CreateBitmap (tileSize, tileSize, Bitmap.Config.Argb8888);
			Canvas canvas = new Canvas (bitmap);

			// new antialised Paint
			Paint paint = new Paint (PaintFlags.AntiAlias);
			// text color - #3D3D3D
			paint.Color = Android.Graphics.Color.Rgb (61, 61, 61);
			// text size in pixels
			paint.TextSize = 32;
			// text shadow
			paint.SetShadowLayer (1f, 0f, 1f, Android.Graphics.Color.White);

			// draw text to the Canvas center
			Rect bounds = new Rect ();
			paint.GetTextBounds (text, 0, text.Length, bounds);
			int x = (256 - bounds.Width()) / 2;
			int y = (bitmap.Height + bounds.Height()) / 2;

			// Resources resources = context.getResources();
			// float scale = resources.getDisplayMetrics().density;
			// canvas.drawText(gText, x * scale, y * scale, paint);
			canvas.DrawText (text, x, y, paint);

			paint.Color = Android.Graphics.Color.Rgb (128, 128, 128);
			canvas.DrawLine (0, 0, 0, tileSize, paint);
			canvas.DrawLine (0, tileSize, tileSize, tileSize, paint);
			canvas.DrawLine (tileSize, tileSize, tileSize, 0, paint);
			canvas.DrawLine (tileSize, 0, 0, 0, paint);
			return bitmap;
		}
	}
}

