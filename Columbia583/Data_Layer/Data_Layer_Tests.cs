using System;
using System.Collections.Generic;

using System.Net;
using System.IO;
using Android.Graphics;

namespace Columbia583
{
	/// <summary>
	/// Data layer tests is a series of sample methods that can be called to determine whether or not the database
	/// queries are functioning correctly.
	/// </summary>
	public class Data_Layer_Tests
	{
		public Data_Layer_Tests ()
		{

		}


		/// <summary>
		/// Creates the tables.
		/// </summary>
		/// <returns>The tables.</returns>
		public string createTables()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			dataLayer.createTables ();

			return "Successfully created tables.";
		}


		/// <summary>
		/// Inserts some sample data into the into tables.
		/// </summary>
		/// <returns>The into tables.</returns>
		public string insertIntoTables()
		{
			byte[] imageBytes = null;
			try
			{
				// Get a sample image from the web.
				WebClient client = new WebClient();
				imageBytes = client.DownloadData("http://www.clker.com/cliparts/3/d/3/7/12065718151966625209johnny_automatic_NPS_map_pictographs_part_96.svg.hi.png");
			}
			catch (WebException e) {
				Console.WriteLine ("Failed to get sample image to insert.  Defaulting to null.");
			}

			// Create rows without foreign keys.
			Activity[] activities = new Activity[3];
			Amenity[] amenities = new Amenity[3];
			MapTile[] mapTiles = new MapTile[1];
			Organization[] organizations = new Organization[1];
			Role[] roles = new Role[1];
			activities [0] = new Activity (1, "Hiking", imageBytes, DateTime.Now);
			activities [1] = new Activity (2, "Biking", new byte[0], DateTime.Now);
			activities [2] = new Activity (3, "Skiing", new byte[0], DateTime.Now);
			amenities [0] = new Amenity (1, "Washrooms", new byte[0], DateTime.Now);
			amenities [1] = new Amenity (2, "Campground", new byte[0], DateTime.Now);
			amenities [2] = new Amenity (3, "Picnic Area", new byte[0], DateTime.Now);
			mapTiles [0] = new MapTile (1, 100, 200, 10, 20, "mapTile_100x200_10x20.jpg", new byte[0], DateTime.Now, DateTime.Now);
			organizations [0] = new Organization (1, "Columbia Valley Greenways Trail Alliance", DateTime.Now);
			roles [0] = new Role (1, "Administrator", DateTime.Now);

			// Create rows with foreign keys.
			// TODO: Figure out how to nullify the next point ID field.
			User[] users = new User[1];
			Trail[] trails = new Trail[3];
			Media[] media = new Media[1];
			Point[] points = new Point[6];
			TrailsToActivities[] trailsToActivities = new TrailsToActivities[4];
			TrailsToAmenities[] trailsToAmenities = new TrailsToAmenities[4];
			FavouriteTrails[] favouriteTrails = new FavouriteTrails[0];
			users [0] = new User (1, 0, "rob_orchiston@greenways.ca", "rob_orchiston", DateTime.Now, DateTime.Now, false);
			trails [0] = new Trail (1, 1, 1, "Trail 1", "Radium", "", "", "32 km", "2 hours", "A hike through the valley.", "", Difficulty.More_Difficult, 4, "", "", "", "", "", true, true, DateTime.Now,DateTime.Now,true);
			trails [1] = new Trail (2, 1, 1, "Trail 2", "Invermere", "", "", "16 km", "1 hours", "A hike along the creeks.", "", Difficulty.Easy, 2, "", "", "", "", "", true, true, DateTime.Now,DateTime.Now,true);
			trails [2] = new Trail (3, 1, 1, "Trail 3", "Windermere", "", "", "8 km", "30 minutes", "A hike alongside Lake Invermere.", "", Difficulty.Easiest, 3, "", "", "", "", "", true, true, DateTime.Now,DateTime.Now,true);
			media [0] = new Media (1, 1, "Baby Deer Looks Cute", "Image", "", new byte[0], DateTime.Now, DateTime.Now, DateTime.Now, false);
			points [0] = new Point (1, 1, 1, 2, "", "", 100, 101, true, DateTime.Now);
			points [1] = new Point (2, 1, 1, -1, "", "", 110, 111, false, DateTime.Now);
			points [2] = new Point (3, 1, 1, 4, "", "", 120, 121, true, DateTime.Now);
			points [3] = new Point (4, 1, 1, -1, "", "", 130, 131, false, DateTime.Now);
			points [4] = new Point (5, 1, 1, 6, "", "", 140, 141, true, DateTime.Now);
			points [5] = new Point (6, 1, 1, -1, "", "", 150, 151, false, DateTime.Now);
			trailsToActivities [0] = new TrailsToActivities (1, 1);
			trailsToActivities [1] = new TrailsToActivities (1, 2);
			trailsToActivities [2] = new TrailsToActivities (2, 1);
			trailsToActivities [3] = new TrailsToActivities (3, 1);
			trailsToAmenities [0] = new TrailsToAmenities (1, 1);
			trailsToAmenities [1] = new TrailsToAmenities (1, 2);
			trailsToAmenities [2] = new TrailsToAmenities (2, 1);
			trailsToAmenities [3] = new TrailsToAmenities (3, 1);
			
			// Insert the rows.
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			dataLayer.insertRows(activities, amenities, favouriteTrails, mapTiles, media, organizations, points, roles, trails, trailsToActivities, trailsToAmenities, users);

			return "Successfully inserted rows.";
		}


