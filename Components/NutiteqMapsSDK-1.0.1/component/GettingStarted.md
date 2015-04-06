This is how you add MapView with a pin to your application:

1) **Add MapView to your application main layout**

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="fill_parent"
    android:layout_height="fill_parent"
    android:orientation="vertical" >
   <com.nutiteq.MapView
    android:id="@+id/mapView"
    android:layout_width="fill_parent" 
    android:layout_height="fill_parent" 
    />
</LinearLayout>
```

2) **Create MapView object.** 

Usually you have the MapView as member in your main activity class, load layout and load the MapView from layout. MapView also need to initialize and start map. Three steps are needed here as minimum: 

* Define map **Configuration** package, which is put in Components object, 
* Define **base map layer** - source of background map
* Tell map object to **start map** activities (downloading threads etc), this is best to be done in Activity lifetime functions onStart and onStop.

```csharp
using Nutiteq.SDK;

[Activity (Label = "Nutiteq.HelloMap", MainLauncher = true)]
public class MainActivity : Activity
{
    /// <summary>
	/// The Nutiteq MapView
	/// </summary>
	private MapView _mapView;
	protected override void OnCreate ( Bundle bundle )
	{
		base.OnCreate ( bundle );

		/// Set our view from the "main" layout resource
		SetContentView ( Resource.Layout.Main );
	
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
```

3) **Add a marker** to map, to given coordinates. Pin will have pop-up Label which opens when you click on it.

```csharp

	/// create layer and add object to the layer, finally add layer to the map. 
	/// All overlay layers must be same projection as base layer, so we reuse it
	_markerLayer = new MarkerLayer ( _mapView.Layers.BaseLayer.Projection );
	_mapView.Layers.AddLayer ( _markerLayer );

	/// Define label what is shown when you click on marker.
	Label markerLabel = new DefaultLabel ( "Marker", "San Francisco" );

	/// Define the location of the marker, it must be converted to EPSG3857 coordinate system
	MapPos mapLocation = _markerLayer.Projection.FromWgs84 ( -122.41666666667, 37.76666666666 );

	/// define marker style.
	/// Copy olmarker.png from sample project to your Resources/drawable folder
	Bitmap pointMarker = UnscaledBitmapLoader.DecodeResource ( Resources, Resource.Drawable.olmarker );
	MarkerStyle.Builder markerStyleBuilder = new MarkerStyle.Builder ();
	markerStyleBuilder.SetBitmap ( pointMarker );
	markerStyleBuilder.SetColor ( Nutiteq.SDK.Color.White );
	markerStyleBuilder.SetSize ( 0.5f );
	MarkerStyle markerStyle = markerStyleBuilder . Build ();

	/// add the label to the Marker
	Marker marker = new Marker ( mapLocation, markerLabel, markerStyle, null );

	/// add the label to the layer
	_markerLayer.Add ( marker );

	/// center Map View at SF, set zoom
	_mapView.FocusPoint = _mapView.Layers.BaseLayer.Projection.FromWgs84 ( -122.41666666667, 37.76666666666 );
	_mapView.Zoom = 10f;

```
4) **Add license registration**

Free installation package will have "Nutiteq evaluation" logo on map. Adding license code removes the logo and enables to replace with own one. Please email sales@nutiteq.com to purchase license code.

```csharp
    // register license. Not needed for development/evaluation
	bool Res = MapView.RegisterLicense ("<ENTER YOUR LICENSEKEY HERE>", ApplicationContext);
	Log.Debug ("license validity: " + Res);

	// Set watermark bitmap - only available for commercial licenses, replaces Nutiteq Evaluation watermark. -1, -1 are relative screen coordinates, 0.2 is watemark size relative to screen
	MapView.SetWatermark (UnscaledBitmapLoader.DecodeResource (Resources, Resource.Drawable.Icon), -1.0f, -1.0f, 0.2f);
```

5) **Enhance your app.** Next please take a look to included HelloMap and AdvancedMap samples, these provide you a lot of useful code snippets. 

## Help and support

For API documentation click "Open API Documentation" in component page, this will open docs in Mono Documentation Browser. 

For advanced features [Nutiteq Maps SDK wiki documentation](https://github.com/nutiteq/hellomap3d/wiki) could be helpful. The developer docs and guides have  Android Java API samples, but same functions are available via Xamarin component and C# API. If you need help to implement these please post to the community forum.

**Support channels:**

* For all developers: community support and discussions: email to nutiteq-dev@googlegroups.com or use [nutiteq-dev web forum](https://groups.google.com/forum/#!forum/nutiteq-dev)
* For paid customers we provide email support. Please contact sales@nutiteq.com to arrange the service
