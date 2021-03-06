﻿
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
			// NOTE: The UI doesn't load until this entire method has completed.  So, the initialization must be threaded.
			// TODO: Fix bug where rotating the screen during the splash screen's loading creates multiple threads.
			new Thread (new ThreadStart (() => {
				// Define the number of tasks to complete for the progress bar.
				int completedTasks = 0;
				int totalTasks = 1;

				// Check if WiFi is available.
				if (NetworkHelper.wifiAvailable(this) == true)
				{
					// If the database has not been initialized, initialize it.
					Data_Access_Layer_Common dataAccessLayer = new Data_Access_Layer_Common ();
					bool databaseInitialized = dataAccessLayer.databaseInitialized ();
					if (databaseInitialized != true)
					{
							dataAccessLayer.initializeDatabase ();
							dataAccessLayer.initializeComments();
							Console.WriteLine("Splash screen has initialized the database.");
					}
				}
				completedTasks++;

				// Update the progress bar.
				RunOnUiThread(() => {
					splashProgressBar.Progress = ((int)((float)completedTasks / (float)totalTasks * 100));
					splashProgressLabel.Text = "Database initialized.";
				});

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

