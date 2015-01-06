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

			if (debugSearchResults) {
				Trail[] debugTrails = new Trail[3];
				debugTrails[0] = new Trail(0, 0, 0, "Edgewater Trail", "BC", "", "", "66.69 km", "10 h", "", "", Difficulty.Easiest, new Rating(4), "", "", "", "", true, true, DateTime.Now);
				debugTrails[1] = new Trail(0, 0, 0, "Niles", "BC", "", "", "113.12 km", "20 h", "", "", Difficulty.Very_Difficult, new Rating(3), "", "", "", "", true, true, DateTime.Now);
				debugTrails[2] = new Trail(0, 0, 0, "Findlay Creek Trail 2", "AB", "", "", "0.59 km", "0.45 h", "", "", Difficulty.Extremely_Difficult, new Rating(5), "", "", "", "", true, true, DateTime.Now);
				this.setSearchResults(debugTrails);
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
			List<Difficulty> difficultiesList = new List<Difficulty>();
			List<string> activitiesList = new List<string>();

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
			SearchFilter searchFilter = new SearchFilter(activitiesList.ToArray(), difficultiesList.ToArray(), minDuration, maxDuration, minDistance, maxDistance);

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
						TextView trailName = new TextView (this);
						trailName.Text = trail.Name;
						searchResultsGrid.AddView(trailName);
//
						// TODO: Display the trail rating.
						TextView rating = new TextView (this);
						rating.Text = trail.Rating.RatingStars;
						searchResultsGrid.AddView(rating);
//
//						// TODO: Display the trail difficulty.
						TextView difficulty = new TextView (this);
						difficulty.Text = trail.Difficulty.ToString().Replace("_", " ");
						searchResultsGrid.AddView(difficulty);
//
//						// TODO: Display the trail distance.
						TextView distance = new TextView (this);
						distance.Text = trail.Distance;
						searchResultsGrid.AddView(distance);
					}
				}
			}
		}
	}
}