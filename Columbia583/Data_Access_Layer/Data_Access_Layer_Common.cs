using System;

namespace Columbia583
{
	/// <summary>
	/// The common data access layer will control all the global database and webservice requests.
	/// </summary>
	public class Data_Access_Layer_Common
	{
		public Data_Access_Layer_Common ()
		{

		}


		/// <summary>
		/// Initializes and populates the local database.
		/// </summary>
		public void initializeDatabase()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();

			// Create the local database's tables.
			dataLayer.createTables ();

			// Get the current time prior to calling the webservice.
			DateTime currentTime = DateTime.Now;

			// TODO: Get the data from the webservices.

			// TODO: Parse the data out into local class objects.
			Activity[] activities = new Activity[0];
			Amenity[] amenities = new Amenity[0];
			MapTile[] mapTiles = new MapTile[0];
			Media[] media = new Media[0];
			Organization[] organizations = new Organization[0];
			Point[] points = new Point[0];
			Role[] roles = new Role[0];
			Trail[] trails = new Trail[0];
			TrailsToActivities[] trailsToActivities = new TrailsToActivities[0];
			TrailsToAmenities[] trailsToAmenities = new TrailsToAmenities[0];
			User[] users = new User[0];
			
			// Store the data in the local database.
			dataLayer.insertRows (activities, amenities, mapTiles, media, organizations, points, roles, trails, trailsToActivities, trailsToAmenities, users);

			// Store the current time in the database as the last-updated time.
			dataLayer.setDatabaseLastUpdated (currentTime);
		}


		/// <summary>
		/// Updates the local database so that it is up-to-date with the live database.  This should be called on a regular interval.
		/// </summary>
		public void updateDatabase()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();

			// Get the current time prior to calling the webservice.
			DateTime currentTime = DateTime.Now;

			// TODO: Call the webservice to get the rows that must be inserted, updated, and deleted.
			Activity[] activitiesToInsert = new Activity[0];
			Activity[] activitiesToUpdate = new Activity[0];
			Activity[] activitiesToDelete = new Activity[0];

			Amenity[] amenitiesToInsert = new Amenity[0];
			Amenity[] amenitiesToUpdate = new Amenity[0];
			Amenity[] amenitiesToDelete = new Amenity[0];

			MapTile[] mapTilesToInsert = new MapTile[0];
			MapTile[] mapTilesToUpdate = new MapTile[0];
			MapTile[] mapTilesToDelete = new MapTile[0];

			Media[] mediaToInsert = new Media[0];
			Media[] mediaToUpdate = new Media[0];
			Media[] mediaToDelete = new Media[0];

			Organization[] organizationsToInsert = new Organization[0];
			Organization[] organizationsToUpdate = new Organization[0];
			Organization[] organizationsToDelete = new Organization[0];

			Point[] pointsToInsert = new Point[0];
			Point[] pointsToUpdate = new Point[0];
			Point[] pointsToDelete = new Point[0];

			Role[] rolesToInsert = new Role[0];
			Role[] rolesToUpdate = new Role[0];
			Role[] rolesToDelete = new Role[0];

			Trail[] trailsToInsert = new Trail[0];
			Trail[] trailsToUpdate = new Trail[0];
			Trail[] trailsToDelete = new Trail[0];

			TrailsToActivities[] trailsToActivitiesToInsert = new TrailsToActivities[0];
			TrailsToActivities[] trailsToActivitiesToUpdate = new TrailsToActivities[0];
			TrailsToActivities[] trailsToActivitiesToDelete = new TrailsToActivities[0];

			TrailsToAmenities[] trailsToAmenitiesToInsert = new TrailsToAmenities[0];
			TrailsToAmenities[] trailsToAmenitiesToUpdate = new TrailsToAmenities[0];
			TrailsToAmenities[] trailsToAmenitiesToDelete = new TrailsToAmenities[0];

			User[] usersToInsert = new User[0];
			User[] usersToUpdate = new User[0];
			User[] usersToDelete = new User[0];
			
			// Insert the rows that must be inserted.
			dataLayer.insertRows (activitiesToInsert, amenitiesToInsert, mapTilesToInsert, mediaToInsert, organizationsToInsert, pointsToInsert, rolesToInsert, trailsToInsert, trailsToActivitiesToInsert,
				trailsToAmenitiesToInsert, usersToInsert);

			// Update the rows that must be updated.
			dataLayer.updateRows (activitiesToUpdate, amenitiesToUpdate, mapTilesToUpdate, mediaToUpdate, organizationsToUpdate, pointsToUpdate, rolesToUpdate, trailsToUpdate, trailsToActivitiesToUpdate,
				trailsToAmenitiesToUpdate, usersToUpdate);

			// Delete the rows that must be deleted.
			dataLayer.deleteRows (activitiesToDelete, amenitiesToDelete, mapTilesToDelete, mediaToDelete, organizationsToDelete, pointsToDelete, rolesToDelete, trailsToDelete, trailsToActivitiesToDelete,
				trailsToAmenitiesToDelete, usersToDelete);

			// Store the current time in the database as the last-updated time.
			dataLayer.setDatabaseLastUpdated (currentTime);
		}

		
		/// <summary>
		/// Destroys the local database before initializing and populating it again.
		/// </summary>
		public void reinitializeDatabase()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();

			// Drop the local database's tables.
			dataLayer.dropTables ();

			// Initialize the database.
			this.initializeDatabase ();
		}


		/// <summary>
		/// Gets the activities.
		/// </summary>
		/// <returns>The activities.</returns>
		public Activity[] getActivities()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			return dataLayer.getActivities ().ToArray();
		}


		/// <summary>
		/// Gets the amenities.
		/// </summary>
		/// <returns>The amenities.</returns>
		public Amenity[] getAmenities()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			return dataLayer.getAmenities ().ToArray ();
		}
	}
}

