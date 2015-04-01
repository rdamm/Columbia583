
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
	[Activity (Label = "UploadTrail")]			
	public class UploadTrail : AndroidActivity
	{
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

		private string distance = "";
		private string duration= "";
		private Difficulty getDifficulty;
		private int getRating;
		private string hazards= "";
		private string surface= "";
		private string landAccess= "";
		private string maintenance= "";
		private string season= "";
		private string location="";
		private string name = "";
		private string directions = "";
		private string discription = "";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.UploadTrail);

			var selectActivity = FindViewById<Button> (Resource.Id.acitvity);
			var selectAmenity = FindViewById<Button> (Resource.Id.amenity);
			var upload = FindViewById<Button> (Resource.Id.upload);

			var diffText = FindViewById<TextView> (Resource.Id.difficultyText);
			var textDist = FindViewById<TextView> (Resource.Id.distance);
			var textDurat = FindViewById<TextView> (Resource.Id.duration);

			var difficulty = FindViewById<SeekBar> (Resource.Id.seekBar1);
			var Tdistance = FindViewById<SeekBar> (Resource.Id.seekBar2);
			var Tduration = FindViewById<SeekBar> (Resource.Id.seekBar3);
			var rating = FindViewById<RatingBar> (Resource.Id.ratingBar1);
			var trailName = FindViewById<MultiAutoCompleteTextView> (Resource.Id.multiAutoCompleteTextView1);
			var Tdirections = FindViewById<MultiAutoCompleteTextView> (Resource.Id.multiAutoCompleteTextView2);
			var Tdescription = FindViewById<MultiAutoCompleteTextView> (Resource.Id.multiAutoCompleteTextView3);
			var Thazards = FindViewById<MultiAutoCompleteTextView> (Resource.Id.multiAutoCompleteTextView4);
			var Tseason = FindViewById<MultiAutoCompleteTextView> (Resource.Id.multiAutoCompleteTextView5);
			var Tlocation = FindViewById<MultiAutoCompleteTextView> (Resource.Id.multiAutoCompleteTextView6);
			var Tmaintenance = FindViewById<MultiAutoCompleteTextView> (Resource.Id.multiAutoCompleteTextView7);
			var TlandAccess = FindViewById<MultiAutoCompleteTextView> (Resource.Id.multiAutoCompleteTextView8);
			var Tsurface = FindViewById<MultiAutoCompleteTextView> (Resource.Id.multiAutoCompleteTextView9);

			var Topen = FindViewById<RadioButton> (Resource.Id.radioButton1);
			var Tclosed = FindViewById<RadioButton> (Resource.Id.radioButton2);
			var Tactive = FindViewById<RadioButton> (Resource.Id.radioButton3);
			var Tunactive = FindViewById<RadioButton> (Resource.Id.radioButton4);

			Data_Layer_Common dataLayer2 = new Data_Layer_Common ();
			List<Activity> debugActivities = dataLayer2.getActivities ();
			List<Amenity> debugAmenities = dataLayer2.getAmenities ();

			foreach (Activity act in debugActivities) 
			{
				activities_check_list.Add(act.activityName);
			}

			foreach (Amenity amenity in debugAmenities) 
			{
				amenities_check_list.Add(amenity.amenityName);
			}

			selectActivity.Click += delegate {
				ShowDialog(activityDialog);
			};

			selectAmenity.Click += delegate {
				ShowDialog(amenityDialog);
			};

			difficulty.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
				Data_Layer_Common dataLayer = new Data_Layer_Common ();
				string[] names = dataLayer.getDifficulty ();

				if(e.FromUser){
					if(e.Progress <=20){
						diffText.Text = String.Format("You have selected easiest difficulty.");
						getDifficulty = Difficulty.Easiest;
					}
					else if(e.Progress <=40 && e.Progress > 20){
						diffText.Text = String.Format("You have selected easy difficulty.");
						getDifficulty = Difficulty.Easy;
					}
					else if(e.Progress <=60 && e.Progress > 40){
						diffText.Text = String.Format("You have selected more difficult difficulty.");
						getDifficulty = Difficulty.More_Difficult;
					}
					else if(e.Progress <=80 && e.Progress > 60){
						diffText.Text = String.Format("You have selected very difficult difficulty.");
						getDifficulty = Difficulty.Very_Difficult;
					}
					else if(e.Progress <=100 && e.Progress > 80){
						diffText.Text = String.Format("You have selected extremely difficult difficulty.");
						getDifficulty = Difficulty.Extremely_Difficult;
					}

				}
			};

			rating.Click += (object sender, EventArgs e) => {
				getRating = (int) rating.Rating;
			};

			Tduration.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
				if(e.FromUser){
					textDurat.Text= String.Format("You have selected {0} as duration",e.Progress);
					duration= e.Progress.ToString();
				}
			};

			Tdistance.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
				if(e.FromUser){
					textDist.Text = String.Format("You have selected {0} as minimum distance",e.Progress);
					distance = String.Format("{0}",e.Progress);
				}
			};


			Topen.Click += (object sender, EventArgs e) => {
				getOpen = true;
			};

			Tclosed.Click += (object sender, EventArgs e) => {
				getOpen = false;
			};

			Tactive.Click += (object sender, EventArgs e) => {
				getActive = true;
			};

			Tunactive.Click += (object sender, EventArgs e) => {
				getActive = false;
			};


			upload.Click += (object sender, EventArgs e) => {
				getRating = (int) rating.Rating;
				hazards = Thazards.Text;
				surface = Tsurface.Text;
				landAccess = TlandAccess.Text;
				name = trailName.Text;
				discription = Tdescription.Text;
				directions = Tdirections.Text;
				season = Tseason.Text;
				location = Tlocation.Text;

				Data_Access_Layer_Common data_access_layer = new Data_Access_Layer_Common();
				User getUser = data_access_layer.getActiveUser();

				Random num = new Random();
				int getnum = num.Next();
				Trail trail = new Trail(getnum,getUser.id ,getUser.orgId,name,location,"","",distance,duration,discription,directions, getDifficulty, getRating,hazards,surface,landAccess,maintenance,season, getOpen, getActive,DateTime.Now,DateTime.Now,true);
				data_access_layer.insertTrail(trail);
				//Console.Out.WriteLine(myView.getRating);
				//Console.Out.WriteLine(myView.getDifficulty);

				Trail list_trail = data_access_layer.getTrailID(getnum);
				Console.Out.WriteLine(list_trail.directions);
				Console.Out.WriteLine(list_trail.difficulty);

				//				foreach(var i in list_comments){
				//					Console.Out.WriteLine(i.text);
				//				}

				Toast.MakeText(this, "Trail has been uploaded", ToastLength.Short).Show();


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

	}
}

