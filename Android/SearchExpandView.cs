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
	public class SearchExpandView : BaseExpandableListAdapter
	{
		private Dictionary<string, List<string> > _dictGroup =null;
		private List<string> _lstGroupID = null;
		private List<string> titles = null;
		private Activity _activity;			//calls Android.App.Activity, can't have a class named this.
		//View convertView;


		public SearchExpandView (Activity act, Dictionary<string, List<string> > dictGroup, List<string> names)
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

		public View getCustomView(View convertView){

			if (convertView == null)
				convertView = _activity.LayoutInflater.Inflate (Resource.Layout.ListControl_ChildItem, null);

			return convertView;
		}

		public override View GetChildView (int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{	
			convertView = null;

			convertView = getCustomView (convertView);
			var name = convertView.FindViewById<TextView> (Resource.Id.nameText);

			foreach(String n in titles){
				name.Text = n;
			}
			name.Text = "hello";
			name.Text = "yes";

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
						//var item = _lstGroupID [groupPosition];
			//var item = _dictGroup [_lstGroupID [groupPosition]];
			//View row = convertView;
			var item = titles [groupPosition];
			if (convertView == null)
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

