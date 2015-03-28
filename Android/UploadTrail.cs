
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;


namespace Columbia583.Android
{
	[Activity (Label = "UploadTrail")]			
	public class UploadTrail : AndroidActivity
	{
		Dictionary<string, List<string> > dictGroup = new Dictionary<string, List<string> > ();
		List<string> lstKeys = new List<string> ();
		private const int activityDialog = 1;
		private const int amenityDialog = 2;

		String[] activitiesSelected;
		List<string> activities_check_list = new List<string> ();
		List<string> activitiesList_String = new List<string> ();
		List<int> activitiesList_ID = new List<int> ();
		bool getOpen, getActive;
		// Amenities.
		//protected LinearLayout amenityOptions = null;
		//protected ScrollView amenityOptionsScroll = null;
		//protected List<CheckboxToAmenity> amenityCheckBoxes = null;
		String[] amenitiesSelected;
		List<string> amenities_check_list = new List<string> ();
		List<string> amenitiesList_String = new List<string> ();
		List<int> amenitiesList_ID = new List<int> ();
		ExpandView myView= null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.UploadTrail);

			CreateExpendableListData ();
			List<string> titles = new List<string>();
			titles.Add ("Select Difficulty");
			titles.Add ("Select Rating");
			titles.Add ("Select Duration");
			titles.Add ("Select Distance");
			titles.Add ("Hazards");
			titles.Add ("Surface");
			titles.Add ("Land Access");
			titles.Add ("Maintenance");
			titles.Add ("Season");
			titles.Add ("Location");
			titles.Add ("Trail Name");
			titles.Add ("Trail Description");
			titles.Add ("Directions to trail");

			var selectActivity = FindViewById<Button> (Resource.Id.button1);
			var selectAmenity = FindViewById<Button> (Resource.Id.button2);
			var upload = FindViewById<Button> (Resource.Id.button3);
			var open = FindViewById<RadioButton> (Resource.Id.radioButton1);
			var closed = FindViewById<RadioButton> (Resource.Id.radioButton2);
			var active = FindViewById<RadioButton> (Resource.Id.radioButton3);
			var unactive = FindViewById<RadioButton> (Resource.Id.radioButton4);

			var ctlExListBox = FindViewById<ExpandableListView> (Resource.Id.expandableListView1);
			ctlExListBox.SetAdapter (
				myView = new ExpandView (this, dictGroup, titles)

			);

			//			imageButton.SetAdapter(new ExpandList(this, dictGroup,titles));


			ctlExListBox.ChildClick += delegate(object sender, ExpandableListView.ChildClickEventArgs e) {
				var itmGroup = lstKeys [e.GroupPosition];
				var itmChild = dictGroup [itmGroup] [e.ChildPosition];

				//Toast.MakeText (this, string.Format ("You Click on Group {0} with child {1}", itmGroup, itmChild), ToastLength.Long).Show ();			
			};

			Data_Layer_Common dataLayer2 = new Data_Layer_Common ();
			List<activity> debugActivities = dataLayer2.getActivities ();
			List<Amenity> debugAmenities = dataLayer2.getAmenities ();

			foreach (activity act in debugActivities) 
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

			open.Click += (object sender, EventArgs e) => {
				getOpen = true;
			};

			closed.Click += (object sender, EventArgs e) => {
				getOpen = false;
			};

			active.Click += (object sender, EventArgs e) => {
				getActive = true;
			};

			unactive.Click += (object sender, EventArgs e) => {
				getActive = false;
			};

			upload.Click += (object sender, EventArgs e) => {
				Data_Access_Layer_Common data_access_layer = new Data_Access_Layer_Common();
				Random num = new Random();

				Trail trail = new Trail(1000, 2,2,myView.name, myView.location,"","",myView.distance,myView.duration,myView.discription,myView.directions,myView.getDifficulty,myView.getRating,myView.hazards,myView.surface,myView.landAccess,myView.maintenance,myView.season,getOpen,getActive,DateTime.Now,DateTime.Now,true);
				data_access_layer.insertTrail(trail);
				//Console.Out.WriteLine(myView.getRating);
				//Console.Out.WriteLine(myView.getDifficulty);

				Trail list_trail = data_access_layer.getTrailID(1000);
				Console.Out.WriteLine(list_trail.description);
				Console.Out.WriteLine(list_trail.difficulty);

//				foreach(var i in list_comments){
//					Console.Out.WriteLine(i.text);
//				}


			};
		}

		void CreateExpendableListData ()
		{
			for (int iGroup = 1; iGroup <= 13; iGroup++) {
				var lstChild = new List<string> ();
				for (int iChild = 1; iChild <= 1; iChild++) {
					lstChild.Add (string.Format ("Group {0} Child {1}", iGroup, iChild));
				}
				dictGroup.Add (string.Format ("Group {0}", iGroup), lstChild);
			}

			lstKeys = new List<string> (dictGroup.Keys);
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

