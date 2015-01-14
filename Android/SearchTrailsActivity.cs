using System;
using System.Collections.Generic;

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
	[Activity (Label = "Columbia583.Android_Search_Trails", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class SearchTrailsActivity : AndroidActivity
	{
		// Activities.
		protected CheckBox hikingCheckBox = null;
		protected CheckBox skiingCheckBox = null;
		protected CheckBox bikingCheckBox = null;

		// Amenities.
		protected LinearLayout amenityOptions = null;
		protected CheckBox[] amenityCheckBoxes = null;

		// Difficulties.
		protected CheckBox easiestCheckBox = null;
		protected CheckBox easyCheckBox = null;
		protected CheckBox moreDifficultCheckBox = null;
		protected CheckBox veryDifficultCheckBox = null;
		protected CheckBox extremelyDifficultCheckBox = null;

		// Ratings.
		protected LinearLayout ratingsOptions = null;
		protected CheckBox[] ratingsCheckBoxes = null;

		// Duration.
		protected EditText durationEditText = null;

		// Distance.
		protected EditText distanceEditText = null;

		// Other controls.
		protected Button updateSearchResultsButton = null;
		protected GridLayout searchResultsGrid = null;
		//protected Button viewTrailButton = null;

		// Debug
		protected bool debugSearchResults = true;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetSearchTrailsPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.SearchTrails);

			// Get the controls.
			hikingCheckBox = FindViewById<CheckBox> (Resource.Id.checkBox_activity_hiking);
			skiingCheckBox = FindViewById<CheckBox> (Resource.Id.checkBox_activity_skiing);
			bikingCheckBox = FindViewById<CheckBox> (Resource.Id.checkBox_activity_biking);
			amenityOptions = FindViewById<LinearLayout> (Resource.Id.amenityOptions);
			easiestCheckBox = FindViewById<CheckBox> (Resource.Id.checkBox_difficulty_easiest);
			easyCheckBox = FindViewById<CheckBox> (Resource.Id.checkBox_difficulty_easy);
			moreDifficultCheckBox = FindViewById<CheckBox> (Resource.Id.checkBox_difficulty_moreDifficult);
			veryDifficultCheckBox = FindViewById<CheckBox> (Resource.Id.checkBox_difficulty_veryDifficult);
			extremelyDifficultCheckBox = FindViewById<CheckBox> (Resource.Id.checkBox_difficulty_extremelyDifficult);
			ratingsOptions = FindViewById<LinearLayout> (Resource.Id.ratingsOptions);
			ratingsCheckBoxes = new CheckBox[5];
			ratingsCheckBoxes[0] = FindViewById<CheckBox> (Resource.Id.checkBox_rating_1);
			ratingsCheckBoxes[1] = FindViewById<CheckBox> (Resource.Id.checkBox_rating_2);
			ratingsCheckBoxes[2] = FindViewById<CheckBox> (Resource.Id.checkBox_rating_3);
			ratingsCheckBoxes[3] = FindViewById<CheckBox> (Resource.Id.checkBox_rating_4);
			ratingsCheckBoxes[4] = FindViewById<CheckBox> (Resource.Id.checkBox_rating_5);
			durationEditText = FindViewById<EditText> (Resource.Id.editText_maxDuration);
			distanceEditText = FindViewById<EditText> (Resource.Id.editText_maxDistance);
			updateSearchResultsButton = FindViewById<Button> (Resource.Id.button_updateSearchResults);
			searchResultsGrid = FindViewById<GridLayout> (Resource.Id.gridLayout_searchResults);
			//viewTrailButton = FindViewById<Button> (Resource.Id.button_viewTrail);

			Trail[] debugTrails = new Trail[3];
			Activity[] debugActivities = new Activity[2];
			Amenity[] debugAmenities = new Amenity[2];

			if (debugSearchResults) {
				debugTrails[0] = new Trail(0, 0, 0, "Edgewater Trail", "BC", "", "", "66.69", "10", "Lorem ipsum", "three steps north, then turn right", Difficulty.Easiest, 4, new int[]{1, 2}, new int[]{2}, "", "", "", "", true, true, DateTime.Now);
				debugTrails[1] = new Trail(0, 0, 0, "Niles", "BC", "", "", "113.12", "20", "dolores umbridge", "Go to Neverland", Difficulty.Very_Difficult, 3, new int[]{1}, new int[]{1, 2}, "", "", "", "", true, true, DateTime.Now);
				debugTrails[2] = new Trail(0, 0, 0, "Findlay Creek Trail 2", "AB", "", "", "0.59", "0.45", "Presumably, there's a Findlay Creek Trail 1, but this isn't it", "1337 d1r3c710n5", Difficulty.Extremely_Difficult, 5, new int[]{1, 2}, new int[]{1, 2}, "", "", "", "", true, true, DateTime.Now);

				debugActivities [0] = new Activity (1, "Hiking", "images/activities/activity-hike.png", DateTime.Now);
				debugActivities[1] = new Activity(2, "Mountain Biking", "images/activities/activity-bike.png", DateTime.Now);

				debugAmenities [0] = new Amenity (1, "restrooms", "images/amenities/restrooms_32.png", DateTime.Now);
				debugAmenities[1] = new Amenity(2, "camping", "images/amenities/camping_32.png", DateTime.Now);

				amenityCheckBoxes = new CheckBox[debugAmenities.Length];

				this.setSearchResults(debugTrails);
			}

			for (int i = 0; i < amenityCheckBoxes.Length; i++) {
				amenityCheckBoxes [i] = new CheckBox (this);
				if (debugSearchResults) {
					amenityCheckBoxes [i].Text = debugAmenities [i].AmenityName;
				}
				amenityOptions.AddView (amenityCheckBoxes [i]);
			}

			// Assign an event handler to the update search results button.
			if (updateSearchResultsButton != null) {
				updateSearchResultsButton.Click += (sender, e) => {
					
					// Get the search filter parameters from the controls.
					SearchFilter searchFilter = this.getSearchFilter();

					// Get the search results.
					Application_Layer_Search_Trails applicationLayer_searchTrails = new Application_Layer_Search_Trails ();
					Trail[] trails = applicationLayer_searchTrails.getTrailsByFilters (searchFilter);

					// Show the search results.
					this.setSearchResults(trails);
				};
			}

			/*
			// Assign an event handler to the view trail button.
			if (viewTrailButton != null) {
				viewTrailButton.Click += (sender, e) => {

					// Load the view trail page.
					var intent = new Intent(this, typeof(ViewTrailActivity));
					// JSON serialization works
					string trailJSONStr = Newtonsoft.Json.JsonConvert.SerializeObject(debugTrails[0]);
					intent.PutExtra("viewedTrail", trailJSONStr);
					StartActivity(intent);

				};
			}
			*/
		}


		/**
		 * Get the search filters from the controls.
		 * */
		protected SearchFilter getSearchFilter()
		{
			List<Difficulty> difficultiesList = new List<Difficulty>();
			List<string> activitiesList = new List<string>();
			List<string> amenitiesList = new List<string> ();
			List<int> ratingsList = new List<int>();

			// Get the search filter parameters from the controls.
			// TODO: Enumerations or strings for activities?
			// TODO: Get the min duration and distances.
			if (hikingCheckBox != null && hikingCheckBox.Checked == true) {
				activitiesList.Add("hiking");
			}
			if (skiingCheckBox != null && skiingCheckBox.Checked == true) {
				activitiesList.Add("skiing");
			}
			if (bikingCheckBox != null && bikingCheckBox.Checked == true) {
				activitiesList.Add("biking");
			}
			for (int i = 0; i < amenityCheckBoxes.Length; i++) {
				if (amenityCheckBoxes[i] != null && amenityCheckBoxes[i].Checked) {
					amenitiesList.Add (amenityCheckBoxes[i].Text);
				}
			}
			if (easiestCheckBox != null && easiestCheckBox.Checked == true) {
				difficultiesList.Add(Difficulty.Easiest);
			}
			if (easyCheckBox != null && easyCheckBox.Checked == true) {
				difficultiesList.Add(Difficulty.Easy);
			}
			if (moreDifficultCheckBox != null && moreDifficultCheckBox.Checked == true) {
				difficultiesList.Add(Difficulty.More_Difficult);
			}
			if (veryDifficultCheckBox != null && veryDifficultCheckBox.Checked == true) {
				difficultiesList.Add(Difficulty.Very_Difficult);
			}
			if (extremelyDifficultCheckBox != null && extremelyDifficultCheckBox.Checked == true) {
				difficultiesList.Add(Difficulty.Extremely_Difficult);
			}
			for (int i = 0; i < ratingsCheckBoxes.Length; i++) {
				if (ratingsCheckBoxes[i] != null && ratingsCheckBoxes[i].Checked) {
					ratingsList.Add(i + 1);
				}
			}
			int minDuration = 0;
			int maxDuration = minDuration;
			if (durationEditText != null && durationEditText.Text != "") {
				maxDuration = Convert.ToInt32(durationEditText.Text);
			}
			int minDistance = 0;
			int maxDistance = minDistance;
			if (distanceEditText != null && distanceEditText.Text != "") {
				maxDistance = Convert.ToInt32(distanceEditText.Text);
			}

			// Encapsulate the filter parameters.
			SearchFilter searchFilter = new SearchFilter(activitiesList.ToArray(), amenitiesList.ToArray(), difficultiesList.ToArray(), ratingsList.ToArray(), minDuration, maxDuration, minDistance, maxDistance);

			return searchFilter;
		}


		/**
		 * Display the search results in the grid.
		 * */
		protected void setSearchResults(Trail[] trails)
		{
			// Display the trails in the view.
			if (searchResultsGrid != null) {
				// Empty the list.
				searchResultsGrid.RemoveAllViews();

				// Show a summary of each matching trail.
				if (trails != null) {
					foreach(Trail trail in trails) {
						const int NUM_ELEMENTS_PER_TRAIL = 4;
						TextView[] trailElements = new TextView[NUM_ELEMENTS_PER_TRAIL];
//						// TODO: Display the trail name.
						trailElements[0] = new TextView (this);
						trailElements[0].Text = trail.Name;
//
						// TODO: Display the trail rating.
						trailElements[1] = new TextView (this);
						string ratingStars = "";
						for (int i = 0; i < trail.Rating; i++) {
							ratingStars += "*";
						}
						trailElements[1].Text = ratingStars;
//
//						// TODO: Display the trail difficulty.
						trailElements[2] = new TextView (this);
						trailElements[2].Text = trail.Difficulty.ToString().Replace("_", " ");
//
//						// TODO: Display the trail distance.
						trailElements[3] = new TextView (this);
						trailElements[3].Text = trail.Distance + " km";

						for (int i = 0; i < NUM_ELEMENTS_PER_TRAIL; i++) {
							trailElements[i].Click += (sender, e) => {

								// Load the view trail page.
								var intent = new Intent (this, typeof(ViewTrailActivity));
								// JSON serialization works
								string trailJSONStr = Newtonsoft.Json.JsonConvert.SerializeObject (trail);
								intent.PutExtra ("viewedTrail", trailJSONStr);
								StartActivity (intent);

							};
							searchResultsGrid.AddView (trailElements[i]);
						}
					}
				}
			}
		}
	}
}