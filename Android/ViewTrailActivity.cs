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
	[Activity (Label = "Columbia583.Android_View_Trail", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class ViewTrailActivity : AndroidActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			SetPage (App.GetViewTrailPage ());

			SetContentView (Resource.Layout.ViewTrail);
		}
	}
}

