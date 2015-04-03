
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
	[Activity (Label = "FavouriteTrailsActivity")]			
	public class FavouriteTrailsActivity : AndroidActivity
	{
		protected GridLayout gridLayoutFavouriteTrails = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.FavouriteTrails);

			// Get the controls.
			gridLayoutFavouriteTrails = FindViewById<GridLayout>(Resource.Id.gridLayout_favouriteTrails);

			// Get the active user.
			Data_Access_Layer_Common dataAccessLayerCommon = new Data_Access_Layer_Common();
			User activeUser = dataAccessLayerCommon.getActiveUser();

			// If there is an active user, get their favourite trails.
			ListableTrail[] favouriteTrails = new ListableTrail[0];
			if (activeUser != null)
			{
				Data_Access_Layer_Favourites dataAccessLayerFavourites = new Data_Access_Layer_Favourites ();
				favouriteTrails = dataAccessLayerFavourites.getFavouriteTrails (activeUser.id);
			}

			// Display the user's favourite trails.
			displayFavouriteTrails(favouriteTrails);
		}


		/// <summary>
		/// Displays the favourite trails.
		/// </summary>
		/// <param name="favouriteTrails">Favourite trails.</param>
		protected void displayFavouriteTrails(ListableTrail[] favouriteTrails)
		{
			if (gridLayoutFavouriteTrails != null)
			{
				// Empty the list.
				gridLayoutFavouriteTrails.RemoveAllViews();

				// Display each trail.
				if (favouriteTrails != null)
				{
					foreach(ListableTrail favouriteTrail in favouriteTrails)
					{
						// Get the trail's details.
						Trail trail = favouriteTrail.trail;
						Activity[] activities = favouriteTrail.activities;
						Amenity[] amenities = favouriteTrail.amenities;
						Point[] points = favouriteTrail.points;

						// Create a list to populate with this row's views.
						List<TextView> trailViewElements = new List<TextView> ();

						// Display the trail's name.
						TextView nameView = new TextView(this);
						nameView.Text = trail.name;
						trailViewElements.Add (nameView);

						// Display the trail's rating.
						TextView ratingView = new TextView (this);
						string ratingStars = "";
						for (int i = 0; i < trail.rating; i++)
						{
							ratingStars += "*";
						}
						ratingView.Text = ratingStars;
						trailViewElements.Add (ratingView);

						// Display the trail's difficulty.
						TextView difficultyView = new TextView (this);
						difficultyView.Text = trail.difficulty.ToString().Replace("_", " ");
						trailViewElements.Add (difficultyView);

						// Display the trail's distance.
						TextView distanceView = new TextView (this);
						distanceView.Text = trail.distance + " km";
						trailViewElements.Add (distanceView);

						foreach(TextView trailViewElement in trailViewElements)
						{
							// Set the event handlers.
							trailViewElement.Click += (sender, e) => {

								// Load the view trail page.
								var intent = new Intent (this, typeof(ViewTrailActivity));
								string trailJSONStr = Newtonsoft.Json.JsonConvert.SerializeObject (trail);
								string activitiesJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(activities);
								string amenitiesJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(amenities);
								string pointsJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(points);
								intent.PutExtra ("viewedTrail", trailJSONStr);
								intent.PutExtra("activities", activitiesJSONstr);
								intent.PutExtra("amenities", amenitiesJSONstr);
								intent.PutExtra("points", pointsJSONstr);
								StartActivity (intent);

							};

							// Add the element to the view.
							gridLayoutFavouriteTrails.AddView (trailViewElement);
						}
					}
				}
			}
		}
	}
}

