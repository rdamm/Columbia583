
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

using Xamarin.Forms.Platform.Android;

using System.Threading;
using System.Threading.Tasks;
using Android.Content.PM;

namespace Columbia583.Android
{
	[Activity (Label = "OptionsMenuActivity", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]			
	public class OptionsMenuActivity : AndroidActivity
	{
		protected Button btnUpdateDatabaseNow = null;
		protected Button btnResetDatabase = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.OptionsMenu);

			// Get the controls.
			btnUpdateDatabaseNow = FindViewById<Button> (Resource.Id.btnUpdateDatabaseNow);
			btnResetDatabase = FindViewById<Button> (Resource.Id.btnResetDatabase);

			// Set the event handlers.
			if (btnUpdateDatabaseNow != null) {
				btnUpdateDatabaseNow.Click += updateDatabaseEvent;
			}
			if (btnResetDatabase != null) {
				btnResetDatabase.Click += resetDatabaseEvent;
			}
		}


		/// <summary>
		/// This event updates the database.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void updateDatabaseEvent(object sender, EventArgs eventArgs)
		{
			// Check if WiFi is available.
			if (NetworkHelper.wifiAvailable (this) == true)
			{
				// Create a background thread to update the database.
				new Thread (new ThreadStart (() => {

					// Inform the user that the database is updating.
					RunOnUiThread(() => {
						Toast.MakeText(this, "Updating database...", ToastLength.Short).Show();
						btnUpdateDatabaseNow.Enabled = false;
					});

					// Update the database.
					Data_Access_Layer_Common dataAccessLayerCommon = new Data_Access_Layer_Common();
					dataAccessLayerCommon.updateDatabase();

					// Inform the user that the database has updated.
					RunOnUiThread(() => {
						Toast.MakeText(this, "Database updated.", ToastLength.Short).Show();
						btnUpdateDatabaseNow.Enabled = true;
					});

				})).Start ();
			}
			else
			{
				RunOnUiThread(() => Toast.MakeText(this, "WiFi unavailable.  Cannot update the database.", ToastLength.Short).Show());
			}
		}


		/// <summary>
		/// This event initializes the database.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void resetDatabaseEvent(object sender, EventArgs eventArgs)
		{
			// Check if WiFi is available.
			if (NetworkHelper.wifiAvailable (this) == true)
			{
				// Create a background thread to initialize the database.
				new Thread (new ThreadStart (() => {

					// Inform the user that the database is initializing.
					RunOnUiThread(() => {
						Toast.MakeText(this, "Resetting database...", ToastLength.Short).Show();
						btnResetDatabase.Enabled = false;
					});

					// Initialize the database.
					Data_Access_Layer_Common dataAccessLayerCommon = new Data_Access_Layer_Common();
					dataAccessLayerCommon.initializeDatabase();

					// Inform the user that the database has initialized.
					RunOnUiThread(() => {
						Toast.MakeText(this, "Database reset.", ToastLength.Short).Show();
						btnResetDatabase.Enabled = true;
					});

				})).Start ();
			}
			else
			{
				RunOnUiThread(() => Toast.MakeText(this, "WiFi unavailable.  Cannot reset the database.", ToastLength.Short).Show());
			}
		}
	}
}

