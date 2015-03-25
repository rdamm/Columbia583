
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;
using Android.Graphics;
using Nutiteq.SDK;

using Xamarin.Forms.Platform.Android;

namespace Columbia583.Android
{
	[Activity (Label = "TestMapActivity")]			
	public class TestMapActivity : FormsApplicationActivity
	{
		/// <summary>
		/// The Nutiteq MapView
		/// </summary>
		private MapView _mapView;
		private MarkerLayer _markerLayer;
		private GeometryLayer _geometryLayer;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
				System.Diagnostics.Debug.WriteLine (e);
			};
			AndroidEnvironment.UnhandledExceptionRaiser += (sender, e) => 
			{
				System.Diagnostics.Debug.WriteLine (e);
			};

			Xamarin.Forms.Forms.Init (this, bundle);

			SetPage (App.GetMapsPage ());

			/// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Map);

			// enable Nutiteq SDK logging
			Log.EnableAll ();

			/// Get our map from the layout resource. 
			_mapView = FindViewById<MapView> (Resource.Id.mapView);

			/// Components keeps internal state and parameters for MapView
			_mapView.Components = new Components ();

			/// Define base projection, almost always EPSG3857, but others can be defined also
			EPSG3857 proj = new EPSG3857 ();

			/// Use packaged data source for tiles. Packaged tiles as stored as individual bitmaps under 'raw' resources. Only tiles up to zoom level 2 are included.
			IRasterDataSource dataSource = new PackagedRasterDataSource (proj, 11, 15, "t{zoom}_{x}_{y}", ApplicationContext);
			// Alternative is to use offlines tiles in MBTiles format. MBTiles data source is included as sample and it uses file-based tile storage.
			//IRasterDataSource dataSource = new MBTilesRasterDataSource (proj, 0, 5, "/sdcard/mapxt/osm.mbtiles");
			RasterLayer mapLayer = new RasterLayer (dataSource, 0);

			/// Set online base layer  
			_mapView.Layers.BaseLayer = mapLayer;

			/// Set options
			_mapView.Options.KineticPanning = true;
			_mapView.Options.DoubleClickZoomIn = true;
			_mapView.Options.DualClickZoomOut = true;
			_mapView.Options.BackgroundPlaneDrawMode = Options.DrawBitmap;
			_mapView.Options.SetBackgroundPlaneBitmap (UnscaledBitmapLoader.DecodeResource (Resources, Resource.Drawable.background_plane));
			_mapView.Constraints.Rotatable = false;

			/// zoom - 0 = world, like on most web maps
			_mapView.Zoom = 11;

			/// constrain zoom range as we have limited set of tiles
			_mapView.Constraints.ZoomRange = new Range (11, 15);

			double Cal_lat = 51.0486f, Cal_long = -114.0708f, e_lat = 50.70f, e_long = -116.132f;
			MapPos focusPoint = mapLayer.Projection.FromWgs84( e_long, e_lat);
			_mapView.FocusPoint = focusPoint;

			// get the zoomcontrols defined in main.xml, set zoom listeners
			ZoomControls zoomControls = FindViewById<ZoomControls> ( Resource.Id.zoomcontrols );
			zoomControls.ZoomInClick += ( sender, e ) => { _mapView.ZoomIn (); };
			zoomControls.ZoomOutClick += ( sender, e ) => { _mapView.ZoomOut (); };

			_markerLayer = new MarkerLayer ( _mapView.Layers.BaseLayer.Projection );
			_mapView.Layers.AddLayer ( _markerLayer );

			_geometryLayer = new GeometryLayer(_mapView.Layers.BaseLayer.Projection);
			_mapView.Layers.AddLayer(_geometryLayer);

			int minZoom = 11;

			Bitmap pointMarker = UnscaledBitmapLoader.DecodeResource (Resources, Resource.Drawable.point);
			Bitmap lineMarker = UnscaledBitmapLoader.DecodeResource (Resources, Resource.Drawable.line);

			PointStyle pointStyle = new PointStyle.Builder ().Build ();
			StyleSet<PointStyle> pointStyleSet = new StyleSet<PointStyle>();

			pointStyle.PickingSize = 1.0f;
			pointStyle.Color = new Nutiteq.SDK.Color (1, 0, 0);

			LineStyle lineStyle = new LineStyle.Builder ().Build ();
			StyleSet<LineStyle> lineStyleSet = new StyleSet<LineStyle> ();

			lineStyle.Color = new Nutiteq.SDK.Color (1, 0, 0);
			lineStyle.PickingWidth = 100.0f;

			IList<MapPos> mapPos = new List<MapPos> ();
			mapPos.Add (proj.FromWgs84((float)e_long, (float)e_lat));
			mapPos.Add (proj.FromWgs84((float)Cal_long, (float)Cal_lat));
			//mapPos.Add (new MapPos (Cal_lat, Cal_long));

			_geometryLayer.Add(new Line(mapPos, new DefaultLabel("Line"), lineStyle, null));

			/// Add marker
			AddMarker ("Marker", "Calgary", Cal_long, Cal_lat);
			AddMarker ("Edgewater", "Default Location", e_long, e_lat);
		
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			/// Start map
			_mapView.StartMapping ();
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			/// Stop map
			_mapView.StopMapping ();
		}
		void AddMarker ( string title, string subtitle, double longitude, double latitude )
		{
			/// Define label what is shown when you click on marker.
			Label markerLabel = new DefaultLabel ( title, subtitle );

			/// Define the location of the marker, it must be converted to EPSG3857 coordinate system
			MapPos mapLocation = _markerLayer.Projection.FromWgs84 ( longitude, latitude );

			///load default market style
			MarkerStyle markerStyle = DefaultMarkerStyle ().Build ();

			/// add the label to the Marker
			Marker marker = new Marker ( mapLocation, markerLabel, markerStyle, null );

			/// add the label to the layer
			_markerLayer.Add ( marker );
		}

		/// <summary>
		/// Defines default marker style (image, size, color)
		/// </summary>
		/// <returns>The marker style builder.</returns>
		private MarkerStyle.Builder DefaultMarkerStyle ()
		{
			// define marker style 
			Bitmap pointMarker = UnscaledBitmapLoader.DecodeResource ( Resources, Resource.Drawable.olmarker );
			MarkerStyle.Builder markerStyleBuilder = new MarkerStyle.Builder ();
			markerStyleBuilder.SetBitmap ( pointMarker );
			markerStyleBuilder.SetColor ( Nutiteq.SDK.Color.White );
			markerStyleBuilder.SetSize ( 0.5f );
			return markerStyleBuilder;
		}
	}
}