		/// <summary>
		/// Gets the datetime of the last database update.
		/// </summary>
		/// <returns>The datetime of the last database update.</returns>
		public string getDatabaseLastUpdated()
		{
			Data_Layer_App_Globals dataLayerAppGlobals = new Data_Layer_App_Globals();
			DateTime databaseLastUpdated = dataLayerAppGlobals.getDatabaseLastUpdated ();

			if (databaseLastUpdated != null) {
				Console.WriteLine (databaseLastUpdated.ToLongDateString());

				return "Successfully got database last updated.";
			} else {
				return "Failed to get database last updated.";
			}
		}


		/// <summary>
		/// Sets the datetime of the last database update.
		/// </summary>
		/// <returns>The database last updated.</returns>
		public string setDatabaseLastUpdated()
		{
			// Get the current datetime.
			DateTime databaseLastUpdated = DateTime.Now;

			// Set the database last updated date.
			Data_Layer_App_Globals dataLayerAppGlobals = new Data_Layer_App_Globals();
			dataLayerAppGlobals.setDatabaseLastUpdated (databaseLastUpdated);

			return "Successfully set the datetime of the last database update.";
		}


		/// <summary>
		/// Gets the activities.
		/// </summary>
		/// <returns>The activities.</returns>
		public string getActivities()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			List<Activity> activities = dataLayer.getActivities ();

