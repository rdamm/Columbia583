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
		protected ViewPager pager = null;
		protected MyFragmentPagerAdapter adapter = null;
		protected TextView trailName = null;
		//protected ViewSwitcher viewSwitcher = null;
		protected LinearLayout layout1 = null;
		protected TextView distance = null;
		protected TextView duration = null;
		protected TextView description = null;
		protected TextView directions = null;
		protected TextView difficultyRating = null;
		protected RatingBar rating = null;
		protected TextView openStatus = null;
		protected TextView season = null;
		protected TextView maintenance = null;
		//private GestureDetector _gestureDetector;

		protected bool debugTrailA = true;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			//SetPage (App.GetViewTrailPage ());
			SetContentView (Resource.Layout.ViewTrailBase);

			pager = FindViewById<ViewPager> (Resource.Id.viewpager);
			trailName = FindViewById<TextView> (Resource.Id.trailName);
			adapter = new MyFragmentPagerAdapter (SupportFragmentManager);

			string trailJSONStr = Intent.GetStringExtra ("viewedTrail") ?? "No trail displayed";
			Trail trail = Newtonsoft.Json.JsonConvert.DeserializeObject<Trail> (trailJSONStr);
			trailName.Text = trail.Name;

			Activity[] debugActivities = new Activity[2];
			Amenity[] debugAmenities = new Amenity[2];

			if (debugTrailA) {
				debugActivities [0] = new Activity (1, "Hiking", "images/activities/activity-hike.png", DateTime.Now);
				debugActivities[1] = new Activity(2, "Mountain Biking", "images/activities/activity-bike.png", DateTime.Now);

				debugAmenities [0] = new Amenity (1, "restrooms", "images/amenities/restrooms_32.png", DateTime.Now);
				debugAmenities[1] = new Amenity(2, "camping", "images/amenities/camping_32.png", DateTime.Now);
			}

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
					openStatus = view.FindViewById<TextView>(Resource.Id.openStatus);
					season = view.FindViewById<TextView>(Resource.Id.season);
					maintenance = view.FindViewById<TextView>(Resource.Id.maintenance);

					// Get trail data.
					distance.Text = trail.Distance + " km";
					duration.Text = trail.Duration + " h";
					description.Text = trail.Description;
					directions.Text = trail.Directions;
					difficultyRating.Text = trail.Difficulty.ToString().Replace("_", " ");
					rating.Rating = trail.Rating;
					if (trail.Open) {
						openStatus.Text = "Open";
					} else {
						openStatus.Text = "Closed";
					}
					season.Text = "Season: " + trail.Season;
					maintenance.Text = trail.Maintenance;

					return view;
				}
			);
			adapter.AddFragmentView((i, v, b) =>
				{
					var view = LayoutInflater.Inflate(Resource.Layout.ViewTrail2, v, false);
					return view;
				}
			);
			pager.Adapter = adapter;

		}
	}
}

