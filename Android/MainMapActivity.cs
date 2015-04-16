
using System.Net;
using System.Text;
using System;
using System.Collections.Generic; 
using System.Linq;
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
using Android.Util;
using Android.App;

using Android.Support.V4.View;
using Android.Support.V4.App;

using Xamarin.Forms.Platform.Android;
using Android.Content.PM;

namespace Columbia583.Android
{
	[Activity (Label = "MainMapActivity", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]			
	public class MainMapActivity : FormsApplicationActivity
	{
		/// <summary>
		/// The Nutiteq MapView
		/// </summary>
		/// 
		const float MAX_MAP_LONG = -115.576172f;
		const float MIN_MAP_LONG = -117.070312f;
		const float MAX_MAP_LAT = 50.991286f;
		const float MIN_MAP_LAT = 50.041266f;
		private MapView _mapView;
		private MarkerLayer _markerLayer;
		private GeometryLayer _geometryLayer;
		protected global::Android.Widget.Button changeFiltersButton = null;
		Dictionary<string, List<string> > dictGroup = new Dictionary<string, List<string> > ();
		List<string> lstKeys = new List<string> ();
		protected ListableTrail[] trails = null;
		protected ListableTrail[] debugSearchResults = null;
		protected double[,] long_lat = null;
		private const int seeMapsDialog = 1;
		List<string> names= new List<string>();
		private string selectedTrail = null;
		private int position = 0;
		private string[,] trailheads = null;
		string[] trailnames = null;
		List<string> allkmlfiles = new List<string>();


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
			SetContentView (Resource.Layout.Map2);

			// enable Nutiteq SDK logging
			Nutiteq.SDK.Log.EnableAll ();

			/// Get our map from the layout resource. 
			_mapView = FindViewById<MapView> (Resource.Id.mapView2);

			/// Components keeps internal state and parameters for MapView
			_mapView.Components = new Components ();

			/// Define base projection, almost always EPSG3857, but others can be defined also
			EPSG3857 proj = new EPSG3857 ();

			/// Use packaged data source for tiles. Packaged tiles as stored as individual bitmaps under 'raw' resources. Only tiles up to zoom level 2 are included.
			IRasterDataSource dataSource = new PackagedRasterDataSource (proj, 10, 15, "t{zoom}_{x}_{y}", ApplicationContext); 
			// Alternative is to use offlines tiles in MBTiles format. MBTiles data source is included as sample and it uses file-based tile storage.
			//IRasterDataSource dataSource = new MBTilesRasterDataSource (proj, 10, 15, getExternalFilesDir(null).getAbsolutePath()+"/MBAtlas.mbtiles");
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
			_mapView.Zoom = 10;

			/// constrain zoom range as we have limited set of tiles
			_mapView.Constraints.ZoomRange = new Range (5, 17);

			float avg_lat = (MAX_MAP_LAT + MIN_MAP_LAT) / 2.0f;
			float avg_long = (MAX_MAP_LONG + MIN_MAP_LONG) / 2.0f;
			MapPos focusPoint = mapLayer.Projection.FromWgs84( avg_long, avg_lat);
			_mapView.FocusPoint = focusPoint;

			// get the zoomcontrols defined in main.xml, set zoom listeners
			ZoomControls zoomControls = FindViewById<ZoomControls> ( Resource.Id.zoomControls2 );
			zoomControls.ZoomInClick += ( sender, e ) => { _mapView.ZoomIn (); };
			zoomControls.ZoomOutClick += ( sender, e ) => { _mapView.ZoomOut (); };

			_markerLayer = new MarkerLayer ( _mapView.Layers.BaseLayer.Projection );
			_mapView.Layers.AddLayer ( _markerLayer );

			_geometryLayer = new GeometryLayer(_mapView.Layers.BaseLayer.Projection);
			_mapView.Layers.AddLayer(_geometryLayer);

			Bitmap pointMarker = UnscaledBitmapLoader.DecodeResource (Resources, Resource.Drawable.point);
			Bitmap lineMarker = UnscaledBitmapLoader.DecodeResource (Resources, Resource.Drawable.line);

			PointStyle pointStyle = new PointStyle.Builder ().Build ();
			StyleSet<PointStyle> pointStyleSet = new StyleSet<PointStyle>();

			pointStyle.PickingSize = 1.0f;
			pointStyle.Color = new Nutiteq.SDK.Color (1, 0, 0);

			LineStyle lineStyle = new LineStyle.Builder ().Build ();
			StyleSet<LineStyle> lineStyleSet = new StyleSet<LineStyle> ();

			lineStyle.Color = new Nutiteq.SDK.Color (0, 0, 255);
			lineStyle.PickingWidth = 100.0f;

			IList<MapPos> array_lat_long = new List<MapPos> ();

			if (long_lat != null) {
				for (int i = 0; i < long_lat.Length / 2; i++) {
					array_lat_long.Add (proj.FromWgs84 ((float)long_lat [i, 0], (float)long_lat [i, 1]));
				}
			} else {
				array_lat_long.Add (proj.FromWgs84 ((float)avg_long, (float)avg_lat));
				array_lat_long.Add (proj.FromWgs84 ((float)avg_long, (float)avg_lat));
			}

			//mapPos.Add (proj.FromWgs84((float)e_long, (float)e_lat));
			//mapPos.Add (proj.FromWgs84((float)Cal_long, (float)Cal_lat));
			//mapPos.Add (new MapPos (Cal_lat, Cal_long));

			_geometryLayer.Add(new Line(array_lat_long, new DefaultLabel("Line"), lineStyle, null));

			/// Add marker

			//AddMarker ("Start", "FocusPoint", avg_long, avg_lat);

			changeFiltersButton = FindViewById<global::Android.Widget.Button> (Resource.Id.changeFilters);

			if (changeFiltersButton != null){
				changeFiltersButton.Click += (sender, e) => {
					var intent = new Intent(this, typeof(SearchTrails));
					StartActivity(intent);
				};
			}
			Application_Layer_Search_Trails applicationLayer_searchTrails = new Application_Layer_Search_Trails ();
			string getResult = Intent.GetStringExtra ("search") ?? "No filter found";
			if (getResult != "No filter found") {
				SearchFilter searchFilter = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchFilter> (getResult);
				trails = applicationLayer_searchTrails.getTrailsBySearchFilter (searchFilter);
			}

			if (trails == null || trails.Length == 0) {
				debugSearchResults = applicationLayer_searchTrails.getTrailsBySearchFilter (new SearchFilter (){ rating = 1 });
				foreach(ListableTrail t in debugSearchResults){
					names.Add (t.trail.name);
				}
			} else {
				foreach(ListableTrail t in trails){
					names.Add (t.trail.name);
				}
			}

			fillKMLarray ();

			trailnames = names.ToArray ();
			trailheads = new string[trailnames.Length, 3];

			getTrailHeads ();


			var seeMaps = FindViewById<Button> (Resource.Id.viewTrail);
			seeMaps.Click += delegate {
				ShowDialog(seeMapsDialog);
			};
		}

		protected override Dialog OnCreateDialog(int id, Bundle args)
		{
			switch (id)
			{
			case seeMapsDialog:
				{
					ListableTrail[] listTrails;
					List<string> trailsToShow = new List<string> ();
					if (trails == null) {
						listTrails = debugSearchResults;
						foreach(ListableTrail trail in listTrails)
						{
							trailsToShow.Add(trail.trail.name);
						}
					}
					else{
						listTrails = trails;
						foreach(ListableTrail trail in listTrails)
						{
							trailsToShow.Add(trail.trail.name);
						}
					}
					var builder = new AlertDialog.Builder(this);
					builder.SetTitle("Select a Trail");
					builder.SetCancelable(true);
					builder.SetSingleChoiceItems(trailsToShow.ToArray(), -1, trailSelected);
					builder.SetPositiveButton(Resource.String.okButtonName, okClicked);
					builder.SetNegativeButton(Resource.String.negativeOption, cancelClicked);
					return builder.Create();
				}
			}
			return base.OnCreateDialog(id, args);
		}

		private void trailSelected(object sender, DialogClickEventArgs args)
		{
			Dialog dialog = (AlertDialog) sender;

			string[] items = names.ToArray ();

			selectedTrail = items [args.Which];

			position = args.Which;
		}

		private void okClicked(object sender, DialogClickEventArgs args)
		{
			Dialog dialog = (AlertDialog) sender;

			ListableTrail getTrail;

			readKML (getKMLString (selectedTrail));

			if(trails != null && trails.Length > 0) 
				getTrail = trails[position];
			else
				getTrail = debugSearchResults[position];
			var intent = new Intent (this, typeof(ViewTrailActivity));
			string trailJSONStr = Newtonsoft.Json.JsonConvert.SerializeObject (getTrail.trail);
			string activitiesJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(getTrail.activities);
			string amenitiesJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(getTrail.amenities);
			string pointsJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(getTrail.points);
			intent.PutExtra ("viewedTrail", trailJSONStr);
			intent.PutExtra("activities", activitiesJSONstr);
			intent.PutExtra("amenities", amenitiesJSONstr);
			intent.PutExtra("points", pointsJSONstr);
			StartActivity (intent);

			dialog.Dismiss ();
		}

		private void cancelClicked(object sender, DialogClickEventArgs args)
		{
			Dialog dialog = (AlertDialog)sender;

			dialog.Dismiss ();
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
			LabelStyle labelStyle = new LabelStyle.Builder ().Build ();	

			/// Define label what is shown when you click on marker.
			Label markerLabel = new DefaultLabel ( title, subtitle, labelStyle );

			/// Define the location of the marker, it must be converted to EPSG3857 coordinate system
			MapPos mapLocation = _markerLayer.Projection.FromWgs84 ( longitude, latitude );

			///load default market style
			MarkerStyle markerStyle = DefaultMarkerStyle ().Build ();

			/// add the label to the Marker
			Marker marker = new Marker ( mapLocation, markerLabel, markerStyle, null );

			/// add the label to the layer
			_markerLayer.Add ( marker );

			labelStyle.DescriptionSize = 32;
			labelStyle.TitleSize = 48;
			labelStyle.BackgroundColor = Nutiteq.SDK.Color.White;
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

		private void fillKMLarray()
		{
			allkmlfiles.Add("5 passes.kml");
			allkmlfiles.Add("Bear Lake.kml");
			allkmlfiles.Add("Brewer Creek Loop.kml");
			allkmlfiles.Add("Chalice.kml");
			allkmlfiles.Add("Cobalt Lake.kml");
			allkmlfiles.Add("Cobb Lake Trail - KNP.kml");
			allkmlfiles.Add("Conrad Kain.kml");
			allkmlfiles.Add("Diana Lake.kml");
			allkmlfiles.Add("Dog Lake - KNP.kml");
			allkmlfiles.Add("Findlay Falls.kml");
			allkmlfiles.Add("Fireweed Loop - lower.kml");
			allkmlfiles.Add("Fireweed Loop - upper.kml");
			allkmlfiles.Add("Gibraltar Trail.kml");
			allkmlfiles.Add("Graves Lookout.kml");
			allkmlfiles.Add("Hoodoos Loop Track.kml");
			allkmlfiles.Add("Dragonfly Boardwalk.kml");
			allkmlfiles.Add("Juniper Trail - KNP.kml");
			allkmlfiles.Add("Kimpton Trail - KNP.kml");
			allkmlfiles.Add("Kindersley Creek Trail - KNP.kml");
			allkmlfiles.Add("Lake Enid.kml");
			allkmlfiles.Add("Lake of the Hanging Glacier.kml");
			allkmlfiles.Add("Lakit Lookout.kml");
			allkmlfiles.Add("Lower Bugaboo Falls.kml");
			allkmlfiles.Add("Marble Canyon.kml");
			allkmlfiles.Add("Aurora Creek_Marvel Pass.kml");
			allkmlfiles.Add("Mause Creek Tarns_Tanglefoot Lake.kml");
			allkmlfiles.Add("McLean Lake.kml");
			allkmlfiles.Add("Teepee Mountain.kml");
			allkmlfiles.Add("Mt Swansea SE Ridge.kml");
			allkmlfiles.Add("Mt Swansea.kml");
			allkmlfiles.Add("Olive Lake Trail -KNP.kml");
			allkmlfiles.Add("Paint Pots Trail - KNP.kml");
			allkmlfiles.Add("Paint Pots to Marble Canyon Trail - KNP.kml");
			allkmlfiles.Add("Pedley Loop.kml");
			allkmlfiles.Add("Premier Lake Trails.kml");
			allkmlfiles.Add("Ptarmigan Lake.kml");
			allkmlfiles.Add("Redstreak Campground Trail -KNP.kml");
			allkmlfiles.Add("Redstreak Creek Trail - KNP.kml");
			allkmlfiles.Add("Redstreak Loop -KNP.kml");
			allkmlfiles.Add("Redstreak Restoration Trail - KNP.kml");
			allkmlfiles.Add("Septet Pass.kml");
			allkmlfiles.Add("Sinclair Creek Trail.kml");
			allkmlfiles.Add("Sinclair Creek Trail - KNP.kml");
			allkmlfiles.Add("Source of the Columbia.kml");
			allkmlfiles.Add("Stanley Glacier -TF.kml");
			allkmlfiles.Add("Templeton.kml");
			allkmlfiles.Add("Top of the World - Coyote Creek campst.kml");
			allkmlfiles.Add("Top of the World - Fish Lake.kml");
			allkmlfiles.Add("Top of the World - Sparkle Lake.kml");
			allkmlfiles.Add("Top of the World - Wildhorse.kml");
			allkmlfiles.Add("Welsh Lakes - lower only.kml");
			allkmlfiles.Add("Whiteswan Lake.kml");
		}

		private void getTrailHeads()
		{
			int traverseLength = 0;

			List<string> searchKMLs = new List<string> ();

			if (trails != null) {
				foreach (ListableTrail trail in trails) {
					searchKMLs.Add (getKMLString(trail.trail.name));
				}
			} else {
				foreach (ListableTrail trail in debugSearchResults) {
					searchKMLs.Add (getKMLString(trail.trail.name));
				}
			}

			traverseLength = searchKMLs.Count;

			for(int i = 0; i < traverseLength; i++)
			{
				string urlLookup = @"http://trails.greenways.ca/kml/";

				string webReqUrl = urlLookup + searchKMLs.ElementAt (i);

				HttpWebRequest webRequest;

				try{
					webRequest = (HttpWebRequest) WebRequest.Create(webReqUrl);

					using (HttpWebResponse response = (HttpWebResponse) webRequest.GetResponse())
					using (var content = response.GetResponseStream ())
					using (var reader = new StreamReader (content)) {
						var strContent = reader.ReadToEnd ();
						var nameContent = strContent;
						int namestart = nameContent.LastIndexOf("<Placemark><name>") + "<Placemark><name>".Length;
						int namelength = nameContent.IndexOf("</name><Style><LineStyle><color>") - namestart;
						string name = nameContent.Substring(namestart, namelength);
						int start = strContent.LastIndexOf ("<LineString><coordinates>") + "<LineString><coordinates>".Length;
						int length = strContent.IndexOf ("</coordinates></LineString>") - start;
						string sub = strContent.Substring (start, length);

						string[] tokens = sub.Split (new char[2]{ ' ', ',' });

						trailheads [i, 0] = name; 
						trailheads [i, 1] = tokens [0];
						trailheads [i, 2] = tokens [1];

						AddMarker(trailheads[i, 0], "Trail Head", Convert.ToDouble(trailheads[i, 1]),  Convert.ToDouble(trailheads[i, 2]));
					}
				}catch(Exception e){
					trailheads [i, 0] = "0";
					trailheads [i, 1] = "0";
					trailheads [i, 2] = "0";
					Console.WriteLine (e.Message);
				}
			}

			for (int j = 0; j < trailnames.Length; j++) {
				AddMarker (trailheads [j, 0], "Trail Head", Convert.ToDouble(trailheads [j, 1]), Convert.ToDouble(trailheads [j, 2]));
			}
		}


		private string getKMLString(string trailName)
		{
			string kmlFile;

			if (trailName.Contains ("Bear Lake")) {
				kmlFile = "Bear Lake.kml";
			}
			else if (trailName.Contains ("5 passes")) {
				kmlFile = "5 passes.kml";
			} 
			else if (trailName.Contains ("Brewer Creek")) {
				kmlFile = "Brewer Creek Loop.kml";
			}
			else if (trailName.Contains("Chalice Creek")){
				kmlFile = "Chalice.kml";
			}
			else if (trailName.Contains ("Cobalt Lake")) {
				kmlFile = "Cobalt Lake.kml";
			}
			else if (trailName.Contains ("Cobb Lake")) {
				kmlFile = "Cobb Lake Trail - KNP.kml";
			}
			else if (trailName.Contains ("Conrad Kain")) {
				kmlFile = "Conrad Kain.kml";
			}
			else if (trailName.Contains ("Diana Lake")) {
				kmlFile = "Diana Lake.kml";
			}
			else if (trailName.Contains ("Dog Lake")) {
				kmlFile = "Dog Lake - KNP.kml";
			}
			else if (trailName.Contains ("Findlay Falls")) {
				kmlFile = "Findlay Falls.kml";
			}
			else if (trailName.Contains ("Fireweed Loop - lower - TF")) {
				kmlFile = "Fireweed Loop - lower.kml";
			}
			else if (trailName.Contains ("Fireweed Loop - upper - TF")) {
				kmlFile = "Fireweed Loop - upper.kml";
			}
			else if (trailName.Contains ("Gibraltar Lookout")) {
				kmlFile = "Gibraltar Trail.kml";
			}
			else if (trailName.Contains ("Graves Lookout")) {
				kmlFile = "Graves Lookout.kml";
			}
			else if (trailName.Contains ("Hoodoos Trail")) {
				kmlFile = "Hoodoos Loop Track.kml";
			}
			else if (trailName.Contains ("James Chabot")) {
				kmlFile = "Dragonfly Boardwalk.kml";
			}
			else if (trailName.Contains ("Juniper Trail")) {
				kmlFile = "Juniper Trail - KNP.kml";
			}
			else if (trailName.Contains ("Kimpton Creek")) {
				kmlFile = "Kimpton Trail - KNP.kml";
			}
			else if (trailName.Contains ("Kindersley Pass")) {
				kmlFile = "Kindersley Creek Trail - KNP.kml";
			}
			else if (trailName.Contains ("Lake Enid")) {
				kmlFile = "Lake Enid.kml";
			}
			else if (trailName.Contains ("Lake of the Hanging Glacier")) {
				kmlFile = "Lake of the Hanging Glacier.kml";
			}
			else if (trailName.Contains ("Lakit Lookout")) {
				kmlFile = "Lakit Lookout.kml";
			}
			else if (trailName.Contains ("Lower Bugaboo Falls")) {
				kmlFile = "Lower Bugaboo Falls.kml";
			}
			else if (trailName.Contains ("Marble Canyon")) {
				kmlFile = "Marble Canyon.kml";
			}
			else if (trailName.Contains ("Marvel Pass")) {
				kmlFile = "Aurora Creek_Marvel Pass.kml";
			}
			else if (trailName.Contains ("Mause Creek")) {
				kmlFile = "Mause Creek Tarns_Tanglefoot Lake.kml";
			}
			else if (trailName.Contains ("McLean Lake")) {
				kmlFile = "McLean Lake.kml";
			}
			else if (trailName.Contains ("Mount Stevens")) {
				kmlFile = "Teepee Mountain.kml";
			}
			else if (trailName.Contains ("Mount Swansea - SE Ridge")) {
				kmlFile = "Mt Swansea SE Ridge.kml";
			}
			else if (trailName.Contains ("Mount Swansea W Ridge")) {
				kmlFile = "Mt Swansea.kml";
			}
			else if (trailName.Contains ("Olive Lake")) {
				kmlFile = "Olive Lake Trail -KNP.kml";
			}
			else if (trailName.Contains ("Paint Pots")) {
				kmlFile = "Paint Pots Trail - KNP.kml";
			}
			else if (trailName.Contains ("Paint Pots via Marble Canyon")) {
				kmlFile = "Paint Pots to Marble Canyon Trail - KNP.kml";
			}
			else if (trailName.Contains ("Pedley Pass")) {
				kmlFile = "Pedley Loop.kml";
			}
			else if (trailName.Contains ("Premier Lake")) {
				kmlFile = "Premier Lake Trails.kml";
			}
			else if (trailName.Contains ("Ptarmigan Lake")) {
				kmlFile = "Ptarmigan Lake.kml";
			}
			else if (trailName.Contains ("Redstreak Campground")) {
				kmlFile = "Redstreak Campground Trail -KNP.kml";
			}
			else if (trailName.Contains ("Redstreak Creek")) {
				kmlFile = "Redstreak Creek Trail - KNP.kml";
			}
			else if (trailName.Contains ("Redstreak Loop")) {
				kmlFile = "Redstreak Loop -KNP.kml";
			}
			else if (trailName.Contains ("Redstreak Restoration")) {
				kmlFile = "Redstreak Restoration Trail - KNP.kml";
			}
			else if (trailName.Contains ("Septet Pass")) {
				kmlFile = "Septet Pass.kml";
			}
			else if (trailName.Contains ("Sinclair Creek Greenway")) {
				kmlFile = "Sinclair Creek Trail.kml";
			}
			else if (trailName.Contains ("Sinclar Creek Trail")) {
				kmlFile = "Sinclair Creek Trail - KNP.kml";
			}
			else if (trailName.Contains ("Source of the Columbia Pathway")) {
				kmlFile = "Source of the Columbia.kml";
			}
			else if (trailName.Contains ("Stanley Glacier")) {
				kmlFile = "Stanley Glacier -TF.kml";
			}
			else if (trailName.Contains ("Templeton Lake")) {
				kmlFile = "Templeton.kml";
			}
			else if (trailName.Contains ("Coyote Campsite/Sugarloaf")) {
				kmlFile = "Top of the World - Coyote Creek campst.kml";
			}
			else if (trailName.Contains ("Fish Lake")) {
				kmlFile = "Top of the World - Fish Lake.kml";
			}
			else if (trailName.Contains ("Sparkle Lake")) {
				kmlFile = "Top of the World - Sparkle Lake.kml";
			}
			else if (trailName.Contains ("Wildhorse Ridge")) {
				kmlFile = "Top of the World - Wildhorse.kml";
			}
			else if (trailName.Contains ("Welsh Lakes")) {
				kmlFile = "Welsh Lakes - lower only.kml";
			}
			else if (trailName.Contains ("Whiteswan Lake")) {
				kmlFile = "Whiteswan Lake.kml";
			}
			else{
				kmlFile = "Findlay Falls.kml";
			}

			return kmlFile;
		}

		private void readKML(string kmlString)
		{
			string urlLookup = @"http://trails.greenways.ca/kml/";

			string webReqUrl = urlLookup + kmlString;

			Console.WriteLine ("Web Request made to: {0}", webReqUrl);

			try{
				HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(webReqUrl);

				using (HttpWebResponse response = (HttpWebResponse) webRequest.GetResponse())
				using (var content = response.GetResponseStream ())
				using (var reader = new StreamReader (content)) {
					var strContent = reader.ReadToEnd ();
					var path = @"/storage/emulated/0/Documents/";
					var filename = System.IO.Path.Combine(path, "output.txt");

					System.IO.File.WriteAllText(filename, strContent);

					getCoordinatesFromFile ();
				}
			}
			catch(Exception e){
				long_lat = new double[0,0];
				Console.WriteLine (e.Message);
			}
		}

		void getCoordinatesFromFile()
		{
			int token_length = 0;

			string path = @"/storage/emulated/0/Documents/output.txt";

			string fileContents = System.IO.File.ReadAllText(path);
			int start = fileContents.LastIndexOf ("<LineString><coordinates>") + "<LineString><coordinates>".Length;
			int length = fileContents.IndexOf ("</coordinates></LineString>") - start;
			string sub = fileContents.Substring (start, length);

			string[] longitude_array;
			string[] latitude_array;

			//Console.WriteLine (sub);
			string[] tokens = sub.Split(new char[2]{' ', ','});

			token_length = tokens.Length;
			longitude_array = new string[token_length/3];
			latitude_array = new string[token_length/3];

			for(int i = 0; i < token_length/3; i++) 
			{
				if (tokens [3 * i] != String.Empty || tokens[3*i] != null
					|| tokens [3 * i + 1] != String.Empty || tokens[3 * i + 1] != null) 
				{
					longitude_array [i] = tokens [3 * i];
					latitude_array [i] = tokens [3 * i + 1];
				}
			}

			long_lat = new double[longitude_array.Length, 2];


			for (int j = 0; j < longitude_array.Length; j++) 
			{
				long_lat [j, 0] = Convert.ToDouble(longitude_array [j]);
				long_lat [j, 1] = Convert.ToDouble(latitude_array [j]);
			}
		}
	}
}
