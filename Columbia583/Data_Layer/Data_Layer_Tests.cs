using System;
using System.Collections.Generic;

namespace Columbia583
{
	public class Data_Layer_Tests
	{
		public Data_Layer_Tests ()
		{

		}


		/**
		 * Creates all the tables.
		 * */
		public string createTables()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			dataLayer.createTables ();

			return "Successfully created tables.";
		}


		/**
		 * Inserts some sample data into the tables.
		 * */
		public string insertIntoTables()
		{
			// Create rows without foreign keys.
			Activity[] activities = new Activity[3];
			Amenity[] amenities = new Amenity[3];
			MapTile[] mapTiles = new MapTile[1];
			Media[] media = new Media[1];
			Organization[] organizations = new Organization[1];
			Role[] roles = new Role[1];
			activities [0] = new Activity (1, "Hiking", "hiking.jpg", DateTime.Now);
			activities [1] = new Activity (2, "Biking", "biking.jpg", DateTime.Now);
			activities [2] = new Activity (3, "Skiing", "skiing.jpg", DateTime.Now);
			amenities [0] = new Amenity (1, "Washrooms", "washrooms.jpg", DateTime.Now);
			amenities [1] = new Amenity (2, "Campground", "campground.jpg", DateTime.Now);
			amenities [2] = new Amenity (3, "Picnic Area", "picnic.jpg", DateTime.Now);
			mapTiles [0] = new MapTile (1, 100, 200, 10, 20, "mapTile_100x200_10x20.jpg", DateTime.Now);
			media [0] = new Media (1, "Baby Deer Looks Cute", "Image", "", DateTime.Now);
			organizations [0] = new Organization (1, "Columbia Valley Greenways Trail Alliance", DateTime.Now);
			roles [0] = new Role (1, "Administrator", DateTime.Now);

			// Create rows with foreign keys.
			// TODO: Figure out how to nullify the next point ID field.
			User[] users = new User[1];
			Trail[] trails = new Trail[3];
			Point[] points = new Point[6];
			TrailsToActivities[] trailsToActivities = new TrailsToActivities[4];
			TrailsToAmenities[] trailsToAmenities = new TrailsToAmenities[4];
			users [0] = new User (1, 0, "rob_orchiston@greenways.ca", "rob_orchiston", DateTime.Now);
			trails [0] = new Trail (1, 1, 1, "Trail 1", "Radium", "", "", "32 km", "2 hours", "A hike through the valley.", "", Difficulty.More_Difficult, 4, "", "", "", "", "", true, true, DateTime.Now);
			trails [1] = new Trail (2, 1, 1, "Trail 2", "Invermere", "", "", "16 km", "1 hours", "A hike along the creeks.", "", Difficulty.Easy, 2, "", "", "", "", "", true, true, DateTime.Now);
			trails [2] = new Trail (3, 1, 1, "Trail 3", "Windermere", "", "", "8 km", "30 minutes", "A hike alongside Lake Invermere.", "", Difficulty.Easiest, 3, "", "", "", "", "", true, true, DateTime.Now);
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
			dataLayer.insertRows(activities, amenities, mapTiles, media, organizations, points, roles, trails, trailsToActivities, trailsToAmenities, users);

			return "Successfully inserted rows.";
		}


		/**
		 * Gets all IDs and timestamps from the database.
		 * */
		public string getAllIdTimestampCombos()
		{
			// NOTE: Not implemented yet.
			return "Cannot get all ID timestamp combos.  Not implemented yet.";

			/*
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			IdTimestampComboList idTimestampComboList = dataLayer.getAllIdTimestampCombos ();
			
			if (idTimestampComboList != null) {
				Console.WriteLine (idTimestampComboList.activityIds.Length + " activities.");
				Console.WriteLine (idTimestampComboList.amenityIds.Length + " amenities.");
				Console.WriteLine (idTimestampComboList.mapTileIds.Length + " map tiles.");
				Console.WriteLine (idTimestampComboList.mediaIds.Length + " medias.");
				Console.WriteLine (idTimestampComboList.organizationIds.Length + " organizations.");
				Console.WriteLine (idTimestampComboList.pointIds.Length + " points.");
				Console.WriteLine (idTimestampComboList.roleIds.Length + " roles.");
				Console.WriteLine (idTimestampComboList.trailIds.Length + " trails.");
				Console.WriteLine (idTimestampComboList.userIds.Length + " users.");

				return "Successfully got all ID timestamp combos.";
			} else {
				return "Failed to get all ID timestamp combos.";
			}
			*/
		}


		/**
		 * Gets all activities in the database.
		 * */
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


		/**
		 * Gets all amenities in the database.
		 * */
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


		/**
		 * Gets the trails by a sample search filter.
		 * */
		public string getTrailsBySearchFilter(SearchFilter searchFilter)
		{
			// Create a sample search filter.
			// TODO: Add more filter parameters.
			//SearchFilter searchFilter = new SearchFilter () { rating = 3 };

			// Get the search results.
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			List<SearchResult> searchResults = dataLayer.getTrailsBySearchFilter (searchFilter);

			if (searchResults != null) {
				foreach (SearchResult searchResult in searchResults) {
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


		/**
		 * Gets a trail from the database.
		 * */
		public string getTrail()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			Trail trail = dataLayer.getTrail (1);

			if (trail != null) {
				Console.WriteLine (trail.id + " - " + trail.name + " - " + trail.rating);
				return "Successfully got trail.";
			} else {
				return "Failed to get trail.";
			}
		}


		/**
		 * Updates the tables with some different sample data.
		 * */
		public string updateTables()
		{
			// NOTE: Not implemented yet.
			return "Cannot test update tables.  It is not implemented yet.";
		}


		/**
		 * Deletes some of the data from the tables.
		 * */
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


		/**
		 * Clears all data from the tables.
		 * */
		public string deleteAllFromTables()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			dataLayer.clearTables ();

			// TODO: Reset the indices for the primary keys.

			return "Successfully deleted all rows.";
		}


		/**
		 * Drops all tables from the database.
		 * */
		public string dropTables()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			dataLayer.dropTables();

			return "Successfully dropped tables.";
		}
	}
}

