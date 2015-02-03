using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

// TODO: Figure out how to display images that are loaded from the database (will probably need to use a stream).
// TODO: Implement image caching logic for the map tiles and the media.
// TODO: Create an ID getter for each table for Mariam's update methods. (TrailsToActivities and TrailsToAmenities).
// TODO: Remove the auto-increment attribute from the database primary keys.

// NOTE: Area of interest is 170 km x 50 km

namespace Columbia583
{
	/// <summary>
	/// Data layer common contains the SQLite queries that are globally common for the app.
	/// </summary>
	public class Data_Layer_Common
	{
		public Data_Layer_Common ()
		{

		}


		/// <summary>
		/// Gets the path to database.
		/// </summary>
		/// <returns>The path to database.</returns>
		public static string getPathToDatabase()
		{
			string docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			string pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");

			return pathToDatabase;
		}


		/// <summary>
		/// Creates the tables in the database.
		/// </summary>
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
				connection.CreateTable<DatabaseLastUpdated>();

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


		/// <summary>
		/// Inserts the rows.
		/// NOTE: SQLite ignores primary keys during insertion.  It will insert the data regardless of what you set the primary key to.
		/// </summary>
		/// <param name="activities">Activities.</param>
		/// <param name="amenities">Amenities.</param>
		/// <param name="mapTiles">Map tiles.</param>
		/// <param name="media">Media.</param>
		/// <param name="organizations">Organizations.</param>
		/// <param name="points">Points.</param>
		/// <param name="roles">Roles.</param>
		/// <param name="trails">Trails.</param>
		/// <param name="trailsToActivities">Trails to activities.</param>
		/// <param name="trailsToAmenities">Trails to amenities.</param>
		/// <param name="users">Users.</param>
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


		/// <summary>
		/// Gets the datetime of the last database update.
		/// </summary>
		/// <returns>The datetime of the last update.</returns>
		public DateTime getDatabaseLastUpdated()
		{
			DateTime databaseLastUpdated = DateTime.FromOADate(0);
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get the last time the database was updated.
				var query = connection.Table<DatabaseLastUpdated>();
				if (query.Count() > 0)
				{
					DatabaseLastUpdated row = (DatabaseLastUpdated)query.First();
					databaseLastUpdated = row.timestamp;
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return databaseLastUpdated;
		}


		/// <summary>
		/// Sets the datetime of the last database update.
		/// </summary>
		/// <param name="newUpdateTime">The new datetime for the last database update.</param>
		public void setDatabaseLastUpdated(DateTime newUpdateTime)
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Encapsulate the given datetime in an object.
				DatabaseLastUpdated databaseLastUpdated = new DatabaseLastUpdated(newUpdateTime);

				// Set the last time the database was updated.
				connection.DeleteAll<DatabaseLastUpdated>();
				connection.Insert(databaseLastUpdated);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}


		/// <summary>
		/// Gets the activities.
		/// </summary>
		/// <returns>The activities.</returns>
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


		/// <summary>
		/// Gets the amenities.
		/// </summary>
		/// <returns>The amenities.</returns>
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


