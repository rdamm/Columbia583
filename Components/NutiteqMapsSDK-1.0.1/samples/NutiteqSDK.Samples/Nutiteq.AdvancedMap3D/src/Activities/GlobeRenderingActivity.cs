using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Graphics;
using Nutiteq.SDK;

namespace Nutiteq.AdvancedMap3D
{
	/// <summary>
	/// Demonstrates globe rendering and interaction using Nutiteq SDK.
	/// Globe rendering is implemented as a special render projection (spherical renderprojection) and this can be switched on/off any time.
	/// </summary>
	[Activity (Label = "GlobeRenderingActivity")]
	public class GlobeRenderingActivity : Activity
	{
		/// <summary>
		/// The Nutiteq MapView
		/// </summary>
		private MapView _mapView;
		/// <summary>
		/// The marker layer.
		/// </summary>
		private MarkerLayer _markerLayer;
		/// <summary>
		/// The marker layer.
		/// </summary>
		private GeometryLayer _geoLayer;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			/// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// enable Nutiteq SDK logging
			Log.EnableAll ();

			/// Get our map from the layout resource. 
			_mapView = FindViewById<MapView> (Resource.Id.mapView);

			/// Components keeps internal state and parameters for MapView
			_mapView.Components = new Components ();

			/// define the map layer, use EPSG4326 data source (EPSG4326 contains 2 root tiles and covers poles, EPSG3857 does not cover poles)
			IRasterDataSource dataSource = new HTTPRasterDataSource (new EPSG4326 (), 0, 5, "http://www.staremapy.cz/naturalearth/{zoom}/{x}/{yflipped}.png");
			RasterLayer mapLayer = new RasterLayer (dataSource, 0);

			/// Set online base layer  
			_mapView.Layers.BaseLayer = mapLayer;

			// Constrain zoom range as our tileset is defined only up to level 5 zoom
			_mapView.Constraints.ZoomRange = new Range (0, 5); // constrain

			/// Switch renderprojection - use spherical renderprojection instead of default flat projection
			_mapView.Options.RenderProjection = Options.SphericalRenderprojection;

			/// Set other options
			_mapView.Options.KineticPanning = true;
			_mapView.Options.KineticRotation = true;
			_mapView.Options.BackgroundPlaneDrawMode = Options.DrawBitmap;
			_mapView.Options.SetBackgroundPlaneBitmap (UnscaledBitmapLoader.DecodeResource (Resources, Resource.Drawable.background_plane));
			_mapView.Options.BackgroundImageDrawMode = Options.DrawBackdropBitmap;
			_mapView.Options.SetBackgroundImageBitmap (CreateBackdropImage ());

			_mapView.Options.SetTextureMemoryCacheSize (20 * 1024 * 1024);
			_mapView.Options.SetCompressedMemoryCacheSize (8 * 1024 * 1024);

			/// Start map
			_mapView.StartMapping ();

			// create layer and add object to the layer, finally add layer to the map. 
			// All overlay layers must be same projection as base layer, so we reuse it
			_markerLayer = new MarkerLayer (_mapView.Layers.BaseLayer.Projection);
			_mapView.Layers.AddLayer (_markerLayer);

			_geoLayer = new GeometryLayer (_mapView.Layers.BaseLayer.Projection);
			_mapView.Layers.AddLayer (_geoLayer);

			// zoom - 0 = world, like on most web maps
			_mapView.Zoom = 2f;

			// add markers/lines to the map
			MapPos sfPos = _mapView.Layers.BaseLayer.Projection.FromWgs84 (-122.416667f, 37.766667f);
			MapPos londonPos = _mapView.Layers.BaseLayer.Projection.FromWgs84 (0.0f, 51.0f);
			AddMarker ("San Francisco", "California", sfPos);
			AddMarker ("London", "United Kingdom", londonPos);
			AddLine (new MapPos[]{ sfPos, londonPos }, 0.2f, Nutiteq.SDK.Color.White, "example of line");

			// Hide zoom buttons
			ZoomControls zoomControls = FindViewById<ZoomControls> (Resource.Id.zoomcontrols);
			zoomControls.Visibility = ViewStates.Invisible;
		}

		private MarkerStyle.Builder DefineMarkerStyle (int resourceId, float size, float offset2D)
		{
			// define marker style 
			Bitmap pointMarker = UnscaledBitmapLoader.DecodeResource (Resources, resourceId);
			MarkerStyle.Builder markerStyleBuilder = new MarkerStyle.Builder ();
			markerStyleBuilder.SetBitmap (pointMarker);
			markerStyleBuilder.SetColor (Nutiteq.SDK.Color.White);
			markerStyleBuilder.SetSize (size);
			markerStyleBuilder.SetOffset2DX (offset2D);

			return markerStyleBuilder;
		}

		void AddMarker (string title, string subtitle, MapPos location)
		{
			/// Define label what is shown when you click on marker.
			Label markerLabel = new DefaultLabel (title, subtitle);

			/// define the marker style
			MarkerStyle markerStyle = DefineMarkerStyle (Resource.Drawable.olmarker, 0.5f, 0f).Build ();

			/// add the label to the Marker
			Marker marker = new Marker (location, markerLabel, markerStyle, _markerLayer);

			/// add the label to the layer
			_markerLayer.Add (marker);
		}

		private LineStyle.Builder DefineLineStyle (int lineResourceId, int pointResourceId, float lineWidth, float pointSize, int lineColor, int pointColor)
		{
			LineStyle.Builder lineStyleBuilder = new LineStyle.Builder ();
			Bitmap lineBitmap = UnscaledBitmapLoader.DecodeResource (Resources, lineResourceId);
			lineStyleBuilder.SetBitmap (lineBitmap);
			lineStyleBuilder.SetWidth (lineWidth);
			lineStyleBuilder.SetColor (lineColor);
			return lineStyleBuilder;
		}

		private void AddLine (MapPos[] points, float width, int color, string lineName)
		{
			LineStyle lineStyle = DefineLineStyle (Resource.Drawable.point, Resource.Drawable.point, 0.1f, 0.2f, Nutiteq.SDK.Color.White, Nutiteq.SDK.Color.Blue).Build ();
			StyleSet<LineStyle> lineStyleSet = new StyleSet<LineStyle> ();
			lineStyleSet.SetZoomStyle (0, lineStyle);

			Line line = new Line (new List<MapPos> (points), new DefaultLabel (lineName), lineStyleSet, null);
			_geoLayer.Add (line);
		}

		private Bitmap CreateBackdropImage ()
		{
			// Create starry image with (approximately) half the resolution of the view
			Android.Util.DisplayMetrics displaymetrics = new Android.Util.DisplayMetrics ();
			WindowManager.DefaultDisplay.GetMetrics (displaymetrics);
			int height = displaymetrics.HeightPixels / 2;
			int width = displaymetrics.WidthPixels / 2;
			Bitmap backgroundBitmap = Bitmap.CreateBitmap (width, height, Bitmap.Config.Argb8888);
			Canvas canvas = new Canvas (backgroundBitmap);
			canvas.DrawRGB (0, 0, 0);
			Paint paint = new Paint ();
			paint.Color = Android.Graphics.Color.White;
			Random rand = new Random ();
			for (int i = 0; i < 200; i++) {
				int x = (int)(rand.NextDouble () * canvas.Width);
				int y = (int)(rand.NextDouble () * canvas.Height);
				canvas.DrawCircle (x, y, (float)rand.NextDouble () * 1.25f, paint);
			}
			return backgroundBitmap;
		}
	}
}
