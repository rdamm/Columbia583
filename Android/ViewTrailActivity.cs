using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Support.V4.View;
using Android.Support.V4.App;


namespace Columbia583.Android
{
	[Activity (Label = "Columbia583.Android_View_Trail", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class ViewTrailActivity : FragmentActivity
	{
		protected const int NUM_PAGES = 2;

		protected ViewPager pager = null;
		protected MyFragmentPagerAdapter adapter = null;
		//protected ViewSwitcher viewSwitcher = null;
		protected LinearLayout layout1 = null;
		protected TextView distance = null;
		protected TextView duration = null;
		protected TextView description = null;
		protected TextView directions = null;
		protected TextView difficultyRating = null;
		protected RatingBar rating = null;
		//private GestureDetector _gestureDetector;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			//SetPage (App.GetViewTrailPage ());
			SetContentView (Resource.Layout.ViewTrailBase);

			pager = FindViewById<ViewPager> (Resource.Id.viewpager);
			adapter = new MyFragmentPagerAdapter (SupportFragmentManager);

			adapter.AddFragmentView((i, v, b) =>
				{
					var view = LayoutInflater.Inflate(Resource.Layout.ViewTrail, v, false);
					// Get the controls.
					layout1 = view.FindViewById<LinearLayout> (Resource.Id.layout1);

					distance = view.FindViewById<TextView> (Resource.Id.distance);
					duration = view.FindViewById<TextView> (Resource.Id.duration);
					description = view.FindViewById<TextView> (Resource.Id.description);
					directions = view.FindViewById<TextView> (Resource.Id.directions);
					difficultyRating = view.FindViewById<TextView> (Resource.Id.difficultyRating);
					rating = view.FindViewById<RatingBar> (Resource.Id.rating);

					// Get trail data.
					string trailJSONStr = Intent.GetStringExtra ("viewedTrail") ?? "No trail displayed";
					Trail trail = Newtonsoft.Json.JsonConvert.DeserializeObject<Trail> (trailJSONStr);
					distance.Text = trail.Distance + " km";
					duration.Text = trail.Duration + " h";
					description.Text = trail.Description;
					directions.Text = trail.Directions;
					difficultyRating.Text = trail.Difficulty.ToString();
					rating.Rating = trail.Rating.RatingNumber;

					return view;
				}
			);
			adapter.AddFragmentView((i, v, b) =>
				{
					// Debug version shows main screen
					var view = LayoutInflater.Inflate(Resource.Layout.Main, v, false);
					return view;
				}
			);
			pager.Adapter = adapter;

		}
	}
}

