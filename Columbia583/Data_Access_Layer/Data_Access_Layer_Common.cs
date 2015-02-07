using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Net;

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
			Service_Access_Layer_Common serviceAccessLayer = new Service_Access_Layer_Common ();

			// Drop and create the local database's tables.
			dataLayer.dropTables ();
			dataLayer.createTables ();

			// Get the current time prior to calling the webservice.
			DateTime currentTime = DateTime.Now;

			// Get the data from the webservice.
			List<Webservice_Trails> webserviceTrails = serviceAccessLayer.getAll ();

			// Define lists to copy the data to.
			List<Trail> trails = null;
			List<Organization> organizations = null;
			List<User> users = null;
			List<Activity> activities = null;
			List<Amenity> amenities = null;
			List<TrailsToActivities> trailsToActivities = null;
			List<TrailsToAmenities> trailsToAmenities = null;

			try
			{
				trails = new List<Trail>();
				organizations = new List<Organization>();
				users = new List<User>();
				activities = new List<Activity>();
				amenities = new List<Amenity>();
				trailsToActivities = new List<TrailsToActivities>();
				trailsToAmenities = new List<TrailsToAmenities>();

				// For each trail retrieved from the webservice, parse the data into the local classes.
				foreach(Webservice_Trails currentTrail in webserviceTrails)
				{
					int userId = 0;
					int orgId = 0;

					// Get this trail's user.
					if (currentTrail.user != null)
					{
						users.Add(new User(currentTrail.user.id, currentTrail.user.org_id, currentTrail.user.email, currentTrail.user.username, Convert.ToDateTime(currentTrail.user.updated_at)));
						userId = currentTrail.user.id;
					}

					// Get this trail's organization.
					if (currentTrail.organization != null)
					{
						organizations.Add(new Organization(currentTrail.organization.id, currentTrail.organization.organization, Convert.ToDateTime(currentTrail.organization.updated_at)));
						orgId = currentTrail.organization.id;
					}

					// Get this trail's activities.
					if (currentTrail.activity != null && currentTrail.activity.Count > 0)
					{
						foreach(Webservice_Activity activity in currentTrail.activity)
						{
							byte[] imageBytes = null;
							if (activity.icon != null && activity.icon != "")
							{
								// Get the full URL of the image by appending its local URI to the website.
								string baseUrl = "http://trails.greenways.ca/";
								string fullImageUrl = baseUrl + activity.icon;

								try
								{
									// Load the image from the web.
									WebClient client = new WebClient();
									imageBytes = client.DownloadData(fullImageUrl);
								}
								catch (WebException e)
								{
									// TODO: Log error.
									Console.WriteLine("Failed to download activity icon.  Activity ID = " + activity.id);
									Console.WriteLine(e.Message);
								}
							}

							activities.Add(new Activity(activity.id, activity.name, imageBytes, Convert.ToDateTime(activity.updated_at)));
							trailsToActivities.Add(new TrailsToActivities(currentTrail.id, activity.id));
						}
					}

					// Get this trail's amenities.
					if (currentTrail.amenity != null && currentTrail.amenity.Count > 0)
					{
						foreach(Webservice_Amenity amenity in currentTrail.amenity)
						{
							byte[] imageBytes = null;
							if (amenity.icon != null && amenity.icon != "")
							{
								// Get the full URL of the image by appending its local URI to the website.
								string baseUrl = "http://trails.greenways.ca/";
								string fullImageUrl = baseUrl + amenity.icon;

								try
								{
									// Load the image from the web.
									WebClient client = new WebClient();
									imageBytes = client.DownloadData(fullImageUrl);
								}
								catch (WebException e)
								{
									// TODO: Log error.
									Console.WriteLine("Failed to download amenity icon.  Amenity ID = " + amenity.id);
									Console.WriteLine(e.Message);
								}
							}

							amenities.Add(new Amenity(amenity.id, amenity.name, imageBytes, Convert.ToDateTime(amenity.updated_at)));
							trailsToAmenities.Add(new TrailsToAmenities(currentTrail.id, amenity.id));
						}
					}

					// Get the boolean for open.
					// TODO: Find a better way to handle boolean strings.
					bool trailOpen = false;
					if (currentTrail.open == "0")
					{
						trailOpen = false;
					}
					else if (currentTrail.open == "1")
					{
						trailOpen = true;
					}
					else
					{
						trailOpen = Convert.ToBoolean(currentTrail.open);
					}

					// Get the enum for difficulty.
					Difficulty trailDifficulty;
					if(currentTrail.difficulty.Contains(" "))
					{
						string[] token = currentTrail.difficulty.Split(' ');
						string temp = token[0]+"_"+token[1];
						trailDifficulty = (Difficulty) Enum.Parse(typeof(Difficulty),temp);

					}
					else
					{
						trailDifficulty = (Difficulty) Enum.Parse(typeof(Difficulty), currentTrail.difficulty);
					}

					// Get the base trail.
					trails.Add(new Trail(currentTrail.id, userId, orgId, currentTrail.name, currentTrail.location, currentTrail.kml_name, currentTrail.kml_content, currentTrail.distance,
						currentTrail.duration, currentTrail.description, currentTrail.directions, trailDifficulty, currentTrail.rating, currentTrail.hazards, currentTrail.surface,
						currentTrail.landAccess, currentTrail.maintenance, currentTrail.season, trailOpen, currentTrail.active, Convert.ToDateTime(currentTrail.updated_at)));
				}

				// Remove duplicate entries in the resulting lists.
				trails = trails.Distinct().ToList();
				organizations = organizations.Distinct().ToList();
				users = users.Distinct().ToList();
				activities = activities.Distinct().ToList();
				amenities = amenities.Distinct().ToList();
				trailsToActivities = trailsToActivities.Distinct().ToList();
				trailsToAmenities = trailsToAmenities.Distinct().ToList();
			}
			catch (Exception e)
			{
				Console.WriteLine (e.Message);
			}
			
			// TODO: Get the remaining database data.
			MapTile[] mapTiles = new MapTile[0];
			Media[] media = new Media[0];
			Point[] points = new Point[0];
			Role[] roles = new Role[0];
			
			// Store the data in the local database.
			dataLayer.insertRows (activities.ToArray(), amenities.ToArray(), mapTiles, media, organizations.ToArray(), points, roles, trails.ToArray(), trailsToActivities.ToArray(),
				trailsToAmenities.ToArray(), users.ToArray());

			// Store the current time in the database as the last-updated time.
			dataLayer.setDatabaseLastUpdated (currentTime);

			Console.WriteLine ("Database Initialized!");
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
			int[] activitiesToDelete = new int[0];

			Amenity[] amenitiesToInsert = new Amenity[0];
			Amenity[] amenitiesToUpdate = new Amenity[0];
			int[] amenitiesToDelete = new int[0];

			MapTile[] mapTilesToInsert = new MapTile[0];
			MapTile[] mapTilesToUpdate = new MapTile[0];
			int[] mapTilesToDelete = new int[0];

			Media[] mediaToInsert = new Media[0];
			Media[] mediaToUpdate = new Media[0];
			int[] mediaToDelete = new int[0];

			Organization[] organizationsToInsert = new Organization[0];
			Organization[] organizationsToUpdate = new Organization[0];
			int[] organizationsToDelete = new int[0];

			Point[] pointsToInsert = new Point[0];
			Point[] pointsToUpdate = new Point[0];
			int[] pointsToDelete = new int[0];

			Role[] rolesToInsert = new Role[0];
			Role[] rolesToUpdate = new Role[0];
			int[] rolesToDelete = new int[0];

			Trail[] trailsToInsert = new Trail[0];
			Trail[] trailsToUpdate = new Trail[0];
			int[] trailsToDelete = new int[0];

			TrailsToActivities[] trailsToActivitiesToInsert = new TrailsToActivities[0];
			TrailsToActivities[] trailsToActivitiesToUpdate = new TrailsToActivities[0];
			TrailsToActivities[] trailsToActivitiesToDelete = new TrailsToActivities[0];

			TrailsToAmenities[] trailsToAmenitiesToInsert = new TrailsToAmenities[0];
			TrailsToAmenities[] trailsToAmenitiesToUpdate = new TrailsToAmenities[0];
			TrailsToAmenities[] trailsToAmenitiesToDelete = new TrailsToAmenities[0];

			User[] usersToInsert = new User[0];
			User[] usersToUpdate = new User[0];
			int[] usersToDelete = new int[0];
			
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

