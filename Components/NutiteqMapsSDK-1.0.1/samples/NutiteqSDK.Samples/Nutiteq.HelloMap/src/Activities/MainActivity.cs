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

namespace Nutiteq.HelloMap
{
	/// <summary>
	/// Basic sample showing how to set up layers in Nutiteq SDK, how to handle different screen resolutions/DPI.
	/// The sample uses MapQuest raster layers as a base layer, displays single marker at SF and sets focus point at SF.
	/// </summary>
	[Activity (Label = "Nutiteq.HelloMap", MainLauncher = true)]
	public class MainActivity : Activity
	{
		/// <summary>
		/// The Nutiteq MapView
		/// </summary>
		private MapView _mapView;

		/// <summary>
		/// The marker layer.
		/// </summary>
		private MarkerLayer _markerLayer;

		protected override void OnCreate ( Bundle bundle )
		{
			base.OnCreate ( bundle );

			/// Set our view from the "main" layout resource
			SetContentView ( Resource.Layout.Main );

			/// Register SDK license (optional for evaluation license)
			//MapView.RegisterLicense ("<ENTER YOUR LICENSEKEY HERE>", ApplicationContext);

			/// Set watermark bitmap - only available for commercial licenses, replaces Nutiteq Evaluation watermark. -1, -1 are relative screen coordinates, 0.2 is watermark size relative to screen
			//MapView.SetWatermark (UnscaledBitmapLoader.DecodeResource (Resources, Resource.Drawable.Icon), -1.0f, -1.0f, 0.2f);

			// enable Nutiteq SDK logging
			Log.EnableAll ();

			/// Get our map from the layout resource. 
			_mapView = FindViewById<MapView> ( Resource.Id.mapView );

			/// Components keeps internal state and parameters for MapView.
			/// This initialization must be done before setting any options or layers!
			_mapView.Components = new Components ();

			// adjust map tile selection based on device DPI
			AdjustMapDpi ();

			/// Define base projection, almost always EPSG3857, but others can be defined also
			EPSG3857 proj = new EPSG3857 ();

			/// define the map layer MapQuest Open Tiles
			IRasterDataSource dataSource = new HTTPRasterDataSource (proj, 0, 18, "http://otile1.mqcdn.com/tiles/1.0.0/osm/{zoom}/{x}/{y}.png");
			RasterLayer mapLayer = new RasterLayer (dataSource, 0);

			/// Set online base layer  
			_mapView.Layers.BaseLayer = mapLayer;

			/// Set options
			_mapView.Options.KineticPanning = true;
			_mapView.Options.DoubleClickZoomIn = true;
			_mapView.Options.DualClickZoomOut = true;
			_mapView.Options.RasterTaskPoolSize = 4; // number of downloaders
			 
			/// create layer and add object to the layer, finally add layer to the map. 
			/// All overlay layers must be same projection as base layer, so we reuse it
			_markerLayer = new MarkerLayer ( _mapView.Layers.BaseLayer.Projection );
			_mapView.Layers.AddLayer ( _markerLayer );

			/// Add marker
			AddMarker ("Marker", "San Francisco", -122.41666666667, 37.76666666666);

			/// center view at SF, set zoom
			_mapView.FocusPoint = _mapView.Layers.BaseLayer.Projection.FromWgs84 ( -122.41666666667, 37.76666666666 );
			_mapView.Zoom = 10f;
		}

		protected override void OnStart ()
		{
			_mapView.StartMapping ();
			base.OnStart ();
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			_mapView.StopMapping ();
		}

		/// <summary>
		/// It Adds a Marker in the map for the current location
		/// </summary>
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

		/// <summary>
		/// adjust zooming to DPI, so texts on rasters will be not too small
		/// useful for non-retina rasters, they would look like "digitally zoomed"
		/// </summary>
		private void AdjustMapDpi()
		{
			Android.Util.DisplayMetrics displaymetrics = new Android.Util.DisplayMetrics ();
			WindowManager.DefaultDisplay.GetMetrics ( displaymetrics );
			// following is equal to  -log2(dpi / DEFAULT_DPI)
			float adjustment = (float) -( Math.Log ( displaymetrics.Density )  / Math.Log ( 2 ) );
			_mapView.Options.TileZoomLevelBias = adjustment / 2.0f;
		}
	}
}




