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
	[Activity (Label = "OfflineMapActivity")]
	public class OfflineMapActivity : Activity
	{
		/// <summary>
		/// The Nutiteq MapView
		/// </summary>
		private MapView _mapView;

		protected override void OnCreate ( Bundle bundle )
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

			/// Define base projection, almost always EPSG3857, but others can be defined also
			EPSG3857 proj = new EPSG3857 ();

			/// Use packaged data source for tiles. Packaged tiles as stored as individual bitmaps under 'raw' resources. Only tiles up to zoom level 2 are included.
			IRasterDataSource dataSource = new PackagedRasterDataSource (proj, 0, 2, "t{zoom}_{x}_{y}", ApplicationContext);
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

			/// zoom - 0 = world, like on most web maps
			_mapView.Zoom = 2;

			/// constrain zoom range as we have limited set of tiles
			_mapView.Constraints.ZoomRange = new Range (0, 3);

			// get the zoomcontrols defined in main.xml, set zoom listeners
			ZoomControls zoomControls = FindViewById<ZoomControls> ( Resource.Id.zoomcontrols );
			zoomControls.ZoomInClick += ( sender, e ) => { _mapView.ZoomIn (); };
			zoomControls.ZoomOutClick += ( sender, e ) => { _mapView.ZoomOut (); };
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
	}
}




