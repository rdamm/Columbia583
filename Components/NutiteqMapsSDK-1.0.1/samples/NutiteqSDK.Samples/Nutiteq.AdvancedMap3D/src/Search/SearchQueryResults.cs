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
using Com.Nutiteq.NGeometry;
using Com.Nutiteq.NLog;
using Android.Provider;
using Com.Nutiteq.Advancedmap.Mapquest;
using Org.Json;
using Com.Nutiteq.NProjections;
using Com.Nutiteq.NUtils;
using Android.Graphics;
using Com.Nutiteq.UI;
using Com.Nutiteq.NStyle;

namespace Nutiteq.AdvancedMap3D
{
	public class SearchQueryResults : Activity
	{
		private const int SEARCH_DIALOG = 1;

		//	private static String MAPQUEST_KEY = "Fmjtd%7Cluub2qu82q%2C70%3Do5-961w1w";

		// UI elements
		private ProgressDialog progressDialog;
		private Marker[] searchResultPlaces;

		private Dictionary<String, String> list;

		/** Called with the activity is first created.
	    * 
	    *  After the typical activity setup code, we check to see if we were launched
	    *  with the ACTION_SEARCH intent, and if so, we handle it.
	    */

		protected override void OnCreate (Bundle savedInstanceState)
		{
			Log.Debug("onCreate search");
			base.OnCreate (savedInstanceState);
			list = new Dictionary<string, string> ();

			// get and process search query here
			Intent queryIntent = Intent;
			String queryAction = queryIntent.Action;
			if (Intent.ActionSearch.Equals(queryAction)) {
				doSearchQuery(queryIntent, "onCreate()");
			}

		}

//		protected override void OnListItemClick (ListView l, View v, int position, long id)
//		{
//			base.OnListItemClick (l, v, position, id);
//			Log.Debug("Clicked: " + position );
//			AddressSearchActivity.SetSearchResult(searchResultPlaces[position]);
//			Finish();
//		}

		/*		* 
	     * Called when new intent is delivered.
	     *
	     * This is where we check the incoming intent for a query string.
	     * 
	     * @param newIntent The intent used to restart this activity
	     */
		protected override void OnNewIntent (Intent intent)
		{

			Log.Debug("onNewIntent search");

			// get and process search query here
			Intent queryIntent = Intent;
			String queryAction = queryIntent.Action;
			if (Intent.ActionSearch.Equals(queryAction)) {
				doSearchQuery(queryIntent, "onNewIntent()");
			}
			base.OnNewIntent (intent);
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			Log.Debug("onStop search");
			progressDialog.Dismiss();
		}

		private void doSearchQuery(Intent queryIntent, String entryPoint) {

			// The search query is provided as an "extra" string in the query intent
			String queryString = queryIntent.GetStringExtra(SearchManager.Query);

			// Record the query string in the recent queries suggestions provider.
			SearchRecentSuggestions suggestions = new SearchRecentSuggestions(this, 
			SearchSuggestionProvider.AUTHORITY, SearchSuggestionProvider.MODE);
			suggestions.SaveRecentQuery(queryString, null);


			// Do the actual search, write to searchResults field
			ShowDialog(SEARCH_DIALOG);

			//	MapQuestGeocoder geocoder = new MapQuestGeocoder();
			//geocoder.geocode(queryString, null, this, MAPQUEST_KEY);
		}

//		// handler to send search results to UI thread
//		Handler handler = handleMessage(msg);
//
//		 new Handler() {
//			public void handleMessage(Message msg) {
//				SimpleAdapter listAdapter = new SimpleAdapter( 
//					SearchQueryResults.this, 
//					list,
//					R.layout.searchrow,
//					new String[] { "line1","line2" },
//					new int[] { R.id.text1, R.id.text2 }  );
//				setListAdapter(listAdapter);
//				getListView().setTextFilterEnabled(true);
//			}
//		};
//
//		 Handler errorHandler = new Handler() {
//			public void handleMessage(Message msg) {
//				Toast.makeText(SearchQueryResults.this, "Nothing found", Toast.LENGTH_LONG).show();
//				finish();
//			}
//		};

		public void searchResults(JSONArray locations) {

			if(locations == null || locations.Length() == 0){
				Log.Debug("no results found");
//				Message msg = ErrorHandler.obtainMessage();
//				errorHandler.sendMessage(msg);
				return;
			}

			Log.Debug("geocode results: "+locations.Length());
			progressDialog.Dismiss();
			searchResultPlaces = new Marker[locations.Length()];

			Projection proj = new EPSG3857();
			//Bitmap pointMarker = UnscaledBitmapLoader.DecodeResource(Resources, Resource.Drawable.olmarker);
			MarkerStyle markerStyle = DefineMarkerStyle (Resource.Drawable.olmarker, 0.001f, 0f).Build();


			for (int i=0;i<locations.Length();i++){

				try {
					JSONObject location = locations.GetJSONObject(i);

					String street = location.OptString("street");
					String city = location.OptString("adminArea5"); // city
					String county = location.OptString("adminArea4"); // county
					String state = location.OptString("adminArea3"); // state
					String country = location.OptString("adminArea1"); // country

					String line1 = notNull(street) + " " + city;
					String line2 = notNull(county) + " "+ notNull(state) + " " + notNull(country);

//					Dictionary<String,String> item = new Dictionary<String,String>();
//					item.Add("line1",);
//					item.Add("line2",line2);


					list.Add(line1,line2);

					Label label = new DefaultLabel(line1,line2);
					JSONObject latLng = location.GetJSONObject("latLng");
					Double lng = latLng.GetDouble("lng");
					Double lat = latLng.GetDouble("lat");
				
					Marker marker = new Marker(proj.FromWgs84(lng, lat), label, markerStyle, location);
					searchResultPlaces[i] = marker;

				} catch (JSONException e) {
					// TODO Auto-generated catch block
					e.PrintStackTrace();
				}

			}        

//			Message msg = handler.obtainMessage();
//			handler.sendMessage(msg);
		}

		/// <summary>
		/// Define marker style (image, size, color)
		/// </summary>
		/// <returns>The marker style.</returns>
		private MarkerStyle.Builder DefineMarkerStyle(int resourceId, float size,float offset2D )
		{
			// define marker style 
			Bitmap pointMarker = UnscaledBitmapLoader.DecodeResource(Resources, resourceId);

			MarkerStyle.Builder markerStyleBuilder = new MarkerStyle.Builder ();
			markerStyleBuilder.SetBitmap (pointMarker);
			markerStyleBuilder.SetColor (Com.Nutiteq.NComponents.Color.White);
			markerStyleBuilder.SetSize (size);
			markerStyleBuilder.SetOffset2DX (offset2D);
			return markerStyleBuilder;
		}


		protected override Dialog OnCreateDialog (int id)
		{
			switch (id) {
			case SEARCH_DIALOG:
				progressDialog = new ProgressDialog(this);
				progressDialog.SetTitle("Searching...");
				progressDialog.SetCancelable(true);
				return progressDialog;
			default:
				return null;
			}

		}



		private String notNull(String txt) {
			if(txt == null)
				return "";
			else
				return txt;
		}
	}
}

