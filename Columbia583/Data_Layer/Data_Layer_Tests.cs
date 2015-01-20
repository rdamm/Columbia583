using System;
using System.Collections.Generic;

namespace Columbia583
{
	public class Data_Layer_Tests
	{
		public Data_Layer_Tests ()
		{

		}


		public string createTables()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			dataLayer.createTables ();

			return "Successfully created tables.";
		}


		public string insertIntoTables()
		{
			// Create rows without foreign keys.
			Activity[] activities = new Activity[3];
			Amenity[] amenities = new Amenity[3];
			MapTile[] mapTiles = new MapTile[1];
			Media[] media = new Media[1];
			Organization[] organizations = new Organization[1];
			Role[] roles = new Role[1];
			activities [0] = new Activity (0, "Hiking", "hiking.jpg", DateTime.Now);
			activities [1] = new Activity (1, "Biking", "biking.jpg", DateTime.Now);
			activities [2] = new Activity (2, "Skiing", "skiing.jpg", DateTime.Now);
			amenities [0] = new Amenity (0, "Washrooms", "washrooms.jpg", DateTime.Now);
			amenities [1] = new Amenity (1, "Campground", "campground.jpg", DateTime.Now);
			amenities [2] = new Amenity (2, "Picnic Area", "picnic.jpg", DateTime.Now);
			mapTiles [0] = new MapTile (0, 100, 200, 10, 20, "mapTile_100x200_10x20.jpg", DateTime.Now);
			media [0] = new Media (0, "Baby Deer Looks Cute", "Image", "", DateTime.Now);
			organizations [0] = new Organization (0, "Columbia Valley Greenways Trail Alliance", DateTime.Now);
			roles [0] = new Role (0, "Administrator", DateTime.Now);

			// Create rows with foreign keys.
			// TODO: Figure out how to nullify the next point ID field.
			User[] users = new User[1];
			Trail[] trails = new Trail[3];
			Point[] points = new Point[6];
			TrailsToActivities[] trailsToActivities = new TrailsToActivities[4];
			TrailsToAmenities[] trailsToAmenities = new TrailsToAmenities[4];
			users [0] = new User (0, 0, "rob_orchiston@greenways.ca", "rob_orchiston", DateTime.Now);
			trails [0] = new Trail (0, 0, 0, "Trail 1", "Radium", "", "", "32 km", "2 hours", "A hike through the valley.", "", Difficulty.More_Difficult, 4, "", "", "", "", "", true, true, DateTime.Now);
			trails [1] = new Trail (0, 0, 0, "Trail 2", "Invermere", "", "", "16 km", "1 hours", "A hike along the creeks.", "", Difficulty.Easy, 2, "", "", "", "", "", true, true, DateTime.Now);
			trails [2] = new Trail (0, 0, 0, "Trail 3", "Windermere", "", "", "8 km", "30 minutes", "A hike alongside Lake Invermere.", "", Difficulty.Easiest, 3, "", "", "", "", "", true, true, DateTime.Now);
			points [0] = new Point (0, 0, 0, 1, "", "", 100, 101, true, DateTime.Now);
			points [1] = new Point (1, 0, 0, -1, "", "", 110, 111, false, DateTime.Now);
			points [2] = new Point (0, 0, 0, 3, "", "", 120, 121, true, DateTime.Now);
			points [3] = new Point (3, 0, 0, -1, "", "", 130, 131, false, DateTime.Now);
			points [4] = new Point (0, 0, 0, 5, "", "", 140, 141, true, DateTime.Now);
			points [5] = new Point (5, 0, 0, -1, "", "", 150, 151, false, DateTime.Now);
			trailsToActivities [0] = new TrailsToActivities (0, 0);
			trailsToActivities [1] = new TrailsToActivities (0, 1);
			trailsToActivities [2] = new TrailsToActivities (1, 0);
			trailsToActivities [3] = new TrailsToActivities (2, 0);
			trailsToAmenities [0] = new TrailsToAmenities (0, 0);
			trailsToAmenities [1] = new TrailsToAmenities (0, 1);
			trailsToAmenities [2] = new TrailsToAmenities (1, 0);
			trailsToAmenities [3] = new TrailsToAmenities (2, 0);


			// Insert the rows.
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			dataLayer.insertRows(activities, amenities, mapTiles, media, organizations, points, roles, trails, trailsToActivities, trailsToAmenities, users);

			return "Successfully inserted rows.";
		}


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


		public string deleteSomeFromTables()
		{
			// Create rows to delete.
			Activity[] activities = new Activity[1];
			activities [0] = new Activity { id = 2 };

			// Create the rest of the arrays.
			Amenity[] amenities = new Amenity[0];
			MapTile[] mapTiles = new MapTile[0];
			Media[] media = new Media[0];
			Organization[] organizations = new Organization[0];
			Role[] roles = new Role[0];
			User[] users = new User[0];
			Trail[] trails = new Trail[0];
			Point[] points = new Point[0];
			TrailsToActivities[] trailsToActivities = new TrailsToActivities[0];
			TrailsToAmenities[] trailsToAmenities = new TrailsToAmenities[0];

			// Delete some data from the tables.
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			dataLayer.deleteRows (activities, amenities, mapTiles, media, organizations, points, roles, trails, trailsToActivities, trailsToAmenities, users);

			return "Successfully deleted some rows.";
		}


		public string deleteAllFromTables()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			dataLayer.clearTables ();

			return "Successfully deleted all rows.";
		}


		public string dropTables()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			dataLayer.dropTables();

			return "Successfully dropped tables.";
		}
	}
}

