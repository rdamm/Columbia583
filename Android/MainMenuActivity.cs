using System;

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
	[Activity (Label = "Columbia583.Android_Main_Menu", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainMenuActivity : AndroidActivity
	{
		protected Button searchTrailsButton = null;
		protected Button debugAndTestsButton = null;

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
			debugAndTestsButton = FindViewById<Button> (Resource.Id.button_debugAndTests);

			// Assign the event handlers.
			if (searchTrailsButton != null) {
				searchTrailsButton.Click += (sender, e) => {

					// Load the search trails page.
					var intent = new Intent(this, typeof(SearchTrailsActivity));
					StartActivity(intent);

				};
			}
			if (debugAndTestsButton != null) {
				debugAndTestsButton.Click += (sender, e) => {

					// Load the debug and tests page.
					var intent = new Intent(this, typeof(DebugMenuActivity));
					StartActivity(intent);

				};
			}
		}
	}
}