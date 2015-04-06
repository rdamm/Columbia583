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
using Nutiteq.SDK;

namespace Nutiteq.AdvancedMap3D
{
	/// <summary>
	/// Demonstrates NMLModelOnlineLayer - online 3D model layer which loads data from Nutiteq NML online API.
	/// The demo server has data of few cities: Tallinn, Barcelona, San Francisco. This content is from Google 3D Warehouse
	/// 
	/// Loaded data is partly cached locally with special cache.
	/// 
	/// @author jaak
	/// @translated by m@t
	/// </summary>
	[Activity (Label = "Online3DMapActivity")]			
	public class Online3DMapActivity : Activity
	{
		private const String DATASET = "http://aws-lb.nutiteq.ee/nml/nmlserver2.php?data=demo"; // default dataset
		private MapView _mapView;
		private ModelStyle _modelStyle;
		private NMLModelOnlineLayer _modelLayer;

		protected override void OnCreate ( Bundle bundle )
		{
			base.OnCreate ( bundle );

			SetContentView ( Resource.Layout.Main );

			Log.EnableAll ();
			Log.SetTag ( "Online3DMapActivity" );

			// 1. Get the MapView from the Layout xml - mandatory
			_mapView = FindViewById<MapView> ( Resource.Id.mapView );

			// Optional, but very useful: restore map state during device rotation,
			// it is saved in onRetainNonConfigurationInstance() below
			Components retainObject =  LastNonConfigurationInstance as Components;

			if ( retainObject != null ) 
			{
				// just restore configuration, skip other initializations
				_mapView.Components = retainObject;
				return;
			} 
			else 
			{
				// 2. create and set MapView components - mandatory
				Components components = new Components ();
				_mapView.Components = components;
			}

			EPSG3857 proj = new EPSG3857 ();
			IRasterDataSource dataSource = new HTTPRasterDataSource (proj, 0, 18, "http://otile1.mqcdn.com/tiles/1.0.0/osm/{zoom}/{x}/{y}.png");
			RasterLayer mapLayer = new RasterLayer (dataSource, 0);
			_mapView.Layers.BaseLayer = mapLayer;

			// define style for 3D to define minimum zoom = 14
			ModelStyle.Builder modelStyleBuilder = new ModelStyle.Builder ();
			_modelStyle = modelStyleBuilder.Build ();

			// San Francisco
			_mapView.FocusPoint = new MapPos ( -1.3625947E7f, 4550716.0f );
			_mapView.Zoom = 17.0f;
			_mapView.Tilt = 35.0f;

			// set initial layer
			Online3DLayer ( DATASET );

			// Activate some mapview options to make it smoother - optional
			_mapView.Options.Preloading = false;
			_mapView.Options.SeamlessHorizontalPan = true;
			_mapView.Options.TileFading = false;
			_mapView.Options.DoubleClickZoomIn = true;
			_mapView.Options.DualClickZoomOut = true;

			// set sky bitmap - optional, default - white
			_mapView.Options.SkyDrawMode = Options.DrawBitmap;
			_mapView.Options.SkyOffset = 4.86f;
			_mapView.Options.SetSkyBitmap ( UnscaledBitmapLoader.DecodeResource ( Resources, Resource.Drawable.sky_small ) );

			// Map background, visible if no map tiles loaded - optional, default - white
			_mapView.Options.BackgroundPlaneDrawMode = Options.DrawBitmap;
			_mapView.Options.SetBackgroundPlaneBitmap ( UnscaledBitmapLoader.DecodeResource ( Resources, Resource.Drawable.background_plane ) );
		
			// configure texture caching - optional, suggested
			_mapView.Options.SetTextureMemoryCacheSize ( 20 * 1024 * 1024 );
			_mapView.Options.SetCompressedMemoryCacheSize ( 8 * 1024 * 1024 );

			// define online map persistent caching - optional, suggested. Default - no caching
			_mapView.Options.SetPersistentCachePath ( this.GetDatabasePath ( "mapcache" ).Path );

			// set persistent raster cache limit to 100MB
			_mapView.Options.SetPersistentCacheSize ( 100 * 1024 * 1024 );

			// 4. zoom buttons using Android widgets - optional
			// get the zoomcontrols defined in main.xml, set zoom listeners
			ZoomControls zoomControls = FindViewById<ZoomControls> ( Resource.Id.zoomcontrols );
			zoomControls.ZoomInClick += (sender, e) => { _mapView.ZoomIn (); };
			zoomControls.ZoomOutClick += (sender, e) => { _mapView.ZoomOut (); };
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			_mapView.StartMapping ();
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			Log.Debug ( "x " + GetMapView ().FocusPoint.X );
			Log.Debug ( "y " + GetMapView ().FocusPoint.Y );
			Log.Debug ( "tilt " + GetMapView ().Tilt);
			Log.Debug ( "rotation " + GetMapView ().Rotation );
			Log.Debug ( "zoom " + GetMapView ().Zoom );
			_mapView.StopMapping ();
		}

