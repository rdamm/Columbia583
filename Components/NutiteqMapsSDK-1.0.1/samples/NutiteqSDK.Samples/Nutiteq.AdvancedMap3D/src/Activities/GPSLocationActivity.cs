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
	[Activity (Label = "GPSLocationActivity")]
	public class GPSLocationActivity : Activity, ILocationListener
	{
		/// <summary>
		/// private field with the current coordinates
		/// </summary>
		private Location _currentLocation;

		/// <summary>
		/// The "Unable to determine your location." message.
		/// </summary>
		private TextView _textViewMessage;

		/// <summary>
		/// The location manager.
		/// </summary>
		private LocationManager _locationManager;

		/// <summary>
		/// The location provider.
		/// </summary>
		private string _locationProvider;

		/// <summary>
		/// The Nutiteq MapView
		/// </summary>
		private MapView _mapView;

		/// <summary>
		/// The marker layer.
		/// </summary>
		private MarkerLayer _markerLayer;

		private bool _markerAdded;

		private Marker _currentPositionMarker;

		protected override void OnCreate ( Bundle bundle )
		{
			base.OnCreate ( bundle );

			/// Set our view from the "main" layout resource
			SetContentView ( Resource.Layout.MainGPS );

			// enable Nutiteq SDK logging
			Log.EnableAll ();

			///bind the textViewMessage
			_textViewMessage = FindViewById<TextView> ( Resource.Id.textViewMessage );

			/// Get our map from the layout resource. 
			_mapView = FindViewById<MapView> ( Resource.Id.mapView );

			/// Components keeps internal state and parameters for MapView
			_mapView.Components = new Components ();

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

			// Map background, visible if no map tiles loaded - optional, default - white
			_mapView.Options.BackgroundPlaneDrawMode = Options.DrawBitmap;
			_mapView.Options.SetBackgroundPlaneBitmap ( UnscaledBitmapLoader.DecodeResource ( Resources,Resource.Drawable.background_plane ) );

			// create layer and add object to the layer, finally add layer to the map. 
			// All overlay layers must be same projection as base layer, so we reuse it
			_markerLayer = new MarkerLayer ( _mapView.Layers.BaseLayer.Projection );

			// zoom - 0 = world, like on most web maps
			_mapView.Zoom = 3f;

			///inizialize the location manager to get the current position
			InitializeLocationManager ();
		}

		protected override void OnStart ()
		{
			_mapView.StartMapping ();
			base.OnStart ();
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			///remove the update of the position
			if ( ( _locationManager != null ) && ( !String.IsNullOrEmpty ( _locationProvider ) ) ) 
			{
				_locationManager.RemoveUpdates(this);
			}

		}

		protected override void OnResume ()
		{
			base.OnResume ();

			///request updated position
			if ( ( _locationManager != null ) && (!String.IsNullOrEmpty ( _locationProvider ) ) ) 
			{
				_locationManager.RequestLocationUpdates ( _locationProvider, 0, 0, this );
			}
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			_mapView.StopMapping ();
		}

		/// <summary>
		/// It Adds a Marker in the map for the current location
		/// </summary>
		/// <param name="location">Location.</param>
		void AddMarker ( string currentPositionTitle, string currentPositionSubtitle, float latitude, float longitude )
		{
			/// Define label what is shown when you click on marker.
			Label markerLabel = new DefaultLabel ( currentPositionTitle, currentPositionSubtitle );


			/// Define the location of the marker, it must be converted to base map coordinate system
			MapPos currentLocation = _mapView.Layers.BaseLayer.Projection.FromWgs84 ( longitude, latitude );

			///load default market style
			MarkerStyle markerStyle = DefaultMarkerStyle ().Build ();

			/// add the label to the Marker
			_currentPositionMarker = new Marker ( currentLocation, markerLabel, markerStyle, _markerLayer );

			/// add the label to the layer
			_markerLayer.Add ( _currentPositionMarker );

			///add layer to the map. 
			_mapView.Layers.AddLayer ( _markerLayer );

			//center the map in the current location
			_mapView.FocusPoint = currentLocation;

			//zoom in the map in the current location
			_mapView.Zoom = 19f;
		}

		void UpdateMarker ( string myPosition, string subtitle, float latitude, float longitude )
		{
			if (!_markerAdded) {
				AddMarker (myPosition, subtitle, latitude, longitude);
				_markerAdded = true;
			} else {
				_currentPositionMarker.MapPos = _mapView.Layers.BaseLayer.Projection.FromWgs84 (longitude, latitude);
			}
		}

		/// <summary>
		/// Initializes the location manager.
		/// </summary>
		void InitializeLocationManager ()
		{
			_locationManager = (LocationManager)GetSystemService ( LocationService );
			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Coarse
			};

			IList<string> acceptableLocationProviders = _locationManager.GetProviders ( criteriaForLocationService, true );

			if ( acceptableLocationProviders.Any () )
			{
				_locationProvider = acceptableLocationProviders.First ();
				_textViewMessage.Visibility = ViewStates.Gone;
			}
			else
			{
				_locationProvider = String.Empty;
				_textViewMessage.Visibility = ViewStates.Visible;
			}
		}

		/// <Docs>The new location, as a Location object.</Docs>
		/// <remarks>Called when the location has changed.</remarks>
		/// <summary>
		/// Raises the location changed event.
		/// </summary>
		/// <param name="location">Location.</param>
		public void OnLocationChanged ( Location location )
		{
			_currentLocation = location;
			if ( _currentLocation == null )
			{
				LocationNotFound ();
			}
			else
			{   
				LocationFound ( location );
			}
		}

		/// <summary>
		/// Add a marker in the map when a new location is found.
		/// </summary>
		/// <param name="location">Location.</param>
		void LocationFound ( Location location )
		{
			///the error message is hidden from the screen
			_textViewMessage.Visibility = ViewStates.Gone;

			string subtitle = String.Format ( "lat:{0} lon:{1}", location.Latitude, location.Longitude );
			UpdateMarker ( "My position", subtitle, (float)location.Latitude, (float)location.Longitude );
		}

		/// <summary>
		/// Locations not found behaviour.
		/// </summary>
		void LocationNotFound ()
		{
			///the error message appears o the screen
			_textViewMessage.Visibility = ViewStates.Visible;
		}

		/// <summary>
		/// Defaults value for the marker style.
		/// </summary>
		/// <returns>The marker style.</returns>
		private MarkerStyle.Builder DefaultMarkerStyle()
		{
			return DefineMarkerStyle ( Resource.Drawable.olmarker, 0.3f, 0f );
		}

		/// <summary>
		/// Define marker style (image, size, color)
		/// </summary>
		/// <returns>The marker style.</returns>
		private MarkerStyle.Builder DefineMarkerStyle ( int resourceId, float size, float offset2D )
		{
			// define marker style 
			Bitmap pointMarker = UnscaledBitmapLoader.DecodeResource ( Resources, resourceId );
			MarkerStyle.Builder markerStyleBuilder = new MarkerStyle.Builder ();
			markerStyleBuilder.SetBitmap ( pointMarker );
			markerStyleBuilder.SetColor ( Nutiteq.SDK.Color.Blue );
			markerStyleBuilder.SetSize ( size );
			markerStyleBuilder.SetOffset2DX ( offset2D );
			return markerStyleBuilder;
		}

		/// <Docs>the name of the location provider associated with this
		///  update.</Docs>
		/// <remarks>Called when the provider is disabled by the user. If requestLocationUpdates
		///  is called on an already disabled provider, this method is called
		///  immediately.</remarks>
		/// <format type="text/html">[Android Documentation]</format>
		/// <since version="Added in API level 1"></since>
		/// <summary>
		/// Raises the provider disabled event.
		/// </summary>
		/// <param name="provider">Provider.</param>
		public void OnProviderDisabled ( string provider )
		{

		}

		/// <Docs>the name of the location provider associated with this
		///  update.</Docs>
		/// <remarks>Called when the provider is enabled by the user.</remarks>
		/// <format type="text/html">[Android Documentation]</format>
		/// <since version="Added in API level 1"></since>
		/// <summary>
		/// Raises the provider enabled event.
		/// </summary>
		/// <param name="provider">Provider.</param>
		public void OnProviderEnabled ( string provider )
		{

		}

		/// <Docs>the name of the location provider associated with this
		///  update.</Docs>
		/// <summary>
		/// Raises the status changed event.
		/// </summary>
		/// <param name="provider">Provider.</param>
		/// <param name="status">Status.</param>
		/// <param name="extras">Extras.</param>
		public void OnStatusChanged ( string provider, Availability status, Bundle extras )
		{

		}
	}
}
