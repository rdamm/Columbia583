using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms.Platform.Android;

using Xamarin.Auth;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Columbia583.Android
{
	[Activity (Label = "Columbia583.Android_Main_Menu", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainMenuActivity : AndroidActivity
	{
		protected Button searchTrailsButton = null;
		protected Button debugAndTestsButton = null;
		protected Button loginButton = null;
		protected Button logoutButton = null;
		protected TextView loggedInUsersUsernameText = null;
		protected Button scanQrCodeButton = null;
		protected Button viewFavouriteTrailsButton = null;
		protected Button upload_trail = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.Main);

			// Get the controls.
			searchTrailsButton = FindViewById<Button> (Resource.Id.button_searchTrails);
			//debugAndTestsButton = FindViewById<Button> (Resource.Id.button_debugAndTests);
			loginButton = FindViewById<Button> (Resource.Id.button_login);
			logoutButton = FindViewById<Button> (Resource.Id.button_logout);
			loggedInUsersUsernameText = FindViewById<TextView> (Resource.Id.txtLoggedInUsersUsername);
			scanQrCodeButton = FindViewById<Button> (Resource.Id.button_scanQrCode);
			viewFavouriteTrailsButton = FindViewById<Button> (Resource.Id.button_viewFavouriteTrails);
			upload_trail = FindViewById<Button> (Resource.Id.uploadTrail);

			upload_trail.Enabled = false;
			Data_Access_Layer_Common data_access_layer = new Data_Access_Layer_Common();
			User getUser = data_access_layer.getActiveUser();
			if (getUser != null)
				upload_trail.Enabled = true;
			
			// Get the active user and show their username.
			Data_Access_Layer_Common dataAccessLayerCommon = new Data_Access_Layer_Common();
			User activeUser = dataAccessLayerCommon.getActiveUser ();
			if (activeUser != null)
			{
				// Set the active user.
				setActiveUser(activeUser);
			}
			else
			{
				// Clear the active user.
				clearActiveUser();
			}
			
			// Assign the event handlers.
			if (searchTrailsButton != null) {
				searchTrailsButton.Click += (sender, e) => {

					// Load the search trails page.
					var intent = new Intent(this, typeof(SearchTrailsPage));
					StartActivity(intent);

				};
			}if (upload_trail != null) {
				upload_trail.Click += (sender, e) => {

					// Load the search trails page.
					var intent = new Intent(this, typeof(UploadTrail));
					StartActivity(intent);


				};
			}
			if (viewFavouriteTrailsButton != null) {
				viewFavouriteTrailsButton.Click += (sender, e) => {

					// Load the view favourite trails page.
					var intent = new Intent(this, typeof(FavouriteTrailsActivity));
					StartActivity(intent);

				};
			}
//			if (debugAndTestsButton != null) {
//				debugAndTestsButton.Click += (sender, e) => {
//
//					// Load the debug and tests page.
//					var intent = new Intent(this, typeof(DebugMenuActivity));
//					StartActivity(intent);
//
//				};
//			}
			if (loginButton != null) {
				loginButton.Click += requestLoginEvent;
			}
			if (logoutButton != null) {
				logoutButton.Click += logoutEvent;
			}
			if (scanQrCodeButton != null) {
				scanQrCodeButton.Click += ScanQrCodeEvent;
			}
		}


		/// <summary>
		/// Requests a Facebook login page, and calls the response login event after it completes.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void requestLoginEvent(object sender, EventArgs eventArgs)
		{
			// If the network is available, continue with login.
			if (NetworkHelper.networkAvailable (this) == true)
			{
				// Create an OAuth authentication request.  Add email to the scope to include it in the account request permissions.
				var auth = new OAuth2Authenticator (
					           clientId: "370094413194090",
					           scope: "email",
					           authorizeUrl: new Uri ("https://m.facebook.com/dialog/oauth/"),
					           redirectUrl: new Uri ("http://www.facebook.com/connect/login_success.html"));

				// Open the login screen.
				StartActivity (auth.GetUI (this));

				// Once the login screen has completed, run this event.
				auth.Completed += responseLoginEvent;
			}
			else
			{
				Toast.MakeText (this, "Network unavailable.  Cannot log in.", ToastLength.Short).Show();
			}
		}


		/// <summary>
		/// Called when the login request returns to the application.  This will store the account and
		/// display its username if it is authenticated, and destroy the account and clear the username
		/// otherwise.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void responseLoginEvent(object sender, AuthenticatorCompletedEventArgs eventArgs)
		{
			// Get the account from the login response arguments.
			Account account = eventArgs.Account;

			// Clear the active user.
			clearActiveUser();

			// If the user was logged in successfully, get their account info.
			if (eventArgs.IsAuthenticated)
			{
				if (account != null)
				{
					// If the network is available, continue with login.
					if (NetworkHelper.networkAvailable (this) == true)
					{
						// Get the user's Facebook profile info.
						var request = new OAuth2Request ("GET", new Uri ("https://graph.facebook.com/me"), null, account);
						request.GetResponseAsync ().ContinueWith (t => {
							// If the response is invalid, log the error.
							if (t.IsFaulted)
							{
								Console.WriteLine ("Error: " + t.Exception.InnerException.Message);
							}
							else
							{
								// Get the user's info from the response.
								string json = t.Result.GetResponseText ();
								OAuthUser userProfile = JsonConvert.DeserializeObject<OAuthUser> (json);

								// Get the user's email from the profile.
								string userEmail = userProfile.email;

								// Get the user from the local users.  If the user doesn't exist, create it.
								Data_Access_Layer_Common dataAccessLayerCommon = new Data_Access_Layer_Common ();
								User user = dataAccessLayerCommon.getUserByEmail (userEmail);
								if (user == null)
								{
									// Create the user.
									user = new User (0, 0, userEmail, userProfile.name, DateTime.Now, DateTime.Now, true);
									Data_Access_Layer_Upload dataAccessLayerUpload = new Data_Access_Layer_Upload ();
									dataAccessLayerUpload.uploadUser (user);
								}

								// Set the active user.
								setActiveUser (user);
							}
						});
					}
					else
					{
						Toast.MakeText (this, "Network unavailable.  Cannot log in.", ToastLength.Short).Show();
					}
				}
			}
		}


		/// <summary>
		/// Logs out the user.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void logoutEvent(object sender, EventArgs eventArgs)
		{
			// Clear the active user.
			clearActiveUser();
		}


		/// <summary>
		/// Sets the active user.
		/// </summary>
		/// <param name="userId">User identifier.</param>
		protected void setActiveUser(User user)
		{
			// Set the active user.
			Data_Access_Layer_Common dataAccessLayerCommon = new Data_Access_Layer_Common();
			dataAccessLayerCommon.setActiveUser (user.id);

			// DEBUG: Display the active user.
			User activeUser = dataAccessLayerCommon.getActiveUser();

			// Change the username view.
			RunOnUiThread (() => loggedInUsersUsernameText.Text = "Logged in as " + user.username + ".");

			// Show the login button.
			RunOnUiThread (() => loginButton.Visibility = ViewStates.Invisible);
			RunOnUiThread (() => logoutButton.Visibility = ViewStates.Visible);
		}


		/// <summary>
		/// Clears the active user.
		/// </summary>
		protected void clearActiveUser()
		{
			// Clear the active user.
			Data_Access_Layer_Common dataAccessLayerCommon = new Data_Access_Layer_Common();
			dataAccessLayerCommon.setActiveUser (0);

			// DEBUG: Display the active user.
			User activeUser = dataAccessLayerCommon.getActiveUser();

			// Change the username view.
			RunOnUiThread (() => loggedInUsersUsernameText.Text = "Not logged in.");

			// Show the login button.
			RunOnUiThread (() => loginButton.Visibility = ViewStates.Visible);
			RunOnUiThread (() => logoutButton.Visibility = ViewStates.Invisible);
		}


		/// <summary>
		/// Scans the QR code of a trail and opens that trail's page if it is a valid QR code.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		async protected void ScanQrCodeEvent(object sender, EventArgs eventArgs)
		{
			// Scan the QR code.
			var scanner = new ZXing.Mobile.MobileBarcodeScanner(this);
			var result = await scanner.Scan();

			// If a result was found, determine the trail ID from it and open the trail.
			if (result != null)
			{
				// DEBUG: Display the raw barcode.
				Console.WriteLine ("Scanned Barcode: " + result.Text);

				// Get the barcode's text from the result.
				string barcodeText = result.Text;

				// Check if the barcode text follows the expected format: http://trails.greenways.ca/trails/[TRAIL_ID]
				if (barcodeText.Length <= 34)
				{
					Toast.MakeText (this, "Invalid QR code detected.", ToastLength.Long).Show();
					return;
				}
				string barcodeDomain = barcodeText.Substring(0, 34);
				if (barcodeDomain != "http://trails.greenways.ca/trails/") {
					Toast.MakeText (this, "Invalid QR code detected.", ToastLength.Long).Show();
					return;
				}

				// Pull the ID from the barcode text.
				int trailId = 0;
				string trailIdString = barcodeText.Substring (34, barcodeText.Length - 34);
				try
				{
					trailId = Int32.Parse(trailIdString);
				}
				catch (Exception e)
				{
					Toast.MakeText (this, "Invalid QR code detected.", ToastLength.Long).Show();
					return;
				}

				// Load the trail details.
				Data_Access_Layer_View_Trail dataAccessLayerViewTrail = new Data_Access_Layer_View_Trail ();
				Trail trail = dataAccessLayerViewTrail.getTrail (trailId);
				if (trail == null)
				{
					Toast.MakeText (this, "Trail not found.", ToastLength.Long).Show();
					return;
				}
				Activity[] activities = dataAccessLayerViewTrail.getActivities (trailId);
				Amenity[] amenities = dataAccessLayerViewTrail.getAmenities (trailId);
				Point[] points = dataAccessLayerViewTrail.getPoints (trailId);

				// Load the trail's page.
				var intent = new Intent (this, typeof(ViewTrailActivity));
				string trailJSONStr = Newtonsoft.Json.JsonConvert.SerializeObject (trail);
				string activitiesJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(activities);
				string amenitiesJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(amenities);
				string pointsJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(points);
				intent.PutExtra ("viewedTrail", trailJSONStr);
				intent.PutExtra("activities", activitiesJSONstr);
				intent.PutExtra("amenities", amenitiesJSONstr);
				intent.PutExtra("points", pointsJSONstr);
				StartActivity (intent);
			}
		}
	}
}