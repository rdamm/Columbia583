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
		protected TextView directions = null;
		protected RatingBar difficultyRank = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			SetPage (App.GetViewTrailPage ());

			SetContentView (Resource.Layout.ViewTrail);

			// Get the controls.
			difficultyRank = FindViewById<RatingBar> (Resource.Id.difficultyRating);
			directions = FindViewById<TextView> (Resource.Id.directions);

			// Get trail data.
			string trailJSONStr = Intent.GetStringExtra ("viewedTrail") ?? "No trail displayed";
			Trail trail = Newtonsoft.Json.JsonConvert.DeserializeObject<Trail> (trailJSONStr);
			directions.Text = trail.Directions;
		}
	}
}

