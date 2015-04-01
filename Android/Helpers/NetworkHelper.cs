using System;
using Android.Net;
using Xamarin.Forms.Platform.Android;

namespace Columbia583.Android
{
	public class NetworkHelper
	{
		public NetworkHelper ()
		{
			
		}


		/// <summary>
		/// Checks if any network connection is available.
		/// </summary>
		/// <returns><c>true</c>, if network is available, <c>false</c> otherwise.</returns>
		public static bool networkAvailable(AndroidActivity activity)
		{
			// Check if any network connection is available.
			bool connectedToNetwork = false;
			var connectivityManager = (ConnectivityManager)activity.GetSystemService(global::Android.Content.ContextWrapper.ConnectivityService);
			var activeConnection = connectivityManager.ActiveNetworkInfo;
			if ((activeConnection != null) && activeConnection.IsConnected)
			{
				connectedToNetwork = true;
			}

			return connectedToNetwork;
		}


		/// <summary>
		/// Checks if a WiFi connection is available.
		/// </summary>
		/// <returns><c>true</c>, if WiFi available, <c>false</c> otherwise.</returns>
		public static bool wifiAvailable(AndroidActivity activity)
		{
			// Check if the network is available via WiFi.
			bool connectedToWifi = false;
			if (networkAvailable(activity) == true)
			{
				var connectivityManager = (ConnectivityManager)activity.GetSystemService(global::Android.Content.ContextWrapper.ConnectivityService);
				var wifiState = connectivityManager.GetNetworkInfo(ConnectivityType.Wifi).GetState();
				if (wifiState == NetworkInfo.State.Connected)
				{
					connectedToWifi = true;
				}
			}

			return connectedToWifi;
		}
	}
}

