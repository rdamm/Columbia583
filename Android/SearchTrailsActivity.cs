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
using Android.Graphics;


namespace Columbia583.Android
{
	[Activity (Label = "Columbia583.Android_Search_Trails", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
	public class SearchTrailsActivity : AndroidActivity
	{
		// Define some structs for iterating through the checkboxes.
		protected class CheckboxToActivity
		{
			public Activity activity { get; set; }
			public CheckBox checkbox { get; set; }

			public CheckboxToActivity(Activity activity, CheckBox checkbox)
			{
				this.activity = activity;
				this.checkbox = checkbox;
			}
		}
		protected class CheckboxToAmenity
		{
			public Amenity amenity { get; set; }
			public CheckBox checkbox { get; set; }

			public CheckboxToAmenity(Amenity amenity, CheckBox checkbox)
			{
				this.amenity = amenity;
				this.checkbox = checkbox;
			}
		}

		private const int activityDialog = 1;
		private const int amenityDialog = 2;

		// Activities.
		//protected LinearLayout activityOptions = null;
		//protected List<CheckboxToActivity> activityCheckBoxes = null;
		String[] activitiesSelected;
		List<string> activities_check_list = new List<string> ();
		List<string> activitiesList_String = new List<string> ();
		List<int> activitiesList_ID = new List<int> ();

		// Amenities.
		//protected LinearLayout amenityOptions = null;
		//protected ScrollView amenityOptionsScroll = null;
		//protected List<CheckboxToAmenity> amenityCheckBoxes = null;
		String[] amenitiesSelected;
		List<string> amenities_check_list = new List<string> ();
		List<string> amenitiesList_String = new List<string> ();
		List<int> amenitiesList_ID = new List<int> ();

		// Difficulties.
		protected CheckBox easiestRadioButton = null;
		protected CheckBox easyRadioButton = null;
		protected CheckBox moreDifficultRadioButton = null;
		protected CheckBox veryDifficultRadioButton = null;
		protected CheckBox extremelyDifficultRadioButton = null;

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
		protected bool debugSearch = true;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetSearchTrailsPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.SearchTrails);

			// Get the controls.
			//activityOptions = FindViewById<LinearLayout> (Resource.Id.activityOptions);
			//amenityOptions = FindViewById<LinearLayout> (Resource.Id.amenityOptions);
			//amenityOptionsScroll = FindViewById<ScrollView> (Resource.Id.amenityOptionsScroll);
			easiestRadioButton = FindViewById<CheckBox> (Resource.Id.checkBox_difficulty_easiest);
			easyRadioButton = FindViewById<CheckBox> (Resource.Id.checkBox_difficulty_easy);
			moreDifficultRadioButton = FindViewById<CheckBox> (Resource.Id.checkBox_difficulty_moreDifficult);
			veryDifficultRadioButton = FindViewById<CheckBox> (Resource.Id.checkBox_difficulty_veryDifficult);
			extremelyDifficultRadioButton = FindViewById<CheckBox> (Resource.Id.checkBox_difficulty_extremelyDifficult);
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

			Application_Layer_Search_Trails applicationLayer_searchTrails = new Application_Layer_Search_Trails ();
			ListableTrail[] debugSearchResults = applicationLayer_searchTrails.getTrailsBySearchFilter (new SearchFilter (){ rating = 1 });
			//List<SearchResult> debugSearchResults = dataLayer.getTrailsBySearchFilter (new SearchFilter (){ rating = 1 });

			// Get the activities and amenities.
			Data_Layer_Common dataLayer2 = new Data_Layer_Common ();
			List<Activity> debugActivities = dataLayer2.getActivities ();
			List<Amenity> debugAmenities = dataLayer2.getAmenities ();

			foreach (Activity activity in debugActivities) 
			{
				activities_check_list.Add(activity.activityName);
			}

			foreach (Amenity amenity in debugAmenities) 
			{
				amenities_check_list.Add(amenity.amenityName);
			}

			var activitiesButton = FindViewById<Button>(Resource.Id.activitiesButton);
			activitiesButton.Click += delegate { ShowDialog(activityDialog); };

			var amenitiesButton = FindViewById<Button>(Resource.Id.amentiesButton);
			amenitiesButton.Click += delegate { ShowDialog(amenityDialog); };

			if (debugSearch) {

				/*debugTrails[0] = new Trail(0, 0, 0, "Edgewater Trail", "BC", "", "", "66.69", "10", "Lorem ipsum", "three steps north, then turn right", Difficulty.Easiest, 4, "falling rocks", "", "", "A work bee is planned for 15 July 2014, weather pending. Some sections of the trail may be closed. Please come out with your shovels and rakes from 10am-1pm and enjoy a bbq afterwards.", "December through January", true, true, DateTime.Now);
				debugTrails[1] = new Trail(0, 0, 0, "Niles", "BC", "", "", "113.12", "20", "dolores umbridge", "Go to Neverland", Difficulty.Very_Difficult, 3, "", "", "", "", "All year", true, true, DateTime.Now);
				debugTrails[2] = new Trail(0, 0, 0, "Findlay Creek Trail 2", "AB", "", "", "0.59", "0.45", "Presumably, there's a Findlay Creek Trail 1, but this isn't it", "1337 d1r3c710n5", Difficulty.Extremely_Difficult, 5, "", "", "", "rwerwer", "Summer", true, true, DateTime.Now);

				debugActivities [0] = new Activity (1, "Hiking", new byte[0], DateTime.Now);
				debugActivities[1] = new Activity(2, "Mountain Biking", new byte[0], DateTime.Now);
				debugActivities [2] = new Activity (3, "Skiing", new byte[0], DateTime.Now);

				debugAmenities [0] = new Amenity (1, "restrooms", new byte[0], DateTime.Now);
				debugAmenities[1] = new Amenity(2, "camping", new byte[0], DateTime.Now);
				*/

				this.setSearchResults(debugSearchResults);
			}

			/*activityCheckBoxes = new List<CheckboxToActivity>();
			amenityCheckBoxes = new List<CheckboxToAmenity>();

			foreach (Activity activity in debugActivities)
			{
				// Create a new checkbox.
				CheckBox checkbox = new CheckBox (this);

				// Add this checkbox to the list of checkboxes.
				activityCheckBoxes.Add (new CheckboxToActivity (activity, checkbox));

				// Add the checkbox to the view.
				activityOptions.AddView (checkbox);

				// Show the checkbox's icon or name.
				if (activity.activityIcon != null)
				{
					// Create an image view for the image.
					ImageView imageView = new ImageView (this);

					// Create a bitmap from the image's byte array.
					Bitmap bitmap = BitmapFactory.DecodeByteArray(activity.activityIcon, 0, activity.activityIcon.Length);
					imageView.SetMinimumHeight (64);
					imageView.SetMinimumWidth (64);
					imageView.SetMaxHeight (64);
					imageView.SetMaxWidth (64);
					imageView.SetBackgroundColor (Color.White);
					imageView.SetImageBitmap(bitmap);

					// Add the icon to the view.
					activityOptions.AddView (imageView);
				}
				else
				{
					// Create a text view for the name.
					TextView textView = new TextView (this);

					// TODO: Set the text view's content.
					//textView.SetText (activity.activityName);

					// Add the text to the view.
					activityOptions.AddView (textView);
				}
			}

			foreach (Amenity amenity in debugAmenities)
			{
				// Create a new checkbox.
				CheckBox checkbox = new CheckBox (this);

				// Add this checkbox to the list of checkboxes.
				amenityCheckBoxes.Add (new CheckboxToAmenity (amenity, checkbox));

				// Add the checkbox to the view.
				amenityOptions.AddView (checkbox);

				// Show the checkbox's icon or name.
				if (amenity.amenityIcon != null)
				{
					// Create an image view for the image.
					ImageView imageView = new ImageView (this);

					// Create a bitmap from the image's byte array.
					Bitmap bitmap = BitmapFactory.DecodeByteArray(amenity.amenityIcon, 0, amenity.amenityIcon.Length);
					imageView.SetMinimumHeight (64);
					imageView.SetMinimumWidth (64);
					imageView.SetMaxHeight (64);
					imageView.SetMaxWidth (64);
					imageView.SetBackgroundColor (Color.White);
					imageView.SetImageBitmap(bitmap);

					// Add the icon to the view.
					amenityOptions.AddView (imageView);
				}
				else
				{
					// Create a text view for the name.
					TextView textView = new TextView (this);

					// TODO: Set the text view's content.
					//textView.SetText (amenity.amenityName);

					// Add the text to the view.
					amenityOptions.AddView (textView);
				}
			}*/

			// Assign an event handler to the update search results button.
			if (updateSearchResultsButton != null) {
				updateSearchResultsButton.Click += (sender, e) => {

					// Get the search filter parameters from the controls.
					SearchFilter searchFilter = this.getSearchFilter();

					// Get the search results.
					//List<SearchResult> results = dataLayer.getTrailsBySearchFilter(searchFilter);
					//Application_Layer_Search_Trails applicationLayer_searchTrails = new Application_Layer_Search_Trails ();
					ListableTrail[] trails = applicationLayer_searchTrails.getTrailsBySearchFilter (searchFilter);

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

		/*private void cancelClicked(object sender, DialogClickEventArgs args)
		{
			Dialog dialog = (AlertDialog)sender;

			dialog.Dispose ();
		}*/

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

		/** Get the search filters from the controls. **/

		protected SearchFilter getSearchFilter()
		{
			List<Difficulty> difficulty = new List<Difficulty>();
			int rating = 1;

			// TODO: Reference the database for activity and amenity enumerations.

			// Get the search filter parameters from the controls.
			// TODO: Get the min duration and distances.
			// TODO: Get the minimum star rating.
			/*foreach (CheckboxToActivity checkboxToActivity in activityCheckBoxes)
			{
				if (checkboxToActivity.checkbox != null && checkboxToActivity.checkbox.Checked)
				{
					activitiesList.Add (checkboxToActivity.activity.id);
				}
			}

			foreach (CheckboxToAmenity checkboxToAmenity in amenityCheckBoxes)
			{
				if (checkboxToAmenity.checkbox != null && checkboxToAmenity.checkbox.Checked)
				{
					amenitiesList.Add (checkboxToAmenity.amenity.id);
				}
			}*/

			if (easiestRadioButton != null && easiestRadioButton.Checked == true) {
				difficulty.Add(Difficulty.Easiest);
			}
			if (easyRadioButton != null && easyRadioButton.Checked == true) {
				difficulty.Add(Difficulty.Easy);
			}
			if (moreDifficultRadioButton != null && moreDifficultRadioButton.Checked == true) {
				difficulty.Add(Difficulty.More_Difficult);
			}
			if (veryDifficultRadioButton!= null && veryDifficultRadioButton.Checked == true) {
				difficulty.Add(Difficulty.Very_Difficult);
			}
			if (extremelyDifficultRadioButton != null && extremelyDifficultRadioButton.Checked == true) {
				difficulty.Add(Difficulty.Extremely_Difficult);
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
			SearchFilter searchFilter=null; 
			//= new SearchFilter(activitiesList_ID.ToArray(), amenitiesList_ID.ToArray(), difficulty.ToArray(), rating, minDuration, maxDuration, minDistance, maxDistance);

			return searchFilter;
		}


		/**
		 * Display the search results in the grid.
		 * */
		protected void setSearchResults(ListableTrail[] searchResults)
		{
			// Display the trails in the view.
			if (searchResultsGrid != null) {
				// Empty the list.
				searchResultsGrid.RemoveAllViews();

				// Show a summary of each matching trail.
				if (searchResults != null) {
					foreach(ListableTrail searchResult in searchResults) {
						Trail trail = searchResult.trail;

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
								string activitiesJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(searchResult.activities);
								string amenitiesJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(searchResult.amenities);
								string pointsJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(searchResult.points);
								intent.PutExtra ("viewedTrail", trailJSONStr);
								intent.PutExtra("activities", activitiesJSONstr);
								intent.PutExtra("amenities", amenitiesJSONstr);
								intent.PutExtra("points", pointsJSONstr);

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