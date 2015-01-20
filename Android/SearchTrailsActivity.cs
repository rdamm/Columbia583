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
		protected LinearLayout activityOptions = null;
		protected CheckBox[] activityCheckBoxes = null;

		// Amenities.
		protected LinearLayout amenityOptions = null;
		protected ScrollView amenityOptionsScroll = null;
		protected CheckBox[] amenityCheckBoxes = null;

		// Difficulties.
		protected RadioButton easiestRadioButton = null;
		protected RadioButton easyRadioButton = null;
		protected RadioButton moreDifficultRadioButton = null;
		protected RadioButton veryDifficultRadioButton = null;
		protected RadioButton extremelyDifficultRadioButton = null;

		// Ratings.
		protected LinearLayout ratingsOptions = null;
		protected RadioButton[] ratingsRadioButtons = null;

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
			activityOptions = FindViewById<LinearLayout> (Resource.Id.activityOptions);
			amenityOptions = FindViewById<LinearLayout> (Resource.Id.amenityOptions);
			amenityOptionsScroll = FindViewById<ScrollView> (Resource.Id.amenityOptionsScroll);
			easiestRadioButton = FindViewById<RadioButton> (Resource.Id.radioButton_difficulty_easiest);
			easyRadioButton = FindViewById<RadioButton> (Resource.Id.radioButton_difficulty_easy);
			moreDifficultRadioButton = FindViewById<RadioButton> (Resource.Id.radioButton_difficulty_moreDifficult);
			veryDifficultRadioButton = FindViewById<RadioButton> (Resource.Id.radioButton_difficulty_veryDifficult);
			extremelyDifficultRadioButton = FindViewById<RadioButton> (Resource.Id.radioButton_difficulty_extremelyDifficult);
			ratingsOptions = FindViewById<LinearLayout> (Resource.Id.ratingsOptions);
			ratingsRadioButtons = new RadioButton[5];
			ratingsRadioButtons[0] = FindViewById<RadioButton> (Resource.Id.radioButton_rating_1);
			ratingsRadioButtons[1] = FindViewById<RadioButton> (Resource.Id.radioButton_rating_2);
			ratingsRadioButtons[2] = FindViewById<RadioButton> (Resource.Id.radioButton_rating_3);
			ratingsRadioButtons[3] = FindViewById<RadioButton> (Resource.Id.radioButton_rating_4);
			ratingsRadioButtons[4] = FindViewById<RadioButton> (Resource.Id.radioButton_rating_5);
			durationEditText = FindViewById<EditText> (Resource.Id.editText_maxDuration);
			distanceEditText = FindViewById<EditText> (Resource.Id.editText_maxDistance);
			updateSearchResultsButton = FindViewById<Button> (Resource.Id.button_updateSearchResults);
			searchResultsGrid = FindViewById<GridLayout> (Resource.Id.gridLayout_searchResults);
			//viewTrailButton = FindViewById<Button> (Resource.Id.button_viewTrail);

			Trail[] debugTrails = new Trail[3];
			Activity[] debugActivities = new Activity[3];
			Amenity[] debugAmenities = new Amenity[2];

			if (debugSearchResults) {
				debugTrails[0] = new Trail(0, 0, 0, "Edgewater Trail", "BC", "", "", "66.69", "10", "Lorem ipsum", "three steps north, then turn right", Difficulty.Easiest, 4, "falling rocks", "", "", "A work bee is planned for 15 July 2014, weather pending. Some sections of the trail may be closed. Please come out with your shovels and rakes from 10am-1pm and enjoy a bbq afterwards.", "December through January", true, true, DateTime.Now);
				debugTrails[1] = new Trail(0, 0, 0, "Niles", "BC", "", "", "113.12", "20", "dolores umbridge", "Go to Neverland", Difficulty.Very_Difficult, 3, "", "", "", "", "All year", true, true, DateTime.Now);
				debugTrails[2] = new Trail(0, 0, 0, "Findlay Creek Trail 2", "AB", "", "", "0.59", "0.45", "Presumably, there's a Findlay Creek Trail 1, but this isn't it", "1337 d1r3c710n5", Difficulty.Extremely_Difficult, 5, "", "", "", "rwerwer", "Summer", true, true, DateTime.Now);

				debugActivities [0] = new Activity (1, "Hiking", "images/activities/activity-hike.png", DateTime.Now);
				debugActivities[1] = new Activity(2, "Mountain Biking", "images/activities/activity-bike.png", DateTime.Now);
				debugActivities [2] = new Activity (3, "Skiing", "", DateTime.Now);

				debugAmenities [0] = new Amenity (1, "restrooms", "images/amenities/restrooms_32.png", DateTime.Now);
				debugAmenities[1] = new Amenity(2, "camping", "images/amenities/camping_32.png", DateTime.Now);

				activityCheckBoxes = new CheckBox[debugActivities.Length];
				amenityCheckBoxes = new CheckBox[debugAmenities.Length];

				this.setSearchResults(debugTrails);
			}

			for (int i = 0; i < activityCheckBoxes.Length; i++) {
				activityCheckBoxes [i] = new CheckBox (this);
				if (debugSearchResults) {
					activityCheckBoxes [i].Text = debugActivities [i].activityName;
					activityCheckBoxes [i].Hint = debugActivities [i].id.ToString();
				}
				activityOptions.AddView (activityCheckBoxes [i]);
			}

			for (int i = 0; i < amenityCheckBoxes.Length; i++) {
				amenityCheckBoxes [i] = new CheckBox (this);
				if (debugSearchResults) {
					amenityCheckBoxes [i].Text = debugAmenities [i].amenityName;
					amenityCheckBoxes [i].Hint = debugAmenities [i].id.ToString();
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
			List<int> activitiesList = new List<int>();
			List<int> amenitiesList = new List<int> ();
			Difficulty difficulty = Difficulty.Easiest;
			int rating = 1;

			// TODO: Reference the database for activity and amenity enumerations.

			// Get the search filter parameters from the controls.
			// TODO: Get the min duration and distances.
			// TODO: Get the minimum star rating.
			for (int i = 0; i < activityCheckBoxes.Length; i++) {
				if (activityCheckBoxes[i] != null && activityCheckBoxes[i].Checked) {
					activitiesList.Add (int.Parse(activityCheckBoxes[i].Hint));
				}
			}

			for (int i = 0; i < amenityCheckBoxes.Length; i++) {
				if (amenityCheckBoxes[i] != null && amenityCheckBoxes[i].Checked) {
					amenitiesList.Add (int.Parse(amenityCheckBoxes[i].Hint));
				}
			}
			if (easiestRadioButton != null && easiestRadioButton.Checked == true) {
				difficulty = Difficulty.Easiest;
			}
			if (easyRadioButton != null && easyRadioButton.Checked == true) {
				difficulty = Difficulty.Easy;
			}
			if (moreDifficultRadioButton != null && moreDifficultRadioButton.Checked == true) {
				difficulty = Difficulty.More_Difficult;
			}
			if (veryDifficultRadioButton!= null && veryDifficultRadioButton.Checked == true) {
				difficulty = Difficulty.Very_Difficult;
			}
			if (extremelyDifficultRadioButton != null && extremelyDifficultRadioButton.Checked == true) {
				difficulty = Difficulty.Extremely_Difficult;
			}
			for (int i = 0; i < ratingsRadioButtons.Length; i++) {
				if (ratingsRadioButtons[i] != null && ratingsRadioButtons[i].Checked) {
					rating = i + 1;
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
			SearchFilter searchFilter = new SearchFilter(activitiesList.ToArray(), amenitiesList.ToArray(), difficulty, rating, minDuration, maxDuration, minDistance, maxDistance);

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
						trailElements[0].Text = trail.name;
//
						// TODO: Display the trail rating.
						trailElements[1] = new TextView (this);
						string ratingStars = "";
						for (int i = 0; i < trail.rating; i++) {
							ratingStars += "*";
						}
						trailElements[1].Text = ratingStars;
//
//						// TODO: Display the trail difficulty.
						trailElements[2] = new TextView (this);
						trailElements[2].Text = trail.difficulty.ToString().Replace("_", " ");
//
//						// TODO: Display the trail distance.
						trailElements[3] = new TextView (this);
						trailElements[3].Text = trail.distance + " km";

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