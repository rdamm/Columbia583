using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Support.V4.View;
using Android.Support.V4.App;

using DK.Ostebaronen.Droid.ViewPagerIndicator;

namespace Columbia583.Android
{
	[Activity (Label = "View Trail", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
	public class ViewTrailActivity : FragmentActivity
	{
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
		protected GridView trailGallery = null;

		// Store references to the view fragments.
		protected global::Android.Views.View viewFragment1 = null;
		protected global::Android.Views.View viewFragment2 = null;
		protected global::Android.Views.View viewFragment3 = null;
		protected global::Android.Views.View viewFragment4 = null;

		// Store the trail's media.
		protected Media[] mediaList = null;

		protected global::Android.Widget.LinearLayout favouritesButtonWrapper = null;
		protected global::Android.Widget.Button uploadMediaButton = null;
		protected global::Android.Widget.Button commentButton = null;
		//private GestureDetector _gestureDetector;

		protected bool debugTrailA = true;

		protected global::Android.Widget.Button viewTrailMapsButton = null;

		protected int trailId;


		protected override void OnResume ()
		{
			base.OnResume ();

			// Refresh the page's media and comments.
			refreshMedia ();
			refreshComment ();
		}

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
			trailId = trail.id;
			trailName.Text = trail.name;

			Data_Access_Layer_View_Trail dataAccessLayerViewTrail = new Data_Access_Layer_View_Trail();
			mediaList = dataAccessLayerViewTrail.getMedia(trailId);

			Activity[] debugActivities = new Activity[6];
			Amenity[] debugAmenities = new Amenity[2];

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

					// Get trail data.
					distance.Text = trail.distance.Substring(0, 4) + " km";
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

					viewTrailMapsButton = view.FindViewById<global::Android.Widget.Button> (Resource.Id.button_map);
					favouritesButtonWrapper = view.FindViewById<global::Android.Widget.LinearLayout> (Resource.Id.layout_favouritesWrapper);

					if (viewTrailMapsButton != null){
						viewTrailMapsButton.Click += (sender, e) => {
							var intent = new Intent(this, typeof(TestMapActivity));
							StartActivity(intent);
						};
					}
					if (favouritesButtonWrapper != null)
					{
						// Only provide favourites options if the user is logged in.
						Data_Access_Layer_Common dataAccessLayerCommon = new Data_Access_Layer_Common();
						User activeUser = dataAccessLayerCommon.getActiveUser ();
						if (activeUser != null)
						{
							// Check if this trail is in the user's favourites.
							Data_Access_Layer_Favourites dataAccessLayerFavourites = new Data_Access_Layer_Favourites();
							bool trailIsFavourited = dataAccessLayerFavourites.trailIsFavourited(activeUser.id, trailId);

							// If the trail is in the user's favourites, set the remove button.  Otherwise, set the add button.
							if (trailIsFavourited == true)
							{
								SetRemoveFromFavouritesButton();
							}
							else
							{
								SetAddToFavouritesButton();
							}
						}
						else
						{
							// Clear the favourites wrapper.
							ClearFavouritesWrapper();
						}
					}

					// Store a reference to the view fragment.
					viewFragment1 = view;

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

					// Store a reference to the view fragment.
					viewFragment2 = view;

					return view;
				}
			);
			adapter.AddFragmentView((i, v, b) =>
				{
					var view = LayoutInflater.Inflate(Resource.Layout.ViewTrail3, v, false);
					commentsLayout = view.FindViewById<LinearLayout>(Resource.Id.commentsLayout);

					// query for all comments for trailId = seeHere
					Data_Access_Layer_View_Trail dvt = new Data_Access_Layer_View_Trail();
					Comment[] comments = dvt.getComments(trailId);
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
						date.Text = comment.createdAt.ToString();
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

					// Store a reference to the view fragment.
					viewFragment3 = view;

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
							intent.PutExtra ("trailId", trailId);
							StartActivity(intent);

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

					// Store a reference to the view fragment.
					viewFragment4 = view;

					return view;
				}
			);
			pager.Adapter = adapter;
			pageIndicator.SetViewPager(pager);

		}


		/// <summary>
		/// Refreshs the page's comments.
		/// </summary>
		protected void refreshComment()
		{
			if (viewFragment3 == null)
			{
				return;
			}

			// Get the controls.
			commentsLayout = viewFragment3.FindViewById<LinearLayout>(Resource.Id.commentsLayout);

			commentsLayout.RemoveAllViews ();

			var layout = viewFragment3.FindViewById<LinearLayout>(Resource.Id.commentsLayout);
			Data_Access_Layer_View_Trail dataAccessLayerViewTrail = new Data_Access_Layer_View_Trail();
			Comment[] comments = dataAccessLayerViewTrail.getComments(trailId);
			foreach (Comment comment in comments)
			{
				RatingBar ratingBar = new RatingBar(this);
				ratingBar.NumStars = 5;
				ratingBar.IsIndicator = true;
				ratingBar.StepSize = 1.0F;
				ratingBar.Rating = comment.rating;
				commentsLayout.AddView(ratingBar);

				TextView username = new TextView(this);
				username.Text = comment.username;
				username.TextSize = 17f;
				commentsLayout.AddView(username);

				TextView date = new TextView(this);
				date.Text = comment.createdAt.ToString();
				commentsLayout.AddView(date);

				TextView commentText = new TextView(this);
				commentText.Text = comment.text;
				commentsLayout.AddView(commentText);

				TextView borderText = new TextView(this);
				borderText.Text = "\n";
				commentsLayout.AddView(borderText);
			}

		}


