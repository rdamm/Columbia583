using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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

using DK.Ostebaronen.Droid.ViewPagerIndicator;

namespace Columbia583.Android
{
	[Activity (Label = "Columbia583.Android_View_Trail", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
	public class ViewTrailActivity : FragmentActivity
	{
		protected int trailId;

		protected ViewPager pager = null;
		protected MyFragmentPagerAdapter adapter = null;
		protected CirclePageIndicator pageIndicator = null;
		protected TextView trailName = null;
		//protected ViewSwitcher viewSwitcher = null;
		protected LinearLayout activitiesLayout = null;
		protected LinearLayout amenitiesLayout = null;
		protected TextView distance = null;
		protected TextView duration = null;
		protected TextView description = null;
		protected TextView directions = null;
		protected TextView difficultyRating = null;
		protected RatingBar rating = null;
		protected TextView openStatus = null;
		protected TextView season = null;
		protected TextView maintenance = null;
		protected LinearLayout commentsLayout = null;
		protected global::Android.Widget.Button addToFavouritesButton = null;
		protected global::Android.Widget.Button uploadMediaButton = null;
		protected GridView trailGallery = null;
		//private GestureDetector _gestureDetector;
		protected global::Android.Widget.Button commentButton = null;
		protected bool debugTrailA = true;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			//SetPage (App.GetViewTrailPage ());
			SetContentView (Resource.Layout.ViewTrailBase);

			pager = FindViewById<ViewPager> (Resource.Id.viewpager);
			trailName = FindViewById<TextView> (Resource.Id.trailName);
			pageIndicator = FindViewById<CirclePageIndicator> (Resource.Id.pageIndicator);
			adapter = new MyFragmentPagerAdapter (SupportFragmentManager);

			string trailJSONStr = Intent.GetStringExtra ("viewedTrail") ?? "No trail displayed";
			string activitiesJSONstr = Intent.GetStringExtra ("activities") ?? "No activities found";
			string amenitiesJSONstr = Intent.GetStringExtra ("amenities") ?? "No amenities found";
			string pointsJSONstr = Intent.GetStringExtra ("points") ?? "No points found";
			Trail trail = Newtonsoft.Json.JsonConvert.DeserializeObject<Trail> (trailJSONStr);
			Activity[] activities = Newtonsoft.Json.JsonConvert.DeserializeObject<Activity[]> (activitiesJSONstr);
			Amenity[] amenities = Newtonsoft.Json.JsonConvert.DeserializeObject<Amenity[]> (amenitiesJSONstr);
			Point[] points = Newtonsoft.Json.JsonConvert.DeserializeObject<Point[]> (pointsJSONstr);
			trailName.Text = trail.name;

			trailId = trail.id;

			Activity[] debugActivities = new Activity[6];
			Amenity[] debugAmenities = new Amenity[2];

			// Get the trail's media.
			Data_Access_Layer_View_Trail dataAccessLayerViewTrail = new Data_Access_Layer_View_Trail();
			Media[] mediaList = dataAccessLayerViewTrail.getMedia(trail.id);

			if (debugTrailA) {
				debugActivities [0] = new Activity (1, "Hiking", new byte[0], DateTime.Now);
				debugActivities[1] = new Activity(2, "Mountain Biking", new byte[0], DateTime.Now);
				debugActivities [2] = new Activity (3, "Skiing", new byte[0], DateTime.Now);
				debugActivities [3] = new Activity (4, "Swimming", new byte[0], DateTime.Now);
				debugActivities [4] = new Activity (5, "Snowboarding", new byte[0], DateTime.Now);
				debugActivities [5] = new Activity (6, "Boating", new byte[0], DateTime.Now);

				debugAmenities [0] = new Amenity (1, "restrooms", new byte[0], DateTime.Now);
				debugAmenities[1] = new Amenity(2, "camping", new byte[0], DateTime.Now);
			}

			adapter.AddFragmentView((i, v, b) =>
				{
					var view = LayoutInflater.Inflate(Resource.Layout.ViewTrail, v, false);
					// Get the controls.
					distance = view.FindViewById<TextView> (Resource.Id.distance);
					duration = view.FindViewById<TextView> (Resource.Id.duration);
					description = view.FindViewById<TextView> (Resource.Id.description);
					difficultyRating = view.FindViewById<TextView> (Resource.Id.difficultyRating);
					rating = view.FindViewById<RatingBar> (Resource.Id.rating);
					openStatus = view.FindViewById<TextView>(Resource.Id.openStatus);
					season = view.FindViewById<TextView>(Resource.Id.season);
					maintenance = view.FindViewById<TextView>(Resource.Id.maintenance);
					addToFavouritesButton = view.FindViewById<global::Android.Widget.Button>(Resource.Id.btnAddToFavourites);

					// Get trail data.
					distance.Text = trail.distance + " km";
					duration.Text = trail.duration + " h";
					description.Text = trail.description;
					difficultyRating.Text = trail.difficulty.ToString().Replace("_", " ");
					rating.Rating = trail.rating;
					if (trail.open) {
						openStatus.Text = "Open";
					} else {
						openStatus.Text = "Closed";
					}
					season.Text = "Season: " + trail.season;
					maintenance.Text = trail.maintenance;

					// Set the event handlers.
					if (addToFavouritesButton != null) {
						addToFavouritesButton.Click += (sender, e) => {

							// Get the active user's ID.
							Data_Access_Layer_Common dataAccessLayerCommon = new Data_Access_Layer_Common();
							User activeUser = dataAccessLayerCommon.getActiveUser();
							int activeUserId = activeUser.id;

							// Add the trail to the user's favourites list.
							Data_Access_Layer_Favourites dataAccessLayerFavourites = new Data_Access_Layer_Favourites();
							dataAccessLayerFavourites.addFavouriteTrail(activeUserId, trailId);

						};
					}

					return view;
				}
			);
			adapter.AddFragmentView((i, v, b) =>
				{
					var view = LayoutInflater.Inflate(Resource.Layout.ViewTrail2, v, false);
					activitiesLayout = view.FindViewById<LinearLayout>(Resource.Id.activities);
					amenitiesLayout = view.FindViewById<LinearLayout>(Resource.Id.amenities);
					directions = view.FindViewById<TextView> (Resource.Id.directions);

					directions.Text = trail.directions;

					if (debugTrailA) {
						foreach (Activity activity in activities)
						{
							TextView activityName = new TextView(this);
							activityName.Text = activity.activityName;
							activitiesLayout.AddView(activityName);
						}
						foreach (Amenity amenity in amenities)
						{
							TextView amenityName = new TextView(this);
							amenityName.Text = amenity.amenityName;
							amenitiesLayout.AddView(amenityName);
						}
					}
					return view;
				}
			);
			adapter.AddFragmentView((i, v, b) =>
				{
					var view = LayoutInflater.Inflate(Resource.Layout.ViewTrail3, v, false);
					commentsLayout = view.FindViewById<LinearLayout>(Resource.Id.commentsLayout);

					// query for all comments for trailId = seeHere
					Data_Access_Layer_View_Trail dvt = new Data_Access_Layer_View_Trail();
					Comment[] comments = dvt.getComments(trail.id);
					foreach (Comment comment in comments)
					{
						RatingBar ratingBar = new RatingBar(this);
						ratingBar.NumStars = 5;
						ratingBar.IsIndicator = true;
						ratingBar.StepSize = 1.0F;
						ratingBar.Rating = comment.rating;
						commentsLayout.AddView(ratingBar);

						TextView username = new TextView(this);
						/*
						if (dvt.getUserForComment(comment) != null) {
							username.Text = dvt.getUserForComment(comment).username;
						} else {
							username.Text = "Anonymous";
						}
						*/
						username.Text = comment.username;
						username.TextSize = 17f;
						commentsLayout.AddView(username);

						TextView date = new TextView(this);
						date.Text = comment.date.ToString();
						commentsLayout.AddView(date);

						TextView commentText = new TextView(this);
						commentText.Text = comment.text;
						commentsLayout.AddView(commentText);

						TextView borderText = new TextView(this);
						borderText.Text = "\n";
						commentsLayout.AddView(borderText);
					}

					commentButton = view.FindViewById<global::Android.Widget.Button> (Resource.Id.uploadComment);

					commentButton.Click += (object sender, EventArgs e) => {

						var intent = new Intent (this, typeof(UploadComment));

						string jsonModel = JsonConvert.SerializeObject (trail);
						//string jsonModelUser = JsonConvert.SerializeObject();
						intent.PutExtra ("Trail Data", jsonModel);
						//intent.PutExtra("User Data", jsonModelUser);

						StartActivity (intent);
					};


					return view;

				}
			);
			adapter.AddFragmentView((i, v, b) =>
				{
					var view = LayoutInflater.Inflate(Resource.Layout.ViewTrail4, v, false);

					// Get the controls.
					trailGallery = view.FindViewById<GridView>(Resource.Id.trailGallery);
					uploadMediaButton = view.FindViewById<global::Android.Widget.Button>(Resource.Id.btnUploadMedia);

					// Set the event handlers.
					if (uploadMediaButton != null)
					{
						uploadMediaButton.Click += (sender, e) => {

							// Load the upload media page.
							var intent = new Intent(this, typeof(UploadMediaActivity));
							intent.PutExtra ("trailId", trail.id);
							StartActivity(intent);

							// TODO: When the upload media page closes, refresh the available media.

						};
					}

					// Create an adapter to populate the page with images and associate them with common indices.
					trailGallery.Adapter = new MediaAdapter (this, mediaList);
					trailGallery.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args) {

						// Get the selected media.
						Media selectedMedia = mediaList[args.Position];

						// Show the selected media in a new activity.
						var intent = new Intent(this, typeof(ViewMediaActivity));
						intent.PutExtra("mediaId", selectedMedia.id);
						StartActivity(intent);

					};

					return view;
				}
			);
			pager.Adapter = adapter;
			pageIndicator.SetViewPager(pager);

		}
	}
}

