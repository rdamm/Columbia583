
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

using Android.Support.V4.View;
using Android.Support.V4.App;

using Xamarin.Forms.Platform.Android;

namespace Columbia583.Android
{
	[Activity (Label = "TestMapActivity")]			
	public class MainMapActivity : FormsApplicationActivity
	{
		/// <summary>
		/// The Nutiteq MapView
		/// </summary>
		private MapView _mapView;
		private MarkerLayer _markerLayer;
		private GeometryLayer _geometryLayer;
		protected global::Android.Widget.Button changeFiltersButton = null;
		protected bool preventSpinnerSelectEventFiringOnCreate = true;
		Dictionary<string, List<string> > dictGroup = new Dictionary<string, List<string> > ();
		List<string> lstKeys = new List<string> ();
		protected SearchResult[] trails = null;
		protected SearchResult[] debugSearchResults = null;
		protected double[,] long_lat = null;

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
			Log.EnableAll ();

			/// Get our map from the layout resource. 
			_mapView = FindViewById<MapView> (Resource.Id.mapView2);

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
			_mapView.Constraints.ZoomRange = new Range (11, 20);

			double Cal_lat = 51.0486f, Cal_long = -114.0708f, e_lat = 50.70f, e_long = -116.132f;
			MapPos focusPoint = mapLayer.Projection.FromWgs84( e_long, e_lat);
			_mapView.FocusPoint = focusPoint;

			// get the zoomcontrols defined in main.xml, set zoom listeners
			ZoomControls zoomControls = FindViewById<ZoomControls> ( Resource.Id.zoomControls2 );
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

			lineStyle.Color = new Nutiteq.SDK.Color (0, 0, 255);
			lineStyle.PickingWidth = 100.0f;

			IList<MapPos> array_lat_long = new List<MapPos> ();
			/*float[] mapPosArray = {
				-115.8953927736605f,
				50.67251672121988f,
				-115.8954425128605f,
				50.67251432246382f,
				-115.895483018777f,
				50.67252044669117f,
				-115.8959401738663f,
				50.67247885560596f,
				-115.8959407123982f,
				50.67245589934966f,
				-115.8958268219555f,
				50.67242131415763f,
				-115.8957205244098f,
;				50.67240789053411f,
;				-115.8951616319567f,
;				50.6722496441577f,
;				-115.8945393619578f,
;				50.67190632415775f,
;				-115.893831251959f,
;				50.67175612415782f,
;				-115.8931231519602f,
;				50.67154154415786f,
;				-115.8923935919615f,
;				50.67147717415789f,
;				-115.892136101962f,
;				50.67128405415794f,
;				-115.8920502719621f,
;				50.67115530415796f,
;				-115.8918571519624f,
;				50.67091927415801f,
;				-115.8910417619639f,
;				50.67076906415805f,
;				-115.8905053119648f,
;				50.67072615415806f,
;				-115.890398031965f,
;				50.67068323415806f,
;				-115.8899688719657f,
;				50.67033991415816f,
;				-115.8894538919666f,
;				50.67001805415822f,
;				-115.8888530719677f,
;				50.66973910415825f,
;				-115.8882737219686f,
;				50.66937432415836f,
;				-115.88747978197f,
;				50.66905245415843f,
;				-115.8873724919702f,
;				50.66898808415841f,
;				-115.8872866619703f,
;				50.66894516415842f,
;				-115.8871243158843f,
;				50.66882451307762f,
;				-115.8871150019707f,
;				50.66879496415847f,
;				-115.8871793719705f,
;				50.66877350415846f,
;				-115.88747978197f,
;				50.66881642415844f,
;				-115.8878445619694f,
;				50.6689666241584f,
;				-115.8883595519685f,
;				50.66894516415842f,
;				-115.8888069933468f,
;				50.66909601442462f,
;				-115.8888530719676f,
;				50.66909537415838f,
;				-115.8888530719676f,
;				50.66900954415835f,
;				-115.888617041968f,
;				50.66879496415838f,
;				-115.8886382823692f,
;				50.66878155686087f,
;				-115.8887887019677f,
;				50.66879496415838f,
;				-115.8892607719669f,
;				50.66894516415835f,
;				-115.8897757519659f,
;				50.66892370415837f,
;				-115.8904624019647f,
;				50.66883787415835f,
;				-115.8911705019635f,
;				50.66870913415832f,
;				-115.8911793721391f,
;				50.66865651239261f,
;				-115.8910278819772f,
;				50.66856896578864f,
;				-115.8904624019647f,
;				50.66853747415838f,
;				-115.889689921966f,
;				50.66836581415842f,
;				-115.8890813487874f,
;				50.66808918504067f,
;				-115.8889371937311f,
;				50.66798075849497f,
;				-115.8888357154974f,
;				50.66787933493015f
;			};
;			for (int i = 0; i < mapPosArray.Length; i+=2) {
;				mapPos.Add(proj.FromWgs84(mapPosArray[i], mapPosArray[i+1]));
;			}*/
			if (long_lat != null) {
				for (int i = 0; i < long_lat.Length / 2; i++) {
					array_lat_long.Add (proj.FromWgs84 ((float)long_lat [i, 0], (float)long_lat [i, 1]));
				}
			} else {
				array_lat_long.Add (proj.FromWgs84 ((float)e_long, (float)e_lat));
				array_lat_long.Add (proj.FromWgs84 ((float)e_long, (float)e_lat));
			}
			//mapPos.Add (proj.FromWgs84((float)e_long, (float)e_lat));
			//mapPos.Add (proj.FromWgs84((float)Cal_long, (float)Cal_lat));
			//mapPos.Add (new MapPos (Cal_lat, Cal_long));