		/// <summary>
		/// Refreshs the page's media.
		/// </summary>
		protected void refreshMedia()
		{
			// If the view fragment isn't set, do nothing.
			if (viewFragment4 == null)
			{
				return;
			}

			// Get the controls.
			trailGallery = viewFragment4.FindViewById<GridView>(Resource.Id.trailGallery);

			// Get the updated media.
			Data_Access_Layer_View_Trail dataAccessLayerViewTrail = new Data_Access_Layer_View_Trail();
			mediaList = dataAccessLayerViewTrail.getMedia(trailId);

			// Update the media contained in the gallery's adapter.
			trailGallery.Adapter = new MediaAdapter (this, mediaList);
		}


		/// <summary>
		/// Sets the favourites wrapper to contain a button for adding to favourites.
		/// </summary>
		protected void SetAddToFavouritesButton()
		{
			// Create the add to favourites button.
			global::Android.Widget.Button addToFavouritesButton = new global::Android.Widget.Button(this);
			addToFavouritesButton.Text = "Add to Favourites";

			// Set the event handler.
			addToFavouritesButton.Click += AddToFavouritesEvent;

			// Add the button to the wrapper.
			favouritesButtonWrapper.RemoveAllViews();
			favouritesButtonWrapper.AddView (addToFavouritesButton);
		}


		/// <summary>
		/// Sets the favourites wrapper to contain a button for removing from favourites.
		/// </summary>
		protected void SetRemoveFromFavouritesButton()
		{
			// Create the remove from favourites button.
			global::Android.Widget.Button removeFromFavouritesButton = new global::Android.Widget.Button(this);
			removeFromFavouritesButton.Text = "Remove From Favourites";

			// Set the event handler.
			removeFromFavouritesButton.Click += RemoveFromFavouritesEvent;

			// Add the button to the wrapper.
			favouritesButtonWrapper.RemoveAllViews();
			favouritesButtonWrapper.AddView(removeFromFavouritesButton);
		}


		/// <summary>
		/// Clears the favourites wrapper.
		/// </summary>
		protected void ClearFavouritesWrapper()
		{
			favouritesButtonWrapper.RemoveAllViews();
		}


		/// <summary>
		/// Adds this trail to the user's favourites.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void AddToFavouritesEvent(object sender, EventArgs eventArgs)
		{
			// Only provide favourites options if the user is logged in.
			Data_Access_Layer_Common dataAccessLayerCommon = new Data_Access_Layer_Common();
			User activeUser = dataAccessLayerCommon.getActiveUser ();
			if (activeUser != null)
			{
				// Add the trail to favourites.
				Data_Access_Layer_Favourites dataAccessLayerFavourites = new Data_Access_Layer_Favourites();
				dataAccessLayerFavourites.addFavouriteTrail(activeUser.id, trailId);

				// Set the remove from favourites button.
				SetRemoveFromFavouritesButton();

				// Inform the user that the trail was added to favourites.
				Toast.MakeText(this, "Added to your favourites.", ToastLength.Short).Show();
			}
			else
			{
				// Clear the favourites wrapper.
				ClearFavouritesWrapper();

				// Inform the user that a login is required.
				Toast.MakeText (this, "Login required for favourites.", ToastLength.Short).Show ();
			}
		}


		/// <summary>
		/// Removes this trail from the user's favourites.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void RemoveFromFavouritesEvent(object sender, EventArgs eventArgs)
		{
			// Only provide favourites options if the user is logged in.
			Data_Access_Layer_Common dataAccessLayerCommon = new Data_Access_Layer_Common();
			User activeUser = dataAccessLayerCommon.getActiveUser ();
			if (activeUser != null)
			{
				// Remove the trail from favourites.
				Data_Access_Layer_Favourites dataAccessLayerFavourites = new Data_Access_Layer_Favourites();
				dataAccessLayerFavourites.removeFavouriteTrail(activeUser.id, trailId);

				// Set the add to favourites button.
				SetAddToFavouritesButton();

				// Inform the user that the trail was removed from favourites.
				Toast.MakeText(this, "Removed from your favourites.", ToastLength.Short).Show();
			}
			else
			{
				// Clear the favourites wrapper.
				ClearFavouritesWrapper();

				// Inform the user that a login is required.
				Toast.MakeText (this, "Login required for favourites.", ToastLength.Short).Show ();
			}
		}
	}
}

