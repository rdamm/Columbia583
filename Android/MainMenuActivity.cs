﻿using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms.Platform.Android;


namespace Columbia583.Android
{
	[Activity (Label = "Columbia583.Android_Main_Menu", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainMenuActivity : AndroidActivity
	{
		protected Button searchTrailsButton = null;
		protected Button testDatabaseButton = null;
		protected Button testDataAccessButton = null;
		protected Button testImageButton = null;

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
			testDatabaseButton = FindViewById<Button> (Resource.Id.button_databaseTests);
			testDataAccessButton = FindViewById<Button> (Resource.Id.button_dataAccessTests);
			testImageButton = FindViewById<Button> (Resource.Id.button_imageTests);

			// Assign an event handler to the button.
			if (searchTrailsButton != null) {
				searchTrailsButton.Click += (sender, e) => {

					// Load the search trails page.
					var intent = new Intent(this, typeof(SearchTrailsActivity));
					StartActivity(intent);

				};
			}
			if (testDatabaseButton != null) {
				testDatabaseButton.Click += (sender, e) => {

					// Load the database tests page.
					var intent = new Intent(this, typeof(TestDatabaseActivity));
					StartActivity(intent);

				};
			}
			if (testDataAccessButton != null) {
				testDataAccessButton.Click += (sender, e) => {

					// Load the data access tests page.
					var intent = new Intent(this, typeof(TestDataAccessActivity));
					StartActivity(intent);

				};
			}
			if (testImageButton != null) {
				testImageButton.Click += (sender, e) => {

					// Load the image tests page.
					var intent = new Intent(this, typeof(TestImageActivity));
					StartActivity(intent);

				};
			}
		}
	}
}