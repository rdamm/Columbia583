using Xamarin.Forms.Platform.Android;
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
using Android.Graphics;

namespace Columbia583.Android
{
	[Activity]			
	public class ExpandView : BaseExpandableListAdapter
	{
		private Dictionary<string, List<string> > _dictGroup =null;
		private List<string> _lstGroupID = null;
		private List<string> titles = null;
		private Activity _activity;			//calls Android.App.Activity, can't have a class named this.
		public string distance = "";
		public string duration= "";
		public Difficulty getDifficulty;
		public int getRating;
		public string hazards= "";
		public string surface= "";
		public string landAccess= "";
		public string maintenance= "";
		public string season= "";
		public string location="";
		public string name = "";
		public string directions = "";
		public string discription = "";
		//View convertView;


		public ExpandView (Activity act, Dictionary<string, List<string> > dictGroup, List<string> names)
		{
			_dictGroup = dictGroup;
			_activity = act;
			_lstGroupID = dictGroup.Keys.ToList();
			titles = names;
			//convertView = null;

		}

		#region implemented abstract members of BaseExpandableListAdapter
		public override Java.Lang.Object GetChild (int groupPosition, int childPosition)
		{
			return _dictGroup [_lstGroupID [groupPosition]] [childPosition];
		}

		public override long GetChildId (int groupPosition, int childPosition)
		{
			return childPosition;
		}

		public override int GetChildrenCount (int groupPosition)
		{
			return _dictGroup [_lstGroupID [groupPosition]].Count;
		}

		public View setRating(View convertView){
			if (convertView == null)
				convertView = _activity.LayoutInflater.Inflate (Resource.Layout.Child_Rating, null);

			var rating = convertView.FindViewById<RatingBar> (Resource.Id.ratingBar1);

			rating.Click += (object sender, EventArgs e) => {
				getRating = (int) rating.Rating;
			};

			return convertView;

		}

		public View setViewDifficulty(View convertView){
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			string[] names = dataLayer.getDifficulty ();
			//			List<Amenity> debugAmenities = dataLayer.getD();

			if (convertView == null)
				convertView = _activity.LayoutInflater.Inflate (Resource.Layout.Child_Difficulty, null);

			var text = convertView.FindViewById<TextView> (Resource.Id.textView1);
			var bar = convertView.FindViewById<SeekBar> (Resource.Id.seekBar1);
			//var Bartext = convertView.FindViewById<TextView> (Resource.Id.textView2);

			//text.Text = "Select Difficulty";

			bar.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
				if(e.FromUser){
					if(e.Progress <=20){
						text.Text = String.Format("You have selected easiest difficulty.");
						getDifficulty = Difficulty.Easiest;
					}
					else if(e.Progress <=40 && e.Progress > 20){
						text.Text = String.Format("You have selected easy difficulty.");
						getDifficulty = Difficulty.Easy;
					}
					else if(e.Progress <=60 && e.Progress > 40){
						text.Text = String.Format("You have selected more difficult difficulty.");
						getDifficulty = Difficulty.Extremely_Difficult;
					}
					else if(e.Progress <=80 && e.Progress > 60){
						text.Text = String.Format("You have selected very difficult difficulty.");
						getDifficulty = Difficulty.More_Difficult;
					}
					else if(e.Progress <=100 && e.Progress > 80){
						text.Text = String.Format("You have selected extremely difficult difficulty.");
						getDifficulty = Difficulty.Very_Difficult;
					}

				}
			};

			return convertView;
		}

		public View setDuration(View convertView){
			//var test = new System.Windows.Forms ();

			if (convertView == null)
				convertView = _activity.LayoutInflater.Inflate (Resource.Layout.Child_Distance, null);

			var minDistance = convertView.FindViewById<TextView> (Resource.Id.textView1);
			//var maxDistance = convertView.FindViewById<TextView> (Resource.Id.textView2);
			var minBar = convertView.FindViewById<SeekBar> (Resource.Id.seekBar1);
			//var maxBar = convertView.FindViewById<SeekBar> (Resource.Id.seekBar2);
			var showMin = convertView.FindViewById<TextView> (Resource.Id.textView3);
			//var showMax = convertView.FindViewById<TextView> (Resource.Id.textView5);

			minDistance.Text = "Duration";
			//.Text = "Maximum Duration";

			minBar.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
				if(e.FromUser){
					showMin.Text= String.Format("You have selected {0} as duration",e.Progress);
					duration= e.Progress.ToString();
				}
			};
				
			return convertView;
		}

		public View setDistance(View convertView){
			//var test = new System.Windows.Forms ();

			if (convertView == null)
				convertView = _activity.LayoutInflater.Inflate (Resource.Layout.Child_Distance, null);

			var minDistance = convertView.FindViewById<TextView> (Resource.Id.textView1);
			//var maxDistance = convertView.FindViewById<TextView> (Resource.Id.textView2);
			var minBar = convertView.FindViewById<SeekBar> (Resource.Id.seekBar1);
			//var maxBar = convertView.FindViewById<SeekBar> (Resource.Id.seekBar2);
			var showMin = convertView.FindViewById<TextView> (Resource.Id.textView3);
			//var showMax = convertView.FindViewById<TextView> (Resource.Id.textView5);

			minDistance.Text = "Distance";
			//maxDistance.Text = "Maximum Distance";

			minBar.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
				if(e.FromUser){
					showMin.Text = String.Format("You have selected {0} as minimum distance",e.Progress);
					distance = String.Format("{0}",e.Progress);
				}
			};
				

			return convertView;
		}

		public View getDescription(View convertView, int flag){
			if (convertView == null)
				convertView = _activity.LayoutInflater.Inflate (Resource.Layout.Child_Description, null);

			var description = convertView.FindViewById<MultiAutoCompleteTextView> (Resource.Id.multiAutoCompleteTextView1);

			if (flag == 1) {
				description.Hint = "Write the hazards encountered on the trail";
				hazards = description.Text;
			} else if (flag == 2) {
				description.Hint = "Write the surfaces in the trail";
				surface = description.Text;
			} else if (flag == 3) {
				description.Hint = "Is the land accessible?";
				landAccess = description.Text;
			} else if (flag == 4) {
				description.Hint = "Is there any maintenance on the trail?";
				maintenance = description.Text;
			} else if (flag == 5) {
				description.Hint = "What season is it?";
				season = description.Text;
			} else if (flag == 6) {
				description.Hint = "Write the location of the trail....";
				location = description.Text;
			} else if (flag == 7) {
				description.Hint = "Write the trail name....";
				name = description.Text;
			} else if (flag == 8) {
				description.Hint = "Write any details about the trail....";
				discription = description.Text;
			} else if (flag == 9) {
				description.Hint = "Write the directions to the trail....";
				directions = description.Text;
			}

			return convertView;
		}

		public override View GetChildView (int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{	
			convertView = null;

			int flag = -1;

			if (groupPosition == 0) {
				convertView = setViewDifficulty (convertView);
				//convertView = GetGroupView (groupPosition,false,convertView,parent);
			} else if (groupPosition == 1) {
				convertView = setRating (convertView);

			} else if (groupPosition == 2) {
				convertView = setDuration (convertView);

			}else if (groupPosition == 3) {
				convertView = setDistance (convertView);
			}else if (groupPosition == 4) {
				flag = 1;
				convertView = getDescription (convertView,flag);
			}else if (groupPosition == 5) {
				flag = 2;
				convertView = getDescription (convertView,flag);
			}else if (groupPosition == 6) {
				flag = 3;
				convertView = getDescription (convertView,flag);
			}else if (groupPosition == 7) {
				flag = 4;
				convertView = getDescription (convertView,flag);
			}else if (groupPosition == 8) {
				flag = 5;
				convertView = getDescription (convertView,flag);
			}else if (groupPosition == 9) {
				flag = 6;
				convertView = getDescription (convertView,flag);
			}else if (groupPosition == 10) {
				flag = 7;
				convertView = getDescription (convertView,flag);
			}else if (groupPosition == 11) {
				flag = 8;
				convertView = getDescription (convertView,flag);
			}else if (groupPosition == 12) {
				flag = 9;
				convertView = getDescription (convertView,flag);
			}else {
				Console.Out.WriteLine ("Error in selection");
				//Toast.MakeText (this, "Error in selection", ToastLength.Short).Show();
			}

			return convertView;
		}

		public override Java.Lang.Object GetGroup (int groupPosition)
		{
			return _lstGroupID [groupPosition];
		}

		public override long GetGroupId (int groupPosition)
		{
			return groupPosition;
		}

		public override View GetGroupView (int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			//			var item = _lstGroupID [groupPosition];
			//var item = _dictGroup [_lstGroupID [groupPosition]] [childPosition];
			View row = convertView;
			var item = titles [groupPosition];
			if (row == null)
				convertView = _activity.LayoutInflater.Inflate (Resource.Layout.ListControl_Group, null);
			//else if (convertView != null && groupPosition == 0)
			//	convertView = _activity.LayoutInflater.Inflate (Resource.Layout.ListControl_Group, parent);
			//writting text to the large text box. 

			var textBox = convertView.FindViewById<TextView> (Resource.Id.title);
			textBox.SetText (item, TextView.BufferType.Normal);

			return convertView;
		}

		public override bool IsChildSelectable (int groupPosition, int childPosition)
		{
			return true;
		}

		public override int GroupCount {
			get {
				return _dictGroup.Count;
			}
		}

		public override bool HasStableIds {
			get {
				return true;
			}
		}
		#endregion

	}
}

