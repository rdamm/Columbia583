
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Net;

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
	public class TestMapActivity :FormsApplicationActivity
	{
		/// <summary>
		/// The Nutiteq MapView
		/// </summary>
		private MapView _mapView;
		private MarkerLayer _markerLayer;
		private GeometryLayer _geoLayer;
		private VectorLayer _vectorLayer;
		private double[,] long_lat;
		private string global_filename;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

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
			IRasterDataSource dataSource = new PackagedRasterDataSource (proj, 0, 14, "t{zoom}_{x}_{y}", ApplicationContext);
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
			_mapView.Zoom = 10;

			/// constrain zoom range as we have limited set of tiles
			_mapView.Constraints.ZoomRange = new Range (0, 15);

			double Cal_lat = 51.0486f, Cal_long = -114.0708f, e_lat = 50.70f, e_long = -116.132f;
			/*double e1_long = -116.132f, e1_lat = 50.70f, e2_long = -116.1323, e2_lat = 50.71f;
			double e3_long = -116.1326f, e3_lat = 50.72f, e4_long = -116.1329, e4_lat = 50.73f;
			double e5_long = -116.1326f, e5_lat = 50.74f, e6_long = -116.1323, e6_lat = 50.75f;*/
			MapPos focusPoint = mapLayer.Projection.FromWgs84( e_long, e_lat);
			_mapView.FocusPoint = focusPoint;

			// get the zoomcontrols defined in main.xml, set zoom listeners
			ZoomControls zoomControls = FindViewById<ZoomControls> ( Resource.Id.zoomcontrols );
			zoomControls.ZoomInClick += ( sender, e ) => { _mapView.ZoomIn (); };
			zoomControls.ZoomOutClick += ( sender, e ) => { _mapView.ZoomOut (); };

			_markerLayer = new MarkerLayer ( _mapView.Layers.BaseLayer.Projection );
			_mapView.Layers.AddLayer ( _markerLayer );

			/// Add marker
			AddMarker ("Marker", "Calgary", Cal_long, Cal_lat);
			AddMarker ("Edgewater", "Default Location", e_long, e_lat);

			_geoLayer = new GeometryLayer(_mapView.Layers.BaseProjection);
			_mapView.Layers.AddLayer (_geoLayer);

			/*string[] directories = Directory.GetDirectories(@"/storage/emulated/0/Documents");
			foreach (string directory in directories)
			{
				Console.WriteLine (directory);
			}*/
			readKML ();

			IList<MapPos> array_lat_long = new List<MapPos>();

			//Directory.SetCurrentDirectory (@"/mnt/extSdCard/");

			for (int i = 0; i < long_lat.Length/2; i++) 
			{
				array_lat_long.Add(proj.FromWgs84((float)long_lat[i,0], (float)long_lat[i,1]));
			}

			LineStyle lineStyle = new LineStyle.Builder().Build();
			StyleSet<LineStyle> lineStyleSet = new StyleSet<LineStyle> ();

			lineStyle.LineJoinMode = LineStyle.RoundLinejoin;
			lineStyle.PointStyle = null;
			lineStyle.PickingWidth = 0.5f;
			lineStyle.Color = new Nutiteq.SDK.Color(Nutiteq.SDK.Color.Blue);
			lineStyleSet.SetZoomStyle (5, lineStyle);

			Line line = new Line (array_lat_long, new DefaultLabel("LINE"), lineStyleSet, null);
			line.SetStyleSet (lineStyleSet);
			line.VertexList = array_lat_long;
			line.Visible = true;

			//_geoLayer.Add(new Nutiteq.SDK.Point(mapLayer.Projection.FromWgs84(e_long, e_lat), new DefaultLabel("POINT"), pointStyleSet, null));
			_geoLayer.Add(line);
			_geoLayer.Visible = true;
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

		void readKML(string kmlString)
		{
			HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(@"http://trails.greenways.ca/kml/Pedley Loop.kml");

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
					var text = e.Result; // get the downloaded text
					var path = @"/storage/emulated/0/Documents/";
					var filename = System.IO.Path.Combine(path, "output.txt");

					global_filename = filename;
					string web_text = text;
					Console.WriteLine (web_text);

					File.WriteAllText (filename, text);
				}
				catch(Exception ex){
					Console.WriteLine("Jack's Exception: " + ex.Message);
				}
			};*/
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
			}*/

			/*array_size = counter_two/2;
			long_lat = new double[array_size,2];

			foreach (string line in lines) 
			{
				long_lat [counter_two, 0] = Convert.ToDouble (lines [3 * counter_two]);
				long_lat [counter_two, 1] = Convert.ToDouble (lines [3 * counter_two + 1]);
				counter_two += 1;
			}

			for (int i = 0; i < array_size; i++) 
			{
				long_lat [i, 0] = Convert.ToDouble(lines [2 * i]);
				long_lat [i, 1] = Convert.ToDouble(lines [2 * i + 1]);
			}*/
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

