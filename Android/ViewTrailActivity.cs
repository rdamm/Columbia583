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
		protected TextView distance = null;
		protected TextView duration = null;
		protected TextView description = null;
		protected TextView directions = null;
		protected TextView difficultyRating = null;
		protected RatingBar rating = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			SetPage (App.GetViewTrailPage ());

			SetContentView (Resource.Layout.ViewTrail);

			// Get the controls.
			distance = FindViewById<TextView> (Resource.Id.distance);
			duration = FindViewById<TextView> (Resource.Id.duration);
			description = FindViewById<TextView> (Resource.Id.description);
			directions = FindViewById<TextView> (Resource.Id.directions);
			difficultyRating = FindViewById<TextView> (Resource.Id.difficultyRating);
			rating = FindViewById<RatingBar> (Resource.Id.rating);

			// Get trail data.
			string trailJSONStr = Intent.GetStringExtra ("viewedTrail") ?? "No trail displayed";
			Trail trail = Newtonsoft.Json.JsonConvert.DeserializeObject<Trail> (trailJSONStr);
			distance.Text = trail.Distance + " km";
			duration.Text = trail.Duration + " h";
			description.Text = trail.Description;
			directions.Text = trail.Directions;
			difficultyRating.Text = trail.Difficulty.ToString();
			//rating.NumStars = 5;
			rating.Rating = trail.Rating.RatingNumber;
		}
	}
}

