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
using Android.Util;
using Android.Graphics;
using Nutiteq.SDK;

namespace Nutiteq.AdvancedMap3D
{
	/// <summary>
	/// This sample has a set of different layers: 
	/// a) raster online: TMS, WMS, Bing, MapBox
	/// b) raster offline: StoredMap (MGM format), PackagedMap from app package (res/raw)
	/// c) 3D layer: OSMPolygon3D with roof structures and color tags
	/// d) Others: MarkerLayer
	/// 
	/// See private methods in this class which layers can be added.
	/// 
	/// OptionsMenu items are made to select base map layers and overlays, and to jump to more interesting places
	/// 
	/// @author jaak
	/// @translated by m@t
	/// </summary>
	[Activity (Label = "AdvancedMapActivity")]			
	public class AdvancedMapActivity : Activity
	{
		private MapView _mapView;
		private Projection _projection;

		protected override void OnCreate ( Bundle bundle )
		{
			base.OnCreate ( bundle );

			// spinner in status bar, for progress indication
			RequestWindowFeature ( WindowFeatures.IndeterminateProgress );

			SetContentView ( Resource.Layout.Main );

			// enable logging for troubleshooting - optional
			Nutiteq.SDK.Log.EnableAll ();
			Nutiteq.SDK.Log.SetTag ( "advancedmap" );

			// 1. Get the MapView from the Layout xml - mandatory
			_mapView = FindViewById<MapView> ( Resource.Id.mapView );
			_projection = new EPSG3857 ();

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

			// add event listener
			MyMapListener mapListener = new MyMapListener ();
			_mapView.Options.MapListener = mapListener;

			// 3. Define map layer for basemap - mandatory.
			// Here we use MapQuest open tiles
			// Almost all online tiled maps use EPSG3857 projection.
			BaseMapQuest ();


			// set initial map view camera - optional. "World view" is default
			// Location: San Francisco
			_mapView.FocusPoint = _mapView.Layers.BaseLayer.Projection.FromWgs84 ( -122.41666666667f, 37.76666666666f );

			// rotation - 0 = north-up
			_mapView.Rotation = 0f;

			// zoom - 0 = world, like on most web maps
			_mapView.Zoom = 2f;

			// tilt means perspective view. Default is 90 degrees for "normal" 2D map view, minimum allowed is 30 degrees.
			_mapView.Tilt = 55.0f;

			// Activate some mapview options to make it smoother - optional
			_mapView.Options.Preloading = false;
			_mapView.Options.SeamlessHorizontalPan = true;
			_mapView.Options.TileFading = false;
			_mapView.Options.KineticPanning = true;
			_mapView.Options.DoubleClickZoomIn = true;
			_mapView.Options.DualClickZoomOut = true;

			AdjustMapDpi();
		
			_mapView.Options.RasterTaskPoolSize = 4;

			// set sky bitmap - optional, default - white
			_mapView.Options.SkyDrawMode = Options.DrawBitmap;
			_mapView.Options.SkyOffset = 4.86f;
			_mapView.Options.SetSkyBitmap( UnscaledBitmapLoader.DecodeResource ( Resources,Resource.Drawable.sky_small ) );

			// Map background, visible if no map tiles loaded - optional, default - white
			_mapView.Options.BackgroundPlaneDrawMode = Options.DrawBitmap;
			_mapView.Options.SetBackgroundPlaneBitmap ( UnscaledBitmapLoader.DecodeResource ( Resources,Resource.Drawable.background_plane ) );

			// configure texture caching - optional, suggested
			_mapView.Options.SetTextureMemoryCacheSize ( 20 * 1024 * 1024 );
			_mapView.Options.SetCompressedMemoryCacheSize ( 8 * 1024 * 1024 );

			// set persistent raster cache limit to 100MB and set cache path
			_mapView.Options.SetPersistentCacheSize ( 100 * 1024 * 1024 );
			_mapView.Options.SetPersistentCachePath ( this.GetDatabasePath ( "mapcache" ).Path );

			// get the zoomcontrols defined in main.xml, set zoom listeners
			ZoomControls zoomControls = FindViewById<ZoomControls> ( Resource.Id.zoomcontrols );
			zoomControls.ZoomInClick += ( sender, e ) => { _mapView.ZoomIn (); };
			zoomControls.ZoomOutClick += ( sender, e ) => { _mapView.ZoomOut (); };
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

		public override bool OnCreateOptionsMenu ( IMenu menu )
		{
			MenuInflater inflater = new MenuInflater ( this );
			inflater.Inflate ( Resource.Menu.mainmenu, menu );
			return true;
		}

		public override bool OnMenuItemSelected ( int featureId, IMenuItem item )
		{
			item.SetChecked ( true );

			switch ( item.ItemId ) {

			// map types
			case Resource.Id.menu_openstreetmap:
				BaseMapQuest ();
				break;

			case Resource.Id.menu_mapboxsatellite:
				BaseLayerMapBoxSatelliteLayer ( false );
				break;

			case Resource.Id.menu_mapboxsatelliteretina:
				BaseLayerMapBoxSatelliteLayer ( true );
				break;  

			case Resource.Id.menu_mapbox:
				BaseLayerMapBoxStreetsLayer ( false );
				break;

			case Resource.Id.menu_mapboxretina:
				BaseLayerMapBoxStreetsLayer ( true );
				break;

			case Resource.Id.menu_stamenterrain:
				BaseLayerStamenTerrainLayer ();
				break;

			case Resource.Id.menu_bing:
				BaseBingLayer ("http://ecn.t3.tiles.virtualearth.net/tiles/r{quadkey}.png?g=1&mkt=en-US&shading=hill&n=z");
				break;

			case Resource.Id.menu_bingaerial:
				BaseBingAerial();
				break;

			case Resource.Id.menu_openaerial:
				BaseMapOpenAerial ();
				break;

			case Resource.Id.menu_stored:
				BasePackagedLayer ();
				break;

/// ######## overlays
			case Resource.Id.menu_nml:
				SingleNmlModelLayer ();
				break;

			case Resource.Id.menu_hillshade:
				IRasterDataSource dataSource = new HTTPRasterDataSource(new EPSG3857 (), 5, 18, "http://toolserver.org/~cmarqu/hill/{zoom}/{x}/{y}.png");
				RasterLayer hillsLayer = new RasterLayer(dataSource, 0);
				_mapView.Layers.AddLayer ( hillsLayer );
				break;

			case Resource.Id.menu_marker:
				AddMarkerLayer ( _projection.FromWgs84 ( -122.416667f, 37.766667f ) );
				break;

			case Resource.Id.menu_tileborders:
				AddTileBorderLayer ( 256 );
				break;

				// Locations
			case Resource.Id.menu_coburg:
				// Coburg, germany
				_mapView.SetFocusPoint ( _projection.FromWgs84( 10.96465, 50.27082 ), 1000 );
				_mapView.Zoom = 16.0f;
				break;

			case Resource.Id.menu_petronas:
				// Petronas towers, Kuala Lumpur, Malaisia
				_mapView.SetFocusPoint ( _projection.FromWgs84 ( 101.71339, 3.15622 ), 1000 );
				_mapView.Zoom = 17.0f;
				_mapView.Tilt = 60;
				break;

			case Resource.Id.menu_sf:
				// San Francisco
				_mapView.SetFocusPoint ( _projection.FromWgs84 ( -122.416667f, 37.766667f ), 1000 );
				break;

			case Resource.Id.menu_barcelona:
				// San Francisco
				_mapView.SetFocusPoint ( _projection.FromWgs84 ( 2.183333f, 41.383333f ), 1000 );
				break;

			case Resource.Id.menu_tll:
				// Tallinn
				_mapView.SetFocusPoint ( new MapPos ( 2753791.3f, 8275296.0f ), 1000 ); 
				break;    
			}

			return true;
		}

	
		// adjust zooming to DPI, so texts on rasters will be not too small
		// useful for non-retina rasters, they would look like "digitally zoomed"
		private void AdjustMapDpi () 
		{
			DisplayMetrics metrics = new DisplayMetrics();
			WindowManager.DefaultDisplay.GetMetrics( metrics );
			float dpi = (float)metrics.DensityDpi;
			// following is equal to  -log2(dpi / DEFAULT_DPI)
			float adjustment = (float) - ( Math.Log ( dpi / metrics.HeightPixels ) / Math.Log ( 2 ) );
			Nutiteq.SDK.Log.Debug ( "adjust DPI = " + dpi + " as zoom adjustment = " + adjustment );
			_mapView.Options.TileZoomLevelBias = adjustment / 4.0f;
		}

		private void AddTileBorderLayer ( int size ) 
		{
			IRasterDataSource dataSource = new TileDebugRasterDataSource (this._projection, 0, 22, size);
			RasterLayer tileDebugLayer = new RasterLayer (dataSource, 17);
			tileDebugLayer.MemoryCaching = false;
			_mapView.Layers.AddLayer ( tileDebugLayer );
		}

		private void BasePackagedLayer () 
		{
			IRasterDataSource dataSource = new PackagedRasterDataSource(new EPSG3857(), 0, 3, "t{zoom}_{x}_{y}", ApplicationContext);
			RasterLayer packagedMapLayer = new RasterLayer (dataSource, 16);
			_mapView.Layers.BaseLayer = packagedMapLayer;
		}

		/// <summary>
		/// Define marker style (image, size, offset)
		/// </summary>
		/// <returns>The marker style.</returns>
		private MarkerStyle.Builder DefineMarkerStyle ( int resourceId, float size, float offset2D )
		{
			// define marker style 
			Bitmap pointMarker = UnscaledBitmapLoader.DecodeResource ( Resources, resourceId );
			MarkerStyle.Builder markerStyleBuilder = new MarkerStyle.Builder ();
			markerStyleBuilder.SetBitmap ( pointMarker );
			markerStyleBuilder.SetSize ( size );
			markerStyleBuilder.SetOffset2DX ( offset2D );

			return markerStyleBuilder;
		}

		/// <summary>
		/// Define label style (image, size, color)
		/// </summary>
		/// <returns>The marker style.</returns>
		private LabelStyle.Builder DefineLabelStyle ( int edgePadding, int linePadding, int sizeTitle, int sizeDescription )
		{
			LabelStyle.Builder builder = new LabelStyle.Builder ();
			builder.SetEdgePadding ( edgePadding );
			builder.SetLinePadding ( linePadding );

			return builder;
		}

		// ** Add simple marker to map.
		private void AddMarkerLayer ( MapPos markerLocation ) 
		{
			// define marker style (image, size, color)
			MarkerStyle markerStyle = DefineMarkerStyle ( Resource.Drawable.olmarker, 0.5f, 0f ).Build ();
			
            // define label what is shown when you click on marker
			LabelStyle labelStyle = DefineLabelStyle ( 24, 12, 38, 32 ).Build ();
			Label markerLabel = new DefaultLabel ( "San Francisco", "Here is a marker", labelStyle );

			// create layer and add object to the layer, finally add layer to the map. 
			// All overlay layers must be same projection as base layer, so we reuse it
			MarkerLayer markerLayer = new MarkerLayer ( _projection );
			Marker marker = new Marker ( markerLocation, markerLabel, markerStyle, null );
			markerLayer.Add ( marker );
			_mapView.Layers.AddLayer ( markerLayer );
			_mapView.SelectVectorElement ( marker );
		}

		private void BaseBingLayer ( String urlTemplate )
		{
			IRasterDataSource dataSource = new HTTPRasterDataSource(new EPSG3857(), 0, 19, urlTemplate);
			RasterLayer bingMap = new RasterLayer (dataSource, 1013);
			_mapView.Layers.BaseLayer = bingMap;
		}

		private void BaseLayerMapBoxSatelliteLayer ( Boolean retina )
		{
			String mapId;
			int cacheID;
			if ( retina )
			{
				mapId = "nutiteq.map-78tlnlmb";
				cacheID = 24;
			}
			else
			{
				mapId = "nutiteq.map-f0sfyluv";
				cacheID = 25;
			}
			IRasterDataSource dataSource = new HTTPRasterDataSource(new EPSG3857(), 0, 19, "http://api.tiles.mapbox.com/v3/" + mapId + "/{zoom}/{x}/{y}.png");
			_mapView.Layers.BaseLayer = new RasterLayer(dataSource, cacheID);
		}

		private void BaseLayerMapBoxStreetsLayer ( Boolean retina )
		{
			String mapId;
			int cacheID;
			if ( retina )
			{
				mapId = "nutiteq.map-aasha5ru";
				cacheID = 22;
			}
			else
			{
				mapId = "nutiteq.map-j6a1wkx0";
				cacheID = 23;
			}
			IRasterDataSource dataSource = new HTTPRasterDataSource(new EPSG3857(), 0, 19, "http://api.tiles.mapbox.com/v3/" + mapId + "/{zoom}/{x}/{y}.png");
			_mapView.Layers.BaseLayer = new RasterLayer(dataSource, cacheID);
		}

		private void BaseMapQuest () 
		{
			IRasterDataSource dataSource = new HTTPRasterDataSource(new EPSG3857(), 0, 20, "http://otile1.mqcdn.com/tiles/1.0.0/osm/{zoom}/{x}/{y}.png");
			_mapView.Layers.BaseLayer = new RasterLayer(dataSource, 11);
		}

		private void BaseLayerStamenTerrainLayer () 
		{
			IRasterDataSource dataSource = new HTTPRasterDataSource(new EPSG3857(), 0, 20, "http://tile.stamen.com/terrain/{zoom}/{x}/{y}.png");
			_mapView.Layers.BaseLayer = new RasterLayer(dataSource, 18);
		}

		private void BaseBingAerial () 
		{
			IRasterDataSource dataSource = new HTTPRasterDataSource(new EPSG3857(), 0, 19, "http://ecn.t3.tiles.virtualearth.net/tiles/a{quadkey}.jpeg?g=471&mkt=en-US");
			_mapView.Layers.BaseLayer = new RasterLayer(dataSource, 14);
		}

		private void BaseMapOpenAerial () 
		{
			IRasterDataSource dataSource = new HTTPRasterDataSource(new EPSG3857(), 0, 11, "http://otile1.mqcdn.com/tiles/1.0.0/sat/{zoom}/{x}/{y}.png");
			_mapView.Layers.BaseLayer = new RasterLayer(dataSource, 15);
		}

		private void SingleNmlModelLayer () 
		{
			ModelStyle.Builder modelStyleBuilder = new ModelStyle.Builder ();
			ModelStyle modelStyle = modelStyleBuilder.Build ();

			StyleSet<ModelStyle> modelStyleSet = new StyleSet<ModelStyle> ( );
			modelStyleSet.SetZoomStyle ( 14, modelStyle );
 
			// create layer and an model
			MapPos mapPos1 = _projection.FromWgs84 ( 20.466027f, 44.810537f );
 
 			// set it to fly abit
			MapPos mapPos = new MapPos ( mapPos1.X, mapPos1.Y, 0.1f );
			NMLModelLayer nmlModelLayer = new NMLModelLayer ( new EPSG3857 () );

			Stream inputStream = this.Resources.OpenRawResource ( Resource.Raw.milktruck );
		
			// set initial position for the milk truck
			NMLModel model = new NMLModel ( mapPos, null, modelStyleSet, inputStream, null );

			// set size
			model.Scale = new Vector3D ( 10, 10, 10 );

			nmlModelLayer.Add ( model );

			_mapView.Layers.AddLayer ( nmlModelLayer );
			_mapView.SetFocusPoint ( mapPos, 10 );
			_mapView.Zoom = 19.0f;
			_mapView.Tilt = 45;
		}
	}
}

