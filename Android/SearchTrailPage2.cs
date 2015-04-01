using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Platform.Android;
using Newtonsoft.Json;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Columbia583.Android
{
	[Activity (Label = "SearchTrailPage2")]			
	public class SearchTrailPage2 : AndroidActivity
	{
		protected bool preventSpinnerSelectEventFiringOnCreate = true;

		Dictionary<string, List<string> > dictGroup = new Dictionary<string, List<string> > ();
		List<string> lstKeys = new List<string> ();

		protected ListableTrail[] trails = null;
		protected ListableTrail[] debugSearchResults = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.SearchTrailPage2);

			string getResult = Intent.GetStringExtra ("search") ?? "No filter found";
			SearchFilter result = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchFilter> (getResult);

			Data_Access_Layer_Search_Trails data_access_trails = new Data_Access_Layer_Search_Trails ();
			trails = data_access_trails.getTrailsBySearchFilter (result);

			List<string> names= new List<string>();

			if (trails.Length ==0 || trails == null) {
				Application_Layer_Search_Trails applicationLayer_searchTrails = new Application_Layer_Search_Trails ();
				debugSearchResults = applicationLayer_searchTrails.getTrailsBySearchFilter (new SearchFilter (){ rating = 1 });

				foreach(ListableTrail t in debugSearchResults){
					names.Add (t.trail.name);
				}
			} else {
				foreach(ListableTrail t in trails){
					names.Add (t.trail.name);
				}
			}

			var spinner = FindViewById<Spinner> (Resource.Id.Spinner);

			ArrayAdapter _adapterFrom = new ArrayAdapter (this, global::Android.Resource.Layout.SimpleSpinnerItem, names.ToArray());
			_adapterFrom.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = _adapterFrom; 

			int position=-1;

			spinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) => {

				if (preventSpinnerSelectEventFiringOnCreate == true)
				{
					preventSpinnerSelectEventFiringOnCreate = false;
					//position = spinner.SelectedItemPosition;
					return;
				}
				position = spinner.SelectedItemPosition;
				if(e.Position == position){


					String foundName = names.ElementAt(position);

					Console.Out.WriteLine(foundName);
					ListableTrail getTrail;

					if(trails.Length != 0)
						getTrail = trails[position];
					else
						getTrail = debugSearchResults[position];
					Console.Out.WriteLine(getTrail.trail.name);

					var intent = new Intent (this, typeof(ViewTrailActivity));
					string trailJSONStr = Newtonsoft.Json.JsonConvert.SerializeObject (getTrail.trail);
					string activitiesJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(getTrail.activities);
					string amenitiesJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(getTrail.amenities);
					string pointsJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(getTrail.points);
					intent.PutExtra ("viewedTrail", trailJSONStr);
					intent.PutExtra("activities", activitiesJSONstr);
					intent.PutExtra("amenities", amenitiesJSONstr);
					intent.PutExtra("points", pointsJSONstr);

					StartActivity (intent);

				}
			};
		}


	}
}