			_geometryLayer.Add(new Line(array_lat_long, new DefaultLabel("Line"), lineStyle, null));

			/// Add marker
			AddMarker ("Marker", "Calgary", Cal_long, Cal_lat);
			AddMarker ("Edgewater", "Default Location", e_long, e_lat);
			AddMarker ("Start", "TestTrail", -115.8953927736605f, 50.67251672121988f);

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

			List<string> names= new List<string>();
			if (trails == null || trails.Length == 0) {
				debugSearchResults = applicationLayer_searchTrails.getTrailsBySearchFilter (new SearchFilter (){ rating = 1 });
				foreach(SearchResult t in debugSearchResults){
					names.Add (t.trail.name);
				}
			} else {
				foreach(SearchResult t in trails){
					names.Add (t.trail.name);
				}
			}
			var spinner = FindViewById<Spinner> (Resource.Id.Spinner2);
			ArrayAdapter _adapterFrom = new ArrayAdapter (this, global::Android.Resource.Layout.SimpleSpinnerItem, names.ToArray());
			_adapterFrom.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = _adapterFrom;
			spinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) => {
				if (preventSpinnerSelectEventFiringOnCreate == true)
				{
					preventSpinnerSelectEventFiringOnCreate = false;
					return;
				}
				int position = spinner.SelectedItemPosition;
				string name = spinner.GetItemAtPosition(position).ToString();
				readKML(getKMLString(name));
				if(e.Position == position){
					String foundName = names.ElementAt(position);
					Console.Out.WriteLine(foundName);
					SearchResult getTrail;
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
				}
			};
				
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

		private string getKMLString(string trailName)
		{
			string kmlFile;

			if (trailName.Contains ("5 passes")) {
				kmlFile = "5 passes.kml";
			} else
				kmlFile = "Chalice.kml";

			return kmlFile;
		}

		private void readKML(string kmlString)
		{
			string urlLookup = @"http://trails.greenways.ca/kml/";

			string webReqUrl = urlLookup + kmlString;

			Console.WriteLine ("Web Request made to: {0}", webReqUrl);

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


			//var url = new Uri ("http://trails.greenways.ca/kml/Templeton.kml");	

			//webClient.Headers [HttpRequestHeader.IfModifiedSince] = "Sat, 29 Oct 1994 19:43:31 GMT";

			//webClient.Encoding = Encoding.UTF8;

			//webClient.DownloadStringAsync (url);

			/*webClient.DownloadStringCompleted += (sender, e) => {
				try
				{
;					var text = e.Result; // get the downloaded text
;					var path = @"/storage/emulated/0/Documents/";
;					var filename = System.IO.Path.Combine(path, "output.txt");
;
;					global_filename = filename;
;					string web_text = text;
;					Console.WriteLine (web_text);
;
;					File.WriteAllText (filename, text);
;				}
;				catch(Exception ex){
;					Console.WriteLine("Jack's Exception: " + ex.Message);
;				}
;			};*/
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

			/*for (int k = 0; k < long_lat.Length/2; k++) 
			{
				if (long_lat [k, 0] > 0)
					long_lat [k, 0] = long_lat [k, 0] * -1;
;			}*/

			/*array_size = counter_two/2;
;			long_lat = new double[array_size,2];
;
;			foreach (string line in lines) 
;			{
;				long_lat [counter_two, 0] = Convert.ToDouble (lines [3 * counter_two]);
;				long_lat [counter_two, 1] = Convert.ToDouble (lines [3 * counter_two + 1]);
;				counter_two += 1;
;			}
;
;			for (int i = 0; i < array_size; i++) 
;			{
;				long_lat [i, 0] = Convert.ToDouble(lines [2 * i]);
;				long_lat [i, 1] = Convert.ToDouble(lines [2 * i + 1]);
;			}*/
		}
	}
}

 using
