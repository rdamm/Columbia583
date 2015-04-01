
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
	[Activity (Label = "TestDataAccessActivity", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]			
	public class TestDataAccessActivity : AndroidActivity
	{
		protected Button initializeDatabaseButton = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.TestDataAccess);

			// Get the controls.
			initializeDatabaseButton = FindViewById<Button> (Resource.Id.btnInitializeDatabase);

			// Assign event handlers to the buttons.
			if (initializeDatabaseButton != null) {
				initializeDatabaseButton.Click += (sender, e) => {
					Data_Access_Layer_Common dataAccessLayer = new Data_Access_Layer_Common();
					dataAccessLayer.initializeDatabase();
				};
			}
		}
	}
}