		public override bool OnCreateOptionsMenu ( IMenu menu )
		{
			MenuInflater inflater = new MenuInflater ( this );
			inflater.Inflate ( Resource.Menu.online3d, menu );
			return true;
		}

		public override bool OnMenuItemSelected ( int featureId, IMenuItem item )
		{ 
			item.SetChecked ( true );
			switch ( item.ItemId ) 
			{
				// map types
				case Resource.Id.menu3d_demo:
					Online3DLayer ( "http://aws-lb.nutiteq.ee/nml/nmlserver2.php?data=demo" );
					// San Francisco
					GetMapView ().SetFocusPoint ( new MapPos ( -1.3625947E7f, 4550716.0f ), 1000 );
					break;

				case Resource.Id.menu3d_hover:
					Online3DLayer ( "http://aws-lb.nutiteq.ee/nml/nmlserver2.php?data=hover" );
					// San Francisco
					GetMapView ().SetFocusPoint ( new MapPos ( -1.3625947E7f, 4550716.0f ), 1000 );
					break;

				case Resource.Id.menu3d_tomtomlod3:
					Online3DLayer ( "http://aws-lb.nutiteq.ee/nml/nmlserver3.php?data=tomtom" );
					// San Francisco
					GetMapView ().SetFocusPoint ( new MapPos ( -1.3625947E7f, 4550716.0f ), 1000 );
					break;

				case Resource.Id.menu3d_blom:
					Online3DLayer ( "http://aws-lb.nutiteq.ee/nml/nmlserver3.php?data=blom" );
					// London
					_mapView.SetFocusPoint ( _mapView.Layers.BaseProjection.FromWgs84 ( -0.109015f, 51.516584f ), 1000 );
					break;

				case Resource.Id.menu3d_seattle:
					Online3DLayer("http://aws-lb.nutiteq.ee/nml/nmlserver3.php?data=seattle");
					_mapView.SetFocusPoint ( _mapView.Layers.BaseProjection.FromWgs84 ( -122.3336f, 47.6014f ), 1000 );
					break;

				case Resource.Id.menu3d_la:
					Online3DLayer ( "http://aws-lb.nutiteq.ee/nml/nmlserver3.php?data=los_angeles");
					_mapView.SetFocusPoint ( _mapView.Layers.BaseProjection.FromWgs84 ( -118.24270, 34.05368 ), 1000 );
					break;

				case Resource.Id.menu3d_chicago:
					Online3DLayer("http://aws-lb.nutiteq.ee/nml/nmlserver3.php?data=chicago");
					_mapView.SetFocusPoint ( _mapView.Layers.BaseProjection.FromWgs84 ( -87.6219, 41.8769 ), 1000 );
					break;
			}

			return false;
		}

		// Switch online 3D Model layer with given URL
		private void Online3DLayer ( String dataset ) 
		{
			if (_modelLayer != null) 
			{
				_mapView.Layers.RemoveLayer (_modelLayer);
			}

			_modelStyle = new ModelStyle.Builder ().Build ();
		
			StyleSet<ModelStyle> modelStyleSet = new StyleSet<ModelStyle> ();
			modelStyleSet.SetZoomStyle ( 14, _modelStyle );
			_modelLayer = new NMLModelOnlineLayer ( new EPSG3857 (), dataset, modelStyleSet );
			_modelLayer.SetMemoryLimit ( 40 * 1024 * 1024 );
			_modelLayer.SetPersistentCacheSize ( 60 * 1024 * 1024 );
			_modelLayer.SetPersistentCachePath ( this.GetDatabasePath ( "nmlcache_" + dataset.Substring ( dataset.LastIndexOf ( "=" ) ) ).Path );
			_modelLayer.SetLODResolutionFactor ( 0.3f );
			_mapView.Layers.AddLayer ( _modelLayer );
		}

		public MapView GetMapView () 
		{
			return _mapView;
		}
	}
}