			if (activities != null) {
				foreach (Activity activity in activities) {
					Console.WriteLine (activity.id + " - " + activity.activityName);
				}

				return "Successfully got activities.";
			} else {
				return "Failed to get activities.";
			}
		}


		/// <summary>
		/// Gets the amenities.
		/// </summary>
		/// <returns>The amenities.</returns>
		public string getAmenities()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			List<Amenity> amenities = dataLayer.getAmenities ();

			if (amenities != null) {
				foreach (Amenity amenity in amenities) {
					Console.WriteLine (amenity.id + " - " + amenity.amenityName);
				}

				return "Successfully got amenities.";
			} else {
				return "Failed to get amenities.";
			}
		}

		public string getUsers()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			List<User> users = dataLayer.getUsers ();

			if (users != null) {
				foreach (User user in users) {
					Console.WriteLine (user.id + " - " + user.username);
				}

				return "Successfully got users.";
			} else {
				return "Failed to get users.";
			}
		}


		/// <summary>
		/// Gets the trails for a sample search filter.
		/// </summary>
		/// <returns>The trails by search filter.</returns>
		public string getTrailsBySearchFilter()
		{
			// Create a sample search filter.
			// TODO: Add more filter parameters.
			SearchFilter searchFilter = new SearchFilter () { rating = 3 };

			// Get the search results.
			Data_Layer_Search_Trails dataLayer = new Data_Layer_Search_Trails ();
			List<ListableTrail> searchResults = dataLayer.getTrailsBySearchFilter (searchFilter);

			if (searchResults != null) {
				foreach (ListableTrail searchResult in searchResults) {
					Console.WriteLine (searchResult.trail.id + " - " + searchResult.trail.name);

					Console.Write(" -Activities: ");
					foreach (Activity activity in searchResult.activities) {
						Console.Write (activity.id + " - " + activity.activityName + ",");
					}
					Console.WriteLine ();

					Console.Write(" -Amenities: ");
					foreach (Amenity amenity in searchResult.amenities) {
						Console.Write (amenity.id + " - " + amenity.amenityName + ",");
					}
					Console.WriteLine ();

					Console.Write(" -Points: ");
					foreach (Point point in searchResult.points) {
						Console.Write (point.id + " - " + point.lat + ",");
					}
					Console.WriteLine ();
				}

				return "Successfully got trails by search filter.";
			} else {
				return "Failed to get trails by search filter.";
			}
		}


		/// <summary>
		/// Gets a sample trail.
		/// </summary>
		/// <returns>The trail.</returns>
		public string getTrail()
		{
			Data_Layer_View_Trail dataLayer = new Data_Layer_View_Trail ();
			Trail trail = dataLayer.getTrail (1);

			if (trail != null) {
				Console.WriteLine (trail.id + " - " + trail.name + " - " + trail.rating);
				return "Successfully got trail.";
			} else {
				return "Failed to get trail.";
			}
		}

		
		/// <summary>
		/// Updates the tables with sample data.
		/// </summary>
		/// <returns>The tables.</returns>
		public string updateTables()
		{
			// NOTE: Not implemented yet.
			return "Cannot test update tables.  It is not implemented yet.";
		}


		/// <summary>
		/// Deletes some data from the tables.
		/// </summary>
		/// <returns>The some from tables.</returns>
		public string deleteSomeFromTables()
		{
			// Define the lists.
			List<Activity> activities = new List<Activity> ();
			List<Amenity> amenities = new List<Amenity> ();
			List<Comment> comments = new List<Comment> ();
			List<FavouriteTrails> favouriteTrails = new List<FavouriteTrails> ();
			List<MapTile> mapTiles = new List<MapTile> ();
			List<Media> media = new List<Media> ();
			List<Organization> organizations = new List<Organization> ();
			List<Point> points = new List<Point> ();
			List<Role> roles = new List<Role> ();
			List<Trail> trails = new List<Trail> ();
			List<TrailsToActivities> trailsToActivities = new List<TrailsToActivities> ();
			List<TrailsToAmenities> trailsToAmenities = new List<TrailsToAmenities> ();
			List<User> users = new List<User> ();

			// Add a row to delete.
			activities.Add (new Activity (){ id = 2 });

			// Delete some data from the tables.
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			dataLayer.deleteRows (activities.ToArray(), amenities.ToArray(), comments.ToArray(), favouriteTrails.ToArray(), mapTiles.ToArray(), media.ToArray(),
				organizations.ToArray(), points.ToArray(), roles.ToArray(), trails.ToArray(), trailsToActivities.ToArray(), trailsToAmenities.ToArray(), users.ToArray());

			return "Successfully deleted some rows.";
		}


		/// <summary>
		/// Deletes all data from the tables.
		/// </summary>
		/// <returns>The all from tables.</returns>
		public string deleteAllFromTables()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			dataLayer.clearTables ();

			// TODO: Reset the indices for the primary keys.

			return "Successfully deleted all rows.";
		}


		/// <summary>
		/// Drops the tables.
		/// </summary>
		/// <returns>The tables.</returns>
		public string dropTables()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			dataLayer.dropTables();

			return "Successfully dropped tables.";
		}
	}
}

