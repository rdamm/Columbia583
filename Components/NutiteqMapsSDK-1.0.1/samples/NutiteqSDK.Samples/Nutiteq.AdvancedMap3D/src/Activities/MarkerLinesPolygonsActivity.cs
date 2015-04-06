using System;
using System.Runtime.InteropServices;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
//using jsqlite;
using Java.IO;
using System.Collections.Generic;
using Nutiteq.SDK;

namespace Nutiteq.AdvancedMap3D
{
	/// <summary>
	/// Deomstrates basic usage of geometry elements: markers with different styles, lines, polygons.
	/// </summary>
	[Activity (Label = "MarkerLinesPolygonsActivity")]			
	public class MarkerLinesPolygonsActivity : Activity
	{
		/// <summary>
		/// The marker layer.
		/// </summary>
		private MarkerLayer _markerLayer;

		/// <summary>
		/// The Lines layer.
		/// </summary>
		private GeometryLayer _linesLayer;

		/// <summary>
		/// The Lines layer.
		/// </summary>
		private GeometryLayer _polygonLayer;

		/// <summary>
		/// The Text layer.
		/// </summary>
		private TextLayer _textLayer;

		/// <summary>
		/// The Nutiteq MapView
		/// </summary>
		private MapView _mapView;

		protected override void OnCreate ( Bundle bundle )
		{
			base.OnCreate ( bundle );

			/// Set our view from the "main" layout resource
			SetContentView ( Resource.Layout.Main );

			// enable Nutiteq SDK logging
			Log.EnableAll ();

			/// Get our map from the layout resource. 
			_mapView = FindViewById<MapView> ( Resource.Id.mapView );

			/// Components keeps internal state and parameters for MapView
			_mapView.Components = new Components ();

			/// Set options
			_mapView.Options.KineticPanning = true;
			_mapView.Options.BackgroundPlaneDrawMode = Options.DrawBitmap;
			_mapView.Options.SetBackgroundPlaneBitmap (UnscaledBitmapLoader.DecodeResource (Resources, Resource.Drawable.background_plane));

			/// Define base projection, almost always EPSG3857, but others can be defined also
			EPSG3857 prog = new EPSG3857 ();

			/// define the map layer MapQuest Open Tiles
			EPSG3857 proj = new EPSG3857 ();
			IRasterDataSource dataSource = new HTTPRasterDataSource (proj, 0, 18, "http://otile1.mqcdn.com/tiles/1.0.0/osm/{zoom}/{x}/{y}.png");
			RasterLayer mapLayer = new RasterLayer (dataSource, 0);

			/// Set online base layer  
			_mapView.Layers.BaseLayer = mapLayer;

			// create layer and add markers to the layer, finally add layer to the map. 
			// All overlay layers must be same projection as base layer, so we reuse it
			_markerLayer = new MarkerLayer ( _mapView.Layers.BaseLayer.Projection );
			_mapView.Layers.AddLayer ( _markerLayer );

			// create layer for the lines 
			_linesLayer = new GeometryLayer ( _mapView.Layers.BaseLayer.Projection );
			_mapView.Layers.AddLayer ( _linesLayer );

			// create layer for the polygon 
			_polygonLayer = new GeometryLayer ( _mapView.Layers.BaseLayer.Projection );
			_mapView.Layers.AddLayer ( _polygonLayer );

			// create layer for Texts 
			_textLayer = new TextLayer ( _mapView.Layers.BaseLayer.Projection );
			_mapView.Layers.AddLayer ( _textLayer );


			// zoom - 0 = world, like on most web maps
			_mapView.Zoom = 3f;

			//add markers to the map
			AddMarker ( "San Francisco", "California", 37.766667f, -122.416667f );
			AddMarker ( "London", "United Kingdom", 51.0f , 0.0f );
			AddMarker ( "Tallinn", "Estonia", 54.43f, 24.74f );

			//set positions
			MapPos position1 = _mapView.Layers.BaseLayer.Projection.FromWgs84 ( -122.416667f,  37.766667f );
			MapPos position2 = _mapView.Layers.BaseLayer.Projection.FromWgs84 ( 51.0f, 0.0f );
			MapPos position3 = _mapView.Layers.BaseLayer.Projection.FromWgs84 ( 54.43f, 24.74f );
			MapPos position4 = _mapView.Layers.BaseLayer.Projection.FromWgs84 ( 24.74f, 54.43f );
			MapPos position5 = _mapView.Layers.BaseLayer.Projection.FromWgs84 ( 0.0f, 51.0f );

			//add a line
			MapPos[] linePos = new MapPos[] { position1, position2, position3 };
			AddLine( linePos, 0.2f, Nutiteq.SDK.Color.White, "example of line" );

			// add a text to the line
			AddText ( position2 , 32, Android.Graphics.Color.SaddleBrown, "Text on map" );
			AddText ( position3 , 100, Nutiteq.SDK.Color.Green, "Green text" );

			//add a polygon
			AddPolygon ( new MapPos[]{ position3, position4, position5  }, Nutiteq.SDK.Color.Green, "example of polygon" );
		
			//add a text for the line

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

		/// <summary>
		/// It Adds a Marker in the map for the current location
		/// </summary>
		/// <param name="location">Location.</param>
		void AddMarker ( string title, string subtitle, float latitude, float longitude )
		{
			/// Define label what is shown when you click on marker.
			Label markerLabel = new DefaultLabel ( title, subtitle );

			/// Define the location of the marker, it must be converted to base map coordinate system
			MapPos location = _mapView.Layers.BaseLayer.Projection.FromWgs84 ( longitude, latitude );

			/// define the marker style
			MarkerStyle markerStyle = DefineMarkerStyle ( Resource.Drawable.olmarker, 0.5f, 0f ).Build ();

			/// add the label to the Marker
			Marker marker = new Marker ( location, markerLabel, markerStyle, _markerLayer );

			/// add the label to the layer
			_markerLayer.Add ( marker );
		}

		/// <summary>
		/// Defaults value for the marker style.
		/// </summary>
		/// <returns>The marker style.</returns>
		private MarkerStyle.Builder DefaultMarkerStyle ()
		{
			return DefineMarkerStyle ( Resource.Drawable.point, 0.1f, 0f );
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
			markerStyleBuilder.SetColor ( Nutiteq.SDK.Color.White );
			markerStyleBuilder.SetSize ( size );
			markerStyleBuilder.SetOffset2DX ( offset2D );

			return markerStyleBuilder;
		}
	
		private LineStyle.Builder DefineLineStyle ( int lineResourceId, int pointResourceId, float lineWidth, float pointSize, int lineColor, int pointColor )
		{
			LineStyle.Builder lineStyleBuilder = new LineStyle.Builder ();
			Bitmap lineBitmap = UnscaledBitmapLoader.DecodeResource ( Resources, lineResourceId );
			lineStyleBuilder.SetBitmap ( lineBitmap );
			lineStyleBuilder.SetWidth ( lineWidth );
			lineStyleBuilder.SetColor ( lineColor );
			lineStyleBuilder.SetPointStyle ( DefinePointStyle ( pointResourceId, pointSize, pointColor ).Build() );
			return lineStyleBuilder;
		}

		private PointStyle.Builder DefinePointStyle ( int pointResourceId, float pointSize, int pointColor )
		{
			PointStyle.Builder pointStyle = new PointStyle.Builder ();
			Bitmap pointBitmap = UnscaledBitmapLoader.DecodeResource ( Resources, pointResourceId );
			pointStyle.SetBitmap ( pointBitmap );
			pointStyle.SetSize ( pointSize );
			pointStyle.SetColor ( pointColor );
			return pointStyle;
		}

		private TextStyle.Builder DefineTextStyle (int textSize, int textColor)
		{
			TextStyle.Builder textStyleBuilder = new TextStyle.Builder ();
			textStyleBuilder.SetColor ( textColor );
			textStyleBuilder.SetSize ( textSize );
			textStyleBuilder.SetFont ( Android.Graphics.Typeface.Monospace );
			textStyleBuilder.SetAllowOverlap ( true ); // if false, then SDK will not display the text if other texts/markers overlap with it
			textStyleBuilder.SetOrientation ( TextStyle.CameraBillboardOrientation );
			textStyleBuilder.SetOffset3DZ ( 0.0f );
			return textStyleBuilder;
		}

		/// <summary>
		/// Adds a line to the map.
		/// </summary>
		/// <param name="width">Width</param>
		/// <param name="color">Color</param> 
		/// <param name="points">Set of points</param>
		/// <param name="lineName">Line name.</param>
		private void AddLine ( MapPos[] points, float width, int color, string lineName )
		{
			LineStyle lineStyle = DefineLineStyle ( Resource.Drawable.point, Resource.Drawable.point, 0.1f, 0.2f, Nutiteq.SDK.Color.White, Nutiteq.SDK.Color.Blue ).Build();
			StyleSet<LineStyle> lineStyleSet = new StyleSet<LineStyle> ( );
			lineStyleSet.SetZoomStyle ( 2, lineStyle ); // start showing lines from zoom level 2

			Line line = new Line ( new List<MapPos> ( points ), new DefaultLabel ( lineName ), lineStyleSet, null );
			_linesLayer.Add ( line );
		}

		/// <summary>
		/// Adds a text to the map
		/// </summary>
		/// <param name="size">Size</param>
		/// <param name="color">Color</param> 
		/// <param name="text">Text to be shown</param>
		/// <param name="basePoint">Location of text</param>
		private void AddText ( MapPos basePoint, int size, int color, string textString )
		{
			TextStyle textStyle = DefineTextStyle (size, color).Build ();
			Text text = new Text (basePoint, textString, textStyle, null);
			_textLayer.Add ( text );
		}

		/// <summary>
		/// Adds the polygon.
		/// </summary>
		/// <param name="points">Points.</param>
		/// <param name="color">Color.</param>
		/// <param name="polygonName">Polygon name.</param>
		private void AddPolygon ( MapPos[] points, int color, string polygonName )
		{
			PolygonStyle.Builder polygonStyleBuilder = new PolygonStyle.Builder ();
			polygonStyleBuilder.SetColor ( color );

			Polygon polygon = new Polygon ( new List<MapPos> (points), new DefaultLabel ( polygonName ), polygonStyleBuilder.Build (), null );
			_polygonLayer.Add ( polygon );
		}
	}
}
