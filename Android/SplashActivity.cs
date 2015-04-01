
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
using Android.Net;

namespace Columbia583.Android.hi
{
	[Activity (MainLauncher = true, NoHistory = true, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
	public class SplashActivity : AndroidActivity
	{
		ProgressBar splashProgressBar = null;
		TextView splashProgressLabel = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.SplashScreen);

			// Get the controls.
			splashProgressBar = FindViewById<ProgressBar> (Resource.Id.prgSplashProgress);
			splashProgressLabel = FindViewById<TextView> (Resource.Id.txtSplashProgressLabel);

			// Create a background thread for the app initialization.
			// NOTE: The UI doesn't load until this entire method has completed.  So, the initialization must go into a background thread.
			// TODO: Fix bug where rotating the screen during the splash screen's loading creates multiple threads.
			new Thread (new ThreadStart (() => {
				// Define the number of tasks to complete for the progress bar.
				int completedTasks = 0;
				int totalTasks = 1;

				// Check if the network is available via WiFi.  We don't want network-dependent methods to run
				// without an internet connection, and we don't want bandwidth-heavy methods to run if only the
				// cellular network is available.
				bool connectedToWifi = NetworkHelper.wifiAvailable(this);

				// If WiFi is available, run the database initialization.
				if (connectedToWifi == true)
				{
					// Check if the database has been initialized.
					Data_Access_Layer_Common dataAccessLayer = new Data_Access_Layer_Common ();
					bool databaseInitialized = dataAccessLayer.databaseInitialized ();

					// Initialize / update the local database.
					if (databaseInitialized == true)
					{
						dataAccessLayer.updateDatabase ();
						Console.WriteLine("Splash screen has updated the database.");
					}
					else
					{
						dataAccessLayer.initializeDatabase ();
						dataAccessLayer.initializeComments();
						Console.WriteLine("Splash screen has initialized the database.");
					}
					completedTasks++;
					Console.WriteLine("Database updated in splash activity initializer.");

					// Update the progress bar.
					RunOnUiThread(() => {
						splashProgressBar.Progress = ((int)((float)completedTasks / (float)totalTasks * 100));
						splashProgressLabel.Text = "Database updated.";
					});
				}
				else
				{
					completedTasks++;
					Console.WriteLine("WiFi unavailable.  Skipping database update.");

					// Update the progress bar.
					RunOnUiThread(() => {
						splashProgressBar.Progress = ((int)((float)completedTasks / (float)totalTasks * 100));
						splashProgressLabel.Text = "WiFi unavailable.  Database update skipped.";
					});
				}

				// Load the main menu.
				RunOnUiThread(() => {
					splashProgressLabel.Text = "Starting app...";
					Console.WriteLine("Starting main menu activity.");
					StartActivity (typeof(MainMenuActivity));
				});

			})).Start ();

			Console.WriteLine ("Reached the end of splash activity main function.");
		}
	}
}