		/// <summary>
		/// Gets the IDs for all the activities.
		/// </summary>
		/// <returns>The activity identifiers.</returns>
		public List<int> getActivityIds()
		{
			List<int> activityIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<Activity>();
				activityIds = new List<int>();
				foreach(Activity activity in query)
				{
					activityIds.Add(activity.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return activityIds;
		}


		/// <summary>
		/// Gets the IDs for all the amenities.
		/// </summary>
		/// <returns>The amenity identifiers.</returns>
		public List<int> getAmenityIds()
		{
			List<int> amenityIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<Amenity>();
				amenityIds = new List<int>();
				foreach(Amenity amenity in query)
				{
					amenityIds.Add(amenity.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return amenityIds;
		}


		/// <summary>
		/// Gets the IDs for all the map tiles.
		/// </summary>
		/// <returns>The map tile identifiers.</returns>
		public List<int> getMapTileIds()
		{
			List<int> mapTileIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<MapTile>();
				mapTileIds = new List<int>();
				foreach(MapTile mapTile in query)
				{
					mapTileIds.Add(mapTile.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return mapTileIds;
		}


		/// <summary>
		/// Gets the IDs for all the media.
		/// </summary>
		/// <returns>The media identifiers.</returns>
		public List<int> getMediaIds()
		{
			List<int> mediaIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<Media>();
				mediaIds = new List<int>();
				foreach(Media media in query)
				{
					mediaIds.Add(media.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return mediaIds;
		}


		/// <summary>
		/// Gets the IDs for all the organizations.
		/// </summary>
		/// <returns>The organization identifiers.</returns>
		public List<int> getOrganizationIds()
		{
			List<int> organizationIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<Organization>();
				organizationIds = new List<int>();
				foreach(Organization organization in query)
				{
					organizationIds.Add(organization.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return organizationIds;
		}


		/// <summary>
		/// Gets the IDs for all the points.
		/// </summary>
		/// <returns>The point identifiers.</returns>
		public List<int> getPointIds()
		{
			List<int> pointIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<Point>();
				pointIds = new List<int>();
				foreach(Point point in query)
				{
					pointIds.Add(point.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return pointIds;
		}


		/// <summary>
		/// Gets the IDs for all the roles.
		/// </summary>
		/// <returns>The role identifiers.</returns>
		public List<int> getRoleIds()
		{
			List<int> roleIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<Role>();
				roleIds = new List<int>();
				foreach(Role role in query)
				{
					roleIds.Add(role.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return roleIds;
		}


		/// <summary>
		/// Gets the IDs for all the trails.
		/// </summary>
		/// <returns>The trail identifiers.</returns>
		public List<int> getTrailIds()
		{
			List<int> trailIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<Trail>();
				trailIds = new List<int>();
				foreach(Trail trail in query)
				{
					trailIds.Add(trail.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return trailIds;
		}


		/// <summary>
		/// Gets the trails to activities.
		/// </summary>
		/// <returns>The trails to activities.</returns>
		public List<TrailsToActivities> getTrailsToActivities()
		{
			List<TrailsToActivities> trailsToActivities = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<TrailsToActivities>();
				trailsToActivities = new List<TrailsToActivities>();
				foreach(TrailsToActivities trailToActivity in query)
				{
					trailsToActivities.Add(trailToActivity);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return trailsToActivities;
		}


		/// <summary>
		/// Gets the trails to amenities.
		/// </summary>
		/// <returns>The trails to amenities.</returns>
		public List<TrailsToAmenities> getTrailsToAmenities()
		{
			List<TrailsToAmenities> trailsToAmenities = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<TrailsToAmenities>();
				trailsToAmenities = new List<TrailsToAmenities>();
				foreach(TrailsToAmenities trailToAmenity in query)
				{
					trailsToAmenities.Add(trailToAmenity);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return trailsToAmenities;
		}


		/// <summary>
		/// Gets the IDs for all the users.
		/// </summary>
		/// <returns>The user identifiers.</returns>
		public List<int> getUserIds()
		{
			List<int> userIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<User>();
				userIds = new List<int>();
				foreach(User user in query)
				{
					userIds.Add(user.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return userIds;
		}

		
		/// <summary>
		/// Updates the rows.
		/// </summary>
		/// <param name="activities">Activities.</param>
		/// <param name="amenities">Amenities.</param>
		/// <param name="mapTiles">Map tiles.</param>
		/// <param name="media">Media.</param>
		/// <param name="organizations">Organizations.</param>
		/// <param name="points">Points.</param>
		/// <param name="roles">Roles.</param>
		/// <param name="trails">Trails.</param>
		/// <param name="trailsToActivities">Trails to activities.</param>
		/// <param name="trailsToAmenities">Trails to amenities.</param>
		/// <param name="users">Users.</param>
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

		
		/// <summary>
		/// Deletes the rows.
		/// </summary>
		/// <param name="activities">Activities.</param>
		/// <param name="amenities">Amenities.</param>
		/// <param name="mapTiles">Map tiles.</param>
		/// <param name="media">Media.</param>
		/// <param name="organizations">Organizations.</param>
		/// <param name="points">Points.</param>
		/// <param name="roles">Roles.</param>
		/// <param name="trails">Trails.</param>
		/// <param name="trailsToActivities">Trails to activities.</param>
		/// <param name="trailsToAmenities">Trails to amenities.</param>
		/// <param name="users">Users.</param>
		public void deleteRows(int[] activities, int[] amenities, int[] mapTiles, int[] media, int[] organizations,
			int[] points, int[] roles, int[] trails, TrailsToActivities[] trailsToActivities, TrailsToAmenities[] trailsToAmenities, int[] users)
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());
				
				// Delete the data that has foreign keys.
				foreach(var row in trailsToActivities)	connection.Delete(row);
				foreach(var row in trailsToAmenities)	connection.Delete(row);
				foreach(int id in points)				connection.Delete<Point>(id);
				foreach(int id in trails)				connection.Delete<Trail>(id);
				foreach(int id in users)				connection.Delete<User>(id);
				
				// Delete the data that has no foreign keys.
				foreach(int id in activities)		connection.Delete<Activity>(id);
				foreach(int id in amenities)		connection.Delete<Amenity>(id);
				foreach(int id in mapTiles)			connection.Delete<MapTile>(id);
				foreach(int id in media)			connection.Delete<Media>(id);
				foreach(int id in organizations)	connection.Delete<Organization>(id);
				foreach(int id in roles)			connection.Delete<Role>(id);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}

		
		/// <summary>
		/// Clears the tables.
		/// NOTE: SQLite doesn't reset the primary key index when a table is cleared.  To reset, drop and recreate the tables.
		/// </summary>
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
				connection.DeleteAll<DatabaseLastUpdated>();

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


		/// <summary>
		/// Drops the tables.
		/// </summary>
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
				connection.DropTable<DatabaseLastUpdated>();

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

