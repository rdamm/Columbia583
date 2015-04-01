
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Platform.Android;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Columbia583.Android
{
	[Activity (Label = "SearchTrailsPage")]			
	public class SearchTrailsPage : AndroidActivity
	{
		private int getDifficulty=0;
		private string getDistance="0";
		private int getRating= -1;
		private string getDuration="0";
		private const int activityDialog = 1;
		private const int amenityDialog = 2;
		String[] activitiesSelected;
		List<string> activities_check_list = new List<string> ();
		List<string> activitiesList_String = new List<string> ();
		List<int> activitiesList_ID = new List<int> ();

		String[] amenitiesSelected;
		List<string> amenities_check_list = new List<string> ();
		List<string> amenitiesList_String = new List<string> ();
		List<int> amenitiesList_ID = new List<int> ();
		bool getOpen, getActive;

		SeekBar difficulty ;
		TextView diffText ;
		TextView ratingText ;
		TextView durationText ;
		TextView distanceText ;
		Button activity ;
		Button amenity ;
		RatingBar rating ;
		SeekBar maxDuration ;
		SeekBar maxDistance ;
		Button upload ;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			// Create your application here
			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.SearchTrailPage);

			difficulty = FindViewById<SeekBar> (Resource.Id.difficultyBar);
			diffText = FindViewById<TextView> (Resource.Id.difficultyText);
			ratingText = FindViewById<TextView> (Resource.Id.ratingText);
			durationText = FindViewById<TextView> (Resource.Id.durationText);
			distanceText = FindViewById<TextView> (Resource.Id.distanceText);
			activity = FindViewById<Button> (Resource.Id.activityButton);
			amenity = FindViewById<Button> (Resource.Id.amenityButton);
			rating = FindViewById<RatingBar> (Resource.Id.ratingBar);
			maxDuration = FindViewById<SeekBar> (Resource.Id.MaxdurationBar);
			maxDistance = FindViewById<SeekBar> (Resource.Id.maxDistanceBar);
			upload = FindViewById<Button> (Resource.Id.update);

			difficulty.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
				Data_Layer_Common dataLayer = new Data_Layer_Common ();
				string[] names = dataLayer.getDifficulty ();

				if(e.FromUser){
					if(e.Progress <=20){
						diffText.Text = String.Format("You have selected easiest difficulty.");
						getDifficulty = (int) Difficulty.Easiest;
					}
					else if(e.Progress <=40 && e.Progress > 20){
						diffText.Text = String.Format("You have selected easy difficulty.");
						getDifficulty = (int)Difficulty.Easy;
					}
					else if(e.Progress <=60 && e.Progress > 40){
						diffText.Text = String.Format("You have selected more difficult difficulty.");
						getDifficulty = (int) Difficulty.More_Difficult;
					}
					else if(e.Progress <=80 && e.Progress > 60){
						diffText.Text = String.Format("You have selected very difficult difficulty.");
						getDifficulty = (int) Difficulty.Very_Difficult;
					}
					else if(e.Progress <=100 && e.Progress > 80){
						diffText.Text = String.Format("You have selected extremely difficult difficulty.");
						getDifficulty = (int) Difficulty.Extremely_Difficult;
					}

				}
			};

			rating.Click += (object sender, EventArgs e) => {
				getRating = (int) rating.Rating;
			};

			maxDuration.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
				if(e.FromUser){
					durationText.Text= String.Format("You have selected {0} as duration",e.Progress);
					getDuration= e.Progress.ToString();
				}
			};

			maxDistance.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
				if(e.FromUser){
					distanceText.Text= String.Format("You have selected {0} as distance",e.Progress);
					getDistance= e.Progress.ToString();
				}
			};

			Data_Layer_Common dataLayer2 = new Data_Layer_Common ();
			List<Activity> debugActivities = dataLayer2.getActivities ();
			List<Amenity> debugAmenities = dataLayer2.getAmenities ();

			foreach (Activity act in debugActivities) 
			{
				activities_check_list.Add(act.activityName);
			}

			foreach (Amenity am in debugAmenities) 
			{
				amenities_check_list.Add(am.amenityName);
			}

			activity.Click += delegate {
				ShowDialog(activityDialog);
			};

			amenity.Click += delegate {
				ShowDialog(amenityDialog);
			};

			upload.Click += (object sender, EventArgs e) => {
				getRating =(int) rating.Rating;
				//				Application_Layer_Search_Trails applicationLayer_searchTrails = new Application_Layer_Search_Trails ();
				//				SearchResult[] debugSearchResults = applicationLayer_searchTrails.getTrailsBySearchFilter (new SearchFilter (){ rating = 1 });


				//var intent = new Intent (this, typeof(SearchTrailPage2));
				//Console.Out.WriteLine(debugSearchResults[1].trail.name);

				//intent.PutExtra("search results", result);
				//StartActivity(intent);

				SearchFilter searchFilter = new SearchFilter();
				searchFilter=this.getSearchFilter();

				this.setSearchResults(searchFilter);

			};
		}

		protected override Dialog OnCreateDialog(int id, Bundle args)
		{
			switch (id) 
			{
			case activityDialog:
				{
					var builder = new AlertDialog.Builder(this);
					builder.SetTitle(Resource.String.activitiesList);
					builder.SetCancelable(true);
					builder.SetMultiChoiceItems(activities_check_list.ToArray(), null, activityListClicked);

					builder.SetPositiveButton(Resource.String.submitButtonName, submitButtonClicked_Activity);
					//builder.SetNegativeButton(Resource.String.negativeOption, cancelClicked);

					return builder.Create();
				}
			case amenityDialog:
				{
					var builder = new AlertDialog.Builder(this);
					builder.SetTitle(Resource.String.amenitiesList);
					builder.SetCancelable(true);
					builder.SetMultiChoiceItems(amenities_check_list.ToArray(), null, amenityListClicked);

					builder.SetPositiveButton(Resource.String.submitButtonName, submitButtonClicked_Amenity);
					//builder.SetNegativeButton(Resource.String.negativeOption, cancelClicked);

					return builder.Create();
				}
			}

			return base.OnCreateDialog(id, args);
		}

		private void activityListClicked(object sender, DialogMultiChoiceClickEventArgs args)
		{
			activitiesSelected = activities_check_list.ToArray();

			if (args.IsChecked) {
				activitiesList_String.Add (activitiesSelected [args.Which]);
			} else if (activitiesList_String.Contains (activitiesSelected [args.Which])) {
				activitiesList_String.Remove (activitiesSelected [args.Which]);
			}
		}

		private void amenityListClicked(object sender, DialogMultiChoiceClickEventArgs args)
		{
			amenitiesSelected = amenities_check_list.ToArray();

			if (args.IsChecked) {
				amenitiesList_String.Add (amenitiesSelected [args.Which]);
			} else if (amenitiesList_String.Contains (amenitiesSelected [args.Which])) {
				amenitiesList_String.Remove (amenitiesSelected [args.Which]);
			}
		}

		private void submitButtonClicked_Activity(object sender, DialogClickEventArgs args)
		{
			Dialog dialog = (AlertDialog) sender;

			int i = 0;

			activitiesList_String.ToArray ();
			Data_Layer_Search_Trails dlSearch = new Data_Layer_Search_Trails ();
			activitiesList_ID.Clear ();

			foreach (string s in activitiesList_String)
			{
				//Activity activity = new Activity (i, activitiesList_String[i], new byte[0], DateTime.Now);
				activitiesList_ID.Add (dlSearch.getActivityIdByName(s));

				i++;
			}
			Console.WriteLine ("Activities submitted.");

			dialog.Dismiss ();
		}

		private void submitButtonClicked_Amenity(object sender, DialogClickEventArgs args)
		{
			Dialog dialog = (AlertDialog) sender;

			int j = 0;

			amenitiesList_String.ToArray ();

			Data_Layer_Search_Trails dlSearch = new Data_Layer_Search_Trails ();

			amenitiesList_ID.Clear ();

			foreach (string t in amenitiesList_String)
			{
				//Amenity amenity = new Amenity (j, amenitiesList_String[j], new byte[0], DateTime.Now);

				amenitiesList_ID.Add (dlSearch.getAmenityIdByName(t));

				j++;
			}

			dialog.Dismiss ();
		}

		protected SearchFilter getSearchFilter()
		{

			int minDuration = 0;
			int maxDuration = int.Parse (getDuration);
			int minDistance = 0;
			int maxDistance = int.Parse(getDistance);

			// Encapsulate the filter parameters.
			SearchFilter searchFilter = new SearchFilter(activitiesList_ID.ToArray(), amenitiesList_ID.ToArray(), getDifficulty, getRating, minDuration, maxDuration, minDistance, maxDistance);

			return searchFilter;
		}

		protected void setSearchResults(SearchFilter searchResults){
			var intent = new Intent (this, typeof(SearchTrailPage2));
			string result = Newtonsoft.Json.JsonConvert.SerializeObject(searchResults);
			intent.PutExtra("search", result);
			StartActivity (intent);

		}

	}
}

