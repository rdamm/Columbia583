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

		// Difficulties.
		protected CheckBox easiestCheckBox = null;
		protected CheckBox easyCheckBox = null;
		protected CheckBox moreDifficultCheckBox = null;
		protected CheckBox veryDifficultCheckBox = null;
		protected CheckBox extremelyDifficultCheckBox = null;

		// Duration.
		protected EditText durationEditText = null;

		// Distance.
		protected EditText distanceEditText = null;

		// Other controls.
		protected Button updateSearchResultsButton = null;
		protected GridLayout searchResultsGrid = null;
		protected Button viewTrailButton = null;

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
			easiestCheckBox = FindViewById<CheckBox> (Resource.Id.checkBox_difficulty_easiest);
			easyCheckBox = FindViewById<CheckBox> (Resource.Id.checkBox_difficulty_easy);
			moreDifficultCheckBox = FindViewById<CheckBox> (Resource.Id.checkBox_difficulty_moreDifficult);
			veryDifficultCheckBox = FindViewById<CheckBox> (Resource.Id.checkBox_difficulty_veryDifficult);
			extremelyDifficultCheckBox = FindViewById<CheckBox> (Resource.Id.checkBox_difficulty_extremelyDifficult);
			durationEditText = FindViewById<EditText> (Resource.Id.editText_maxDuration);
			distanceEditText = FindViewById<EditText> (Resource.Id.editText_maxDistance);
			updateSearchResultsButton = FindViewById<Button> (Resource.Id.button_updateSearchResults);
			searchResultsGrid = FindViewById<GridLayout> (Resource.Id.gridLayout_searchResults);
			viewTrailButton = FindViewById<Button> (Resource.Id.button_viewTrail);

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

			// Assign an event handler to the view trail button.
			if (viewTrailButton != null) {
				viewTrailButton.Click += (sender, e) => {

					// Load the view trail page.
					var intent = new Intent(this, typeof(ViewTrailActivity));
					StartActivity(intent);

				};
			}
		}


		/**
		 * Get the search filters from the controls.
		 * */
		protected SearchFilter getSearchFilter()
		{
			List<int> activitiesList = new List<int>();
			List<int> amenitiesList = new List<int> ();

			// TODO: Reference the database for activity and amenity enumerations.
			int hikingEnum = 1;
			int skiingEnum = 2;
			int bikingEnum = 3;

			// Get the search filter parameters from the controls.
			// TODO: Get the min duration and distances.
			// TODO: Get the minimum star rating.
			if (hikingCheckBox != null && hikingCheckBox.Checked == true) {
				activitiesList.Add(hikingEnum);
			}
			if (skiingCheckBox != null && skiingCheckBox.Checked == true) {
				activitiesList.Add(skiingEnum);
			}
			if (bikingCheckBox != null && bikingCheckBox.Checked == true) {
				activitiesList.Add(bikingEnum);
			}
			Difficulty difficulty = Difficulty.Easiest;
			if (easiestCheckBox != null && easiestCheckBox.Checked == true) {
				difficulty = Difficulty.Easiest;
			}
			if (easyCheckBox != null && easyCheckBox.Checked == true) {
				difficulty = Difficulty.Easy;
			}
			if (moreDifficultCheckBox != null && moreDifficultCheckBox.Checked == true) {
				difficulty = Difficulty.More_Difficult;
			}
			if (veryDifficultCheckBox != null && veryDifficultCheckBox.Checked == true) {
				difficulty = Difficulty.Very_Difficult;
			}
			if (extremelyDifficultCheckBox != null && extremelyDifficultCheckBox.Checked == true) {
				difficulty = Difficulty.Extremely_Difficult;
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
			int rating = 1;

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
//						// TODO: Display the trail name.
//						searchResultsGrid.AddView(new TextView {
//							Text = trail.Name
//						});
//
//						// TODO: Display the trail rating.
//						searchResultsGrid.AddView(new TextView {
//							Text = ""
//						});
//
//						// TODO: Display the trail difficulty.
//						searchResultsGrid.AddView(new TextView {
//							Text = trail.Difficulty
//						});
//
//						// TODO: Display the trail distance.
//						searchResultsGrid.AddView(new TextView {
//							Text = trail.Distance
//						});
					}
				}
			}
		}
	}
}