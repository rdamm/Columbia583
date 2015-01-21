using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Columbia583
{
	public class Data_Layer_Common
	{
		public Data_Layer_Common ()
		{

		}
		


		/**
		 * Gets the path to the database.
		 * */
		public static string getPathToDatabase()
		{
			string docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			string pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");

			return pathToDatabase;
		}


		/**
		 * Creates the tables.
		 * */
		public void createTables()
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Create all the tables that have no foreign keys.
				connection.CreateTable<Activity>();
				connection.CreateTable<Amenity>();
				connection.CreateTable<MapTile>();
				connection.CreateTable<Media>();
				connection.CreateTable<Organization>();
				connection.CreateTable<Role>();

				// Create all the tables that have foreign keys.
				connection.CreateTable<User>();					// References Organization.
				connection.CreateTable<Trail>();				// References Organization and User.
				connection.CreateTable<Point>();				// References Trail and MapTile.
				connection.CreateTable<TrailsToActivities>();	// References Trail and Activity.
				connection.CreateTable<TrailsToAmenities>();	// References Trail and Amenity.

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}


		/**
		 * Insert the given rows.
		 * 
		 * NOTE: SQLite ignores primary keys during insertion.  It will insert the data regardless of what you set the primary key to.
		 * */
		public void insertRows(Activity[] activities, Amenity[] amenities, MapTile[] mapTiles, Media[] media, Organization[] organizations,
			Point[] points, Role[] roles, Trail[] trails, TrailsToActivities[] trailsToActivities, TrailsToAmenities[] trailsToAmenities, User[] users)
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Insert the data that has no foreign keys.
				connection.InsertAll(activities);
				connection.InsertAll(amenities);
				connection.InsertAll(mapTiles);
				connection.InsertAll(media);
				connection.InsertAll(organizations);
				connection.InsertAll(roles);

				// Insert the data that has foreign keys.
				connection.InsertAll(users);
				connection.InsertAll(trails);
				connection.InsertAll(points);
				connection.InsertAll(trailsToActivities);
				connection.InsertAll(trailsToAmenities);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}


		/**
		 * Get the IDs and timestamps for every table in the database that contains them.
		 * */
		public IdTimestampComboList getAllIdTimestampCombos()
		{
			IdTimestampComboList idsAndTimestamps = null;

			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all ID and timestamps in the database.
				// TODO: Figure out how to perform a limited SELECT WHERE query.
				idsAndTimestamps = new IdTimestampComboList();

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return idsAndTimestamps;
		}


		/**
		 * Gets all activities.
		 * */
		public List<Activity> getActivities()
		{
			List<Activity> activities = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all activities.
				var query = connection.Table<Activity>();
				activities = new List<Activity>();
				foreach(Activity activity in query)
				{
					activities.Add(activity);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return activities;
		}


		/**
		 * Gets all amenities.
		 * */
		public List<Amenity> getAmenities()
		{
			List<Amenity> amenities = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all amenities.
				var query = connection.Table<Amenity>();
				amenities = new List<Amenity>();
				foreach(Amenity amenity in query)
				{
					amenities.Add(amenity);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return amenities;
		}


		/**
		 * Gets the trails by their search filter.
		 * */
		public List<SearchResult> getTrailsBySearchFilter(SearchFilter searchFilter)
		{
			List<SearchResult> searchResults = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				/*
				 * public int[] activities { get; set; }
				public int[] amenities { get; set; }
				public Difficulty difficulty { get; set; }
				public int rating { get; set; }
				public int minDuration { get; set; }
				public int maxDuration { get; set; }
				public int minDistance { get; set; }
				public int maxDistance { get; set; }*/
				
				// Get all trails that match the search filter.
				// TODO: Add the rest of the search filter parameters.
				var response = connection.Query<Trail>("SELECT * FROM Trail WHERE rating >= ?", searchFilter.rating);

				// For each matching trail, get its points, activities, and amenities.
				searchResults = new List<SearchResult>();
				foreach (Trail trailRow in response)
				{
					List<Point> points = new List<Point>();
					List<Activity> activities = new List<Activity>();
					List<Amenity> amenities = new List<Amenity>();

					// Get the points.
					var pointsQueryResponse = connection.Query<Point>("SELECT * FROM Point WHERE trailId = ?", trailRow.id);
					foreach(Point point in pointsQueryResponse)
					{
						points.Add(point);
					}

					// Get the activities.
					var activitiesQueryResponse = connection.Query<Activity>("SELECT * FROM Activity INNER JOIN TrailsToActivities ON Activity.id = TrailsToActivities.activityId WHERE trailId = ?", trailRow.id);
					foreach(Activity activity in activitiesQueryResponse)
					{
						activities.Add(activity);
					}

					// Get the amenities.
					var amenitiesQueryResponse = connection.Query<Amenity>("SELECT * FROM Amenity INNER JOIN TrailsToAmenities ON Amenity.id = TrailsToAmenities.amenityId WHERE trailId = ?", trailRow.id);
					foreach(Amenity amenity in amenitiesQueryResponse)
					{
						amenities.Add(amenity);
					}

					// Encapsulate the data into a search result and add it to the list.
					SearchResult searchResult = new SearchResult(trailRow, points.ToArray(), activities.ToArray(), amenities.ToArray());
					searchResults.Add(searchResult);
				}
				
				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return searchResults;
		}


		/**
		 * Gets a given trail.
		 * */
		public Trail getTrail(int trailId)
		{
			Trail trail = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());
				
				// Get the trail.
				// TODO: Get the other necessary trail information (eg. Media)
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				trail = connection.Find<Trail>(trailId);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return trail;
		}


		/**
		 * Updates the given rows.
		 * */
		public void updateRows(Activity[] activities, Amenity[] amenities, MapTile[] mapTiles, Media[] media, Organization[] organizations,
			Point[] points, Role[] roles, Trail[] trails, TrailsToActivities[] trailsToActivities, TrailsToAmenities[] trailsToAmenities, User[] users)
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());
				
				// Update the data that has no foreign keys.
				connection.UpdateAll(activities);
				connection.UpdateAll(amenities);
				connection.UpdateAll(mapTiles);
				connection.UpdateAll(media);
				connection.UpdateAll(organizations);
				connection.UpdateAll(roles);

				// Update the data that has foreign keys.
				// TODO: Figure out how to handle foreign keys for updates.
				/*
				connection.UpdateAll(points);
				connection.UpdateAll(trails);
				connection.UpdateAll(users);
				*/
				
				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}


		/**
		 * Delete the given rows.
		 * */
		public void deleteRows(Activity[] activities, Amenity[] amenities, MapTile[] mapTiles, Media[] media, Organization[] organizations,
			Point[] points, Role[] roles, Trail[] trails, TrailsToActivities[] trailsToActivities, TrailsToAmenities[] trailsToAmenities, User[] users)
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Delete the data that has foreign keys.
				foreach(var row in trailsToActivities)	connection.Delete(row);
				foreach(var row in trailsToAmenities)	connection.Delete(row);
				foreach(var row in points)				connection.Delete(row);
				foreach(var row in trails)				connection.Delete(row);
				foreach(var row in users)				connection.Delete(row);
				
				// Delete the data that has no foreign keys.
				foreach(var row in activities)			connection.Delete(row);
				foreach(var row in amenities)			connection.Delete(row);
				foreach(var row in mapTiles)			connection.Delete(row);
				foreach(var row in media)				connection.Delete(row);
				foreach(var row in organizations)		connection.Delete(row);
				foreach(var row in roles)				connection.Delete(row);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}


		/**
		 * Clears all data from the local database tables.
		 * */
		public void clearTables()
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Delete the data that has foreign keys.
				connection.DeleteAll<TrailsToActivities>();
				connection.DeleteAll<TrailsToAmenities>();
				connection.DeleteAll<Point>();
				connection.DeleteAll<Trail>();
				connection.DeleteAll<User>();

				// Delete the data that has no foreign keys.
				connection.DeleteAll<Activity>();
				connection.DeleteAll<Amenity>();
				connection.DeleteAll<MapTile>();
				connection.DeleteAll<Media>();
				connection.DeleteAll<Organization>();
				connection.DeleteAll<Role>();

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}


		/**
		 * Drops the tables from the local database.
		 * */
		public void dropTables()
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Drop the tables that have foreign keys.
				connection.DropTable<TrailsToActivities>();
				connection.DropTable<TrailsToAmenities>();
				connection.DropTable<Point>();
				connection.DropTable<Trail>();
				connection.DropTable<User>();

				// Drop the tables that have no foreign keys.
				connection.DropTable<Activity>();
				connection.DropTable<Amenity>();
				connection.DropTable<MapTile>();
				connection.DropTable<Media>();
				connection.DropTable<Organization>();
				connection.DropTable<Role>();

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}
	}
}

