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
using Android.Provider;
using Android.Views.InputMethods;
using Org.Json;
using Nutiteq.SDK;

namespace Nutiteq.AdvancedMap3D
{
	/**
	 * Address search / Geocoding sample.
	 * 
	 * Sample uses Android searchable interface, which is linked to Activity via AndroidManifest file.
	 * 
	 * Classes:
	 * 1. geocode.MapQuestGeocoder.java Geocoder implementation, uses MapQuest Open API REST API Map
	 * 
	 * 2. mapquest.SearchQueryResults.java - ListView which initiates real search, and shows results as ListView
	 * 
	 * 3. mapquest.SearchRecentSuggestionsProvider.java - stores last search terms to memory
	 * 
	 * 4. AddressSearchActivity.java opens Android default search UI. Search result comes from resuming 
	 *      from search results activity, this is shown on map, and map is re-centered to found result.
	 * 
	 * 5. Resources: values/strings.xml, layout/search_query_results.xml and layout/searchrow.xml define ListView.
	 *      xml/searchable.xml - needed for Android searchable interface
	 *
	 * Used layer(s):
	 *  TMSMapLayer for base map
	 *        
	 * @author jaak
	 * @translated by m@t
	 */
	[Activity (Label = "AddressSearchActivity", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
	[IntentFilter(new string[]{"android.intent.action.SEARCH"})]	
	[MetaData(("android.app.searchable"), Resource = "@xml/searchable")]		
	public class AddressSearchActivity : Activity
	{
		#region const Fields

		private const int SEARCH_DIALOG = 1;
		private const String MAPQUEST_KEY = "Fmjtd%7Cluub2qu82q%2C70%3Do5-961w1w";

		#endregion
	
		#region private Fields

		private MapView _mapView;
		private ListView _listViewResults;
		private MarkerLayer _searchMarkerLayer;

		#endregion

		#region private static fields

		private static Dictionary<int, JSONObject> _listData;
		private static List<Marker> _searchResultPlaces;

		#endregion


		protected override void OnCreate ( Bundle bundle )
		{
			base.OnCreate ( bundle );

			SetContentView ( Resource.Layout.MainSearch );

			_searchResultPlaces = new List<Marker> ();

			HandleIntent ( Intent );

			Log.EnableAll ();
			Log.SetTag ( "addresssearch" );

			// 1. Get the MapView from the Layout xml - mandatory
			_mapView = FindViewById<MapView> ( Resource.Id.mapView );

			_listViewResults = FindViewById<ListView> ( Resource.Id.listViewResults );

		

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
			RasterLayer mapLayer = new RasterLayer ( dataSource, 0 );
 
			/// Set online base layer  
			_mapView.Layers.BaseLayer = mapLayer;
 
			//Tallin
			//_ mapView.SetFocusPoint ( 24.5f, 58.3f );
 
			// rotation - 0 = north-up
			_mapView.Rotation = 0f;
			_mapView.Zoom = 5.0f;

			// tilt means perspective view. Default is 90 degrees for "normal" 2D map view, minimum allowed is 30 degrees.
			_mapView.Tilt = 90.0f;
			// Activate some mapview options to make it smoother - optional
			_mapView.Options.Preloading = true;
			_mapView.Options.SeamlessHorizontalPan = true;
			_mapView.Options.TileFading = true;
			_mapView.Options.KineticPanning = true;
			_mapView.Options.DoubleClickZoomIn = true;
			_mapView.Options.DualClickZoomOut = true;

			// set sky bitmap - optional, default - white
			_mapView.Options.SkyDrawMode = Options.DrawBitmap;
			_mapView.Options.SkyOffset = 4.86f;
			_mapView.Options.SetSkyBitmap ( UnscaledBitmapLoader.DecodeResource ( Resources, Resource.Drawable.sky_small ) );

			// Map background, visible if no map tiles loaded - optional, default - white
			_mapView.Options.BackgroundPlaneDrawMode = Options.DrawBitmap;
			_mapView.Options.SetBackgroundPlaneBitmap( UnscaledBitmapLoader.DecodeResource ( Resources, Resource.Drawable.background_plane ) );

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
			zoomControls.ZoomInClick += ( sender, e ) => { _mapView.ZoomIn (); };
			zoomControls.ZoomOutClick += ( sender, e ) => { _mapView.ZoomOut (); };

			// create layer and add object to the layer, finally add layer to the map. 
			// All overlay layers must be same projection as base layer, so we reuse it
			_searchMarkerLayer = new MarkerLayer( _mapView.Layers.BaseLayer.Projection );

			// set listener for map events
			MapListener listener = new MyMapListener ();
			_mapView.Options.MapListener = listener;
		}

		private String[] GetLocationArray ()
		{
			List<string> values = new List<string> ();

			for ( int i = 0; i < _listData.Count; i++ ) 
			{
				JSONObject location = _listData [i];

				String street = location.OptString( "street" );
				String city = location.OptString( "adminArea5" ); // city
				String county = location.OptString( "adminArea4" ); // county
				String state = location.OptString( "adminArea3" ); // state
				String country = location.OptString( "adminArea1" ); // country

				StringBuilder labelText = new StringBuilder ( NotNull( street ) );
				labelText.Append (" ");
				labelText.Append (city);
				labelText.AppendLine ();
				labelText.Append ( NotNull ( county ) ); 
				labelText.Append (" ");
				labelText.Append ( NotNull ( state ) ); 
				labelText.Append (" ");
				labelText.Append ( NotNull ( country ) ); 
				values.Add ( labelText.ToString () );
			}

			return values.ToArray ();
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			_mapView.StartMapping ();
			Console.WriteLine ( "OnStart Places:" + _searchResultPlaces.Count );

		}

		protected override void OnResume ()
		{
			base.OnResume ();
			_listViewResults.ItemClick += ItemClickHandler;
			UpdateAdapter ();
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			_mapView.StopMapping ();
			Console.WriteLine ( "Pause Places:" + _searchResultPlaces.Count );
			_listViewResults.ItemClick -= ItemClickHandler;
		}

		void HandleIntent ( Intent intent )
		{
			if ( Intent.ActionSearch.Equals ( intent.Action ) ) 
			{
				string query = intent.GetStringExtra ( SearchManager.Query );
				ShowResults ( query ); 
			}
		}

		protected override void OnNewIntent ( Android.Content.Intent intent )
		{
			// Because this activity has set launchMode="singleTop", the system calls this method
			// to deliver the intent if this actvity is currently the foreground activity when
			// invoked again (when the user executes a search from this activity, we don't create
			// a new instance of this activity, so the system delivers the search intent here)
			HandleIntent ( intent );
		}

		void ShowResults ( string query )
		{
			// The search query is provided as an "extra" string in the query intent
			String queryString = query;

			// Do the actual search, write to searchResults field
			ShowDialog ( SEARCH_DIALOG );

			MapQuestGeocoder geocoder = new MapQuestGeocoder();
			geocoder.Geocode ( queryString, null, this, MAPQUEST_KEY );

		}

		public override bool OnCreateOptionsMenu ( IMenu menu )
		{
			MenuInflater.Inflate ( Resource.Menu.options_menu, menu );

			var searchManager = (SearchManager)GetSystemService ( Context.SearchService );
			var searchView = (SearchView)menu.FindItem ( Resource.Id.search ).ActionView;

			searchView.SetSearchableInfo ( searchManager.GetSearchableInfo ( ComponentName ) );
			searchView.SetIconifiedByDefault ( false );

			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch ( item.ItemId ) 
			{
				case Resource.Id.search:
					OnSearchRequested ();
					return true;

				default:
					return false;
			}
		}

		public void SearchResults ( JSONArray locations ) 
		{
			if( locations == null || locations.Length () == 0 )
			{
				Log.Debug( "no results found" );
				return;
			}

			Log.Debug( "geocode results: " + locations.Length () );

			_mapView.StartMapping ();
			_listData = new Dictionary<int, JSONObject> ();
			for ( int i=0; i< locations.Length(); i++ )
			{
				try 
				{
					JSONObject location = locations.GetJSONObject ( i );
					_listData.Add ( i, location );

				} 
				catch ( JSONException e )
				{
					e.PrintStackTrace ();
				}
			} 

			UpdateAdapter ();

		}

		private void UpdateAdapter ()
		{
			if ( ( _listData != null ) && ( _listData.Count  > 0) ) 
			{
				_listViewResults.Adapter = new ArrayAdapter<String> ( this, Android.Resource.Layout.SimpleListItem1, GetLocationArray () );
				_listViewResults.Visibility = ViewStates.Visible;

				//hide the keyboard
				InputMethodManager manager = (InputMethodManager)GetSystemService ( InputMethodService );
				manager.HideSoftInputFromWindow ( _listViewResults.WindowToken, 0 );

			} 
			else 
			{
				_listViewResults.Visibility = ViewStates.Gone;
			}
		}

		private void ItemClickHandler (object sender, AdapterView.ItemClickEventArgs e)
		{
			JSONObject location = _listData[e.Position];
			String street = location.OptString ( "street" );
			String city = location.OptString ( "adminArea5" ); // city
			String county = location.OptString ( "adminArea4" ); // county
			String state = location.OptString ( "adminArea3" ); // state
			String country = location.OptString ( "adminArea1" ); // country

			String line1 = NotNull ( street ) + " " + city;
			String line2 = NotNull ( county ) + " "+ NotNull ( state ) + " " + NotNull ( country );

			JSONObject latLng = location.GetJSONObject ( "latLng" );
			float lng = (float)latLng.GetDouble ( "lng" );
			float lat = (float)latLng.GetDouble ( "lat" );
			_listViewResults.Visibility = ViewStates.Gone;

			AddMarker ( line1, line2, lat, lng );

			//reset data
			_listData = new Dictionary<int, JSONObject> ();
		}

		/// <summary>
		/// It Adds a Marker in the map for the current location
		/// </summary>
		/// <param name="location">Location.</param>
		private void AddMarker ( string currentPositionTitle, string currentPositionSubtitle, float latitude, float longitude )
		{
			/// Define label what is shown when you click on marker.
			Label markerLabel = new DefaultLabel ( currentPositionTitle, currentPositionSubtitle );

			/// Define the location of the marker, it must be converted to base map coordinate system
			MapPos currentLocation = _mapView.Layers.BaseLayer.Projection.FromWgs84 ( longitude, latitude );

			///load default market style
			MarkerStyle markerStyle = DefaultMarkerStyle ().Build ();

			/// add the label to the Marker
			Marker currentPositionMarker = new Marker ( currentLocation, markerLabel, markerStyle, _searchMarkerLayer );

			_searchResultPlaces.Add ( currentPositionMarker );

			/// add the label to the layer
			_searchMarkerLayer.Add ( currentPositionMarker );

			///add layer to the map. 
			_mapView.Layers.AddLayer ( _searchMarkerLayer );

			_mapView.FocusPoint = currentLocation;

			_mapView.Zoom = 10f;
		}

		/// <summary>
		/// Defaults value for the marker style.
		/// </summary>
		/// <returns>The marker style.</returns>
		private MarkerStyle.Builder DefaultMarkerStyle ()
		{
			return DefineMarkerStyle ( Resource.Drawable.point, 0.4f, 0f );
		}

		/// <summary>
		/// Define marker style (image, size, color)
		/// </summary>
		/// <returns>The marker style.</returns>
		private MarkerStyle.Builder DefineMarkerStyle ( int resourceId, float size,float offset2D )
		{
			// define marker style 
			Bitmap pointMarker = UnscaledBitmapLoader.DecodeResource ( Resources, resourceId );

			MarkerStyle.Builder markerStyleBuilder = new MarkerStyle.Builder ();
			markerStyleBuilder.SetBitmap ( pointMarker );
			markerStyleBuilder.SetColor ( Nutiteq.SDK.Color.White );
			markerStyleBuilder.SetSize ( size );
			markerStyleBuilder.SetOffset2DX ( offset2D );
			return markerStyleBuilder;
		}

		private String NotNull ( String txt ) 
		{
			if(txt == null)
				return "";
			else
				return txt;
		}
	}
}

