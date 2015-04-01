
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

namespace Columbia583.Android
{
	[Activity (Label = "DebugMenuActivity", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
	public class DebugMenuActivity : AndroidActivity
	{
		protected Button testDatabaseButton = null;
		protected Button testDataAccessButton = null;
		protected Button testImageButton = null;
		protected Button testRecordTrailButton = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.DebugMenu);

			// Get the controls.
			testDatabaseButton = FindViewById<Button> (Resource.Id.button_databaseTests);
			testDataAccessButton = FindViewById<Button> (Resource.Id.button_dataAccessTests);
			testImageButton = FindViewById<Button> (Resource.Id.button_imageTests);
			testRecordTrailButton = FindViewById<Button> (Resource.Id.button_recordTrailTests);

			// Assign the event handlers.
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
			if (testRecordTrailButton != null) {
				testRecordTrailButton.Click += (sender, e) => {

					// Load the record trail page.
					var intent = new Intent(this, typeof(RecordTrailActivity));
					StartActivity(intent);

				};
			}
		}
	}
}

