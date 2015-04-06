using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Org.Json;
using Nutiteq.SDK;

namespace Nutiteq.AdvancedMap3D
{
	public class MapQuestGeocoder
	{
		private static List<String> _urls { get; set; }
		private static AddressSearchActivity _callback { get; set;}

		public void Geocode ( String request, Envelope bbox, AddressSearchActivity callback, String apiKey )
		{
			UriBuilder uri = new UriBuilder ( "http://open.mapquestapi.com/geocoding/v1/address?location=" + request );

			if ( bbox != null )
			{
				String boundingBox = bbox.MinY + "," + bbox.MinX + "," + bbox.MaxY + "," + bbox.MaxX;
				uri.Path = uri.Path + "&boundingBox=" + boundingBox;
			}

			String url = uri.ToString ();

			if ( apiKey != null )
			{
				url += "&key=" + apiKey;
			}

			Log.Debug ( "geocode url: "+ uri.ToString () );

			if ( _urls == null) 
			{
				_urls = new List<string> ();
			}
			_urls.Insert ( 0, url );
			new MqGeocodeTask ( callback ).Execute ( url );
		}

		public class MqGeocodeTask : AsyncTask<String, Object, JSONArray> 
		{
			#region implemented abstract members of AsyncTask

			protected override JSONArray RunInBackground ( params string[] @params )
			{
				String json = DownloadUrl ( _urls[0], 2000 );
				Console.WriteLine ( _urls [0] );
				Console.WriteLine ( json );
				try 
				{
					JSONObject jObj = new JSONObject ( json );
					JSONArray locations = jObj.GetJSONArray ( "results" ).GetJSONObject ( 0 ).GetJSONArray ( "locations" );
					return locations;
				} 
				catch ( JSONException e ) 
				{
					Log.Error ( "Error parsing JSON data " + e.ToString () );
				}

				return null;
			}

			#endregion

			public string DownloadUrl ( string url, int timeout )
			{
				StringBuilder stringResponse = new StringBuilder ( "{}" );
				try
				{
					//Set up the web request
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create ( url );
					request.Method = "GET";
					request.Timeout = timeout;

					///receive the response
					HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
					Stream responseStream = response.GetResponseStream ();
					Encoding enc = System.Text.Encoding.UTF8;
					StreamReader responseStreamReader = new StreamReader ( responseStream, enc );
					string responseString = responseStreamReader.ReadToEnd ();
					responseStreamReader.Close ();
					response.Close ();
					Console.WriteLine ( responseString );

					return responseString;
				}
				catch ( Exception genericException )
				{
					stringResponse = new StringBuilder ( "{\"error\":\"generic error:" );
					stringResponse.Append ( genericException.Message );
					stringResponse.Append ( "\"}" );
				}

				return stringResponse.ToString ();
			}

			public MqGeocodeTask ( AddressSearchActivity callback )
			{
				_callback = callback;
			}


			protected override void OnPostExecute ( JSONArray result )
			{
				_callback.SearchResults ( result );
			}
		}
	}
}


