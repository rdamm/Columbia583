
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
	[Activity (MainLauncher = true, NoHistory = true)]
	public class SplashActivity : AndroidActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// If the database has been initialized, update it.  Otherwise, initialize it.
			Data_Access_Layer_Common dataAccessLayer = new Data_Access_Layer_Common ();
			Console.WriteLine ("Checking if database has been initialized.");
			if (dataAccessLayer.databaseInitialized () == true)
			{
				Console.WriteLine ("Updating database...");
				dataAccessLayer.updateDatabase ();
				Console.WriteLine ("Updated database.");
			}
			else
			{
				dataAccessLayer.initializeDatabase ();
			}
			Console.WriteLine ("Startup scripts completed.  Opening main activity.");
			
			StartActivity (typeof(MainMenuActivity));
		}
	}
}

