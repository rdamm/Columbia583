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
		/// Determines whether the database has been initialized or not.
		/// </summary>
		/// <returns><c>true</c>, if database was initialized, <c>false</c> otherwise.</returns>
		public bool databaseInitialized()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			return dataLayer.databaseInitialized ();
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
			Service_Access_Layer_Common serviceAccessLayer = new Service_Access_Layer_Common ();

			// Get the current time prior to calling the webservice.
			DateTime currentTime = DateTime.Now;
			
			// Get the last time the database was updated.
			DateTime lastUpdated = dataLayer.getDatabaseLastUpdated ();

			// Get the data from the webservice.
			string urlDate = lastUpdated.ToString("yyyy-MM-dd");
			List<Webservice_Trails> webserviceTrails_getAll = serviceAccessLayer.getAll ();
			List<Webservice_Trails> webserviceTrails = serviceAccessLayer.updateAll (urlDate);

			// Define lists to copy the data to.
			List<Trail> trails = null;
			List<Organization> organizations = null;
			List<User> users = null;
			List<Activity> activities = null;
			List<Amenity> amenities = null;
			List<TrailsToActivities> trailsToActivities = null;
			List<TrailsToAmenities> trailsToAmenities = null;

			List<Trail> updateTrails = null;
			List<Organization> updateOrganizations = null;
			List<User> updateUsers = null;
			List<Activity> updateActivities = null;
			List<Amenity> updateAmenities = null;
			List<TrailsToActivities> updateTrailsToActivities = null;
			List<TrailsToAmenities> updateTrailsToAmenities = null;

			List<Trail> deleteTrails = null;
			List<Organization> deleteOrganizations = null;
			List<User> deleteUsers = null;
			List<Activity> deleteActivities = null;
			List<Amenity> deleteAmenities = null;
			List<TrailsToActivities> deleteTrailsToActivities = null;
			List<TrailsToAmenities> deleteTrailsToAmenities = null;

			try
			{
				trails = new List<Trail>();
				organizations = new List<Organization>();
				users = new List<User>();
				activities = new List<Activity>();
				amenities = new List<Amenity>();
				trailsToActivities = new List<TrailsToActivities>();
				trailsToAmenities = new List<TrailsToAmenities>();

				updateTrails = new List<Trail>();
				updateOrganizations = new List<Organization>();
				updateUsers = new List<User>();
				updateActivities = new List<Activity>();
				updateAmenities = new List<Amenity>();
				updateTrailsToActivities = new List<TrailsToActivities>();
				updateTrailsToAmenities = new List<TrailsToAmenities>();

				deleteTrails = new List<Trail>();
				deleteOrganizations = new List<Organization>();
				deleteUsers = new List<User>();
				deleteActivities = new List<Activity>();
				deleteAmenities = new List<Amenity>();
				deleteTrailsToActivities = new List<TrailsToActivities>();
				deleteTrailsToAmenities = new List<TrailsToAmenities>();

				// Get the IDs for the existing database rows.
				List<int> existingTrailIds = new List<int>(dataLayer.getTrailIds());
				List<int> existingUserIds = new List<int>(dataLayer.getUserIds());
				List<int> existingOrganizationIds = new List<int>(dataLayer.getOrganizationIds());
				List<int> existingActivityIds = new List<int>(dataLayer.getActivityIds());
				List<int> existingAmenityIds = new List<int>(dataLayer.getAmenityIds());
				List<TrailsToActivities> existingTrailsToActivities = new List<TrailsToActivities>(dataLayer.getTrailsToActivities());
				List<TrailsToAmenities> existingTrailsToAmenities = new List<TrailsToAmenities>(dataLayer.getTrailsToAmenities());

				// Insert / update rows in the database.
				foreach(Webservice_Trails currentTrail in webserviceTrails)
				{
					int userId = 0;
					int orgId = 0;

					// Get this trail's user.
					if (currentTrail.user != null)
					{
						// If the user exists, update it.  Otherwise, insert it.
						if(dataLayer.getUser(currentTrail.user.id) != null)
						{
							updateUsers.Add(new User(currentTrail.user.id, currentTrail.user.org_id, currentTrail.user.email, currentTrail.user.username, Convert.ToDateTime(currentTrail.user.updated_at)));
						}
						else
						{
							users.Add(new User(currentTrail.user.id, currentTrail.user.org_id, currentTrail.user.email, currentTrail.user.username, Convert.ToDateTime(currentTrail.user.updated_at)));
						}
						userId = currentTrail.user.id;
					}
					
					// Get this trail's organization.
					if (currentTrail.organization != null)
					{
						// If the organization exists, update it.  Otherwise, insert it.
						if(dataLayer.getOrganization(currentTrail.organization.id) != null)
						{
							updateOrganizations.Add(new Organization(currentTrail.organization.id, currentTrail.organization.organization, Convert.ToDateTime(currentTrail.organization.updated_at)));
						}
						else
						{
							organizations.Add(new Organization(currentTrail.organization.id, currentTrail.organization.organization, Convert.ToDateTime(currentTrail.organization.updated_at)));
						}
						orgId = currentTrail.organization.id;
					}

					// Get this trail's activities.
					if (currentTrail.activity != null && currentTrail.activity.Count > 0)
					{
						foreach(Webservice_Activity activity in currentTrail.activity)
						{
							// If the activity exists, update it.  Otherwise, insert it.
							// TODO: Fetch the amenity icon's image from the net.
							byte[] activityIcon = new byte[0];
							if(dataLayer.getActivity(activity.id) != null)
							{
								updateActivities.Add(new Activity(activity.id, activity.name, activityIcon, Convert.ToDateTime(activity.updated_at)));
								updateTrailsToActivities.Add(new TrailsToActivities(currentTrail.id, activity.id));
							}
							else
							{
								activities.Add(new Activity(activity.id, activity.name, activityIcon, Convert.ToDateTime(activity.updated_at)));
								trailsToActivities.Add(new TrailsToActivities(currentTrail.id, activity.id));
							}
						}
					}

					// Get this trail's amenities.
					if (currentTrail.amenity != null && currentTrail.amenity.Count > 0)
					{
						foreach(Webservice_Amenity amenity in currentTrail.amenity)
						{
							// If the amenity exists, update it.  Otherwise, insert it.
							// TODO: Fetch the amenity icon's image from the net.
							byte[] amenityIcon = new byte[0];
							if(dataLayer.getAmenity(amenity.id) != null)
							{
								updateAmenities.Add(new Amenity(amenity.id, amenity.name, amenityIcon, Convert.ToDateTime(amenity.updated_at)));
								updateTrailsToAmenities.Add(new TrailsToAmenities(currentTrail.id, amenity.id));
							}
							else
							{
								amenities.Add(new Amenity(amenity.id, amenity.name, amenityIcon, Convert.ToDateTime(amenity.updated_at)));
								trailsToAmenities.Add(new TrailsToAmenities(currentTrail.id, amenity.id));
							}
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

					// If the base trail exists, update it.  Otherwise, insert it.
					if(dataLayer.getTrail(currentTrail.id) != null)
					{
						updateTrails.Add(new Trail(currentTrail.id, userId, orgId, currentTrail.name, currentTrail.location, currentTrail.kml_name, currentTrail.kml_content, currentTrail.distance,
							currentTrail.duration, currentTrail.description, currentTrail.directions, trailDifficulty, currentTrail.rating, currentTrail.hazards, currentTrail.surface,
							currentTrail.landAccess, currentTrail.maintenance, currentTrail.season, trailOpen, currentTrail.active, Convert.ToDateTime(currentTrail.updated_at)));
					}
					else
					{
						trails.Add(new Trail(currentTrail.id, userId, orgId, currentTrail.name, currentTrail.location, currentTrail.kml_name, currentTrail.kml_content, currentTrail.distance,
							currentTrail.duration, currentTrail.description, currentTrail.directions, trailDifficulty, currentTrail.rating, currentTrail.hazards, currentTrail.surface,
							currentTrail.landAccess, currentTrail.maintenance, currentTrail.season, trailOpen, currentTrail.active, Convert.ToDateTime(currentTrail.updated_at)));
					}
				}

				// Remove duplicate entries in the resulting lists.
				trails = trails.Distinct().ToList();
				organizations = organizations.Distinct().ToList();
				users = users.Distinct().ToList();
				activities = activities.Distinct().ToList();
				amenities = amenities.Distinct().ToList();
				trailsToActivities = trailsToActivities.Distinct().ToList();
				trailsToAmenities = trailsToAmenities.Distinct().ToList();

				// Find the trails to delete.
				foreach(int existingTrailId in existingTrailIds)
				{
					bool match = false;
					foreach(Webservice_Trails webserviceTrail in webserviceTrails_getAll)
					{
						if (webserviceTrail.id == existingTrailId)
						{
							match = true;
						}
					}
					if (match == false)
					{
						deleteTrails.Add(new Trail(){id = existingTrailId});
					}
				}

				// Find the users to delete.
				foreach(int existingUserId in existingUserIds)
				{
					bool match = false;
					foreach(Webservice_Trails webserviceTrail in webserviceTrails_getAll)
					{
						if (webserviceTrail.userId == existingUserId)
						{
							match = true;
						}
					}
					if (match == false)
					{
						deleteUsers.Add(new User(){id = existingUserId});
					}
				}

				// Find the organizations to delete.
				foreach(int existingOrganizationId in existingOrganizationIds)
				{
					bool match = false;
					foreach(Webservice_Trails webserviceTrail in webserviceTrails_getAll)
					{
						if (webserviceTrail.orgId == existingOrganizationId)
						{
							match = true;
						}
					}
					if (match == false)
					{
						deleteOrganizations.Add(new Organization(){id = existingOrganizationId});
					}
				}

				// Find the activities to delete.
				foreach(int existingActivityId in existingActivityIds)
				{
					bool match = false;
					foreach(Webservice_Trails webserviceTrail in webserviceTrails_getAll)
					{
						if (webserviceTrail.activity != null && webserviceTrail.activity.Count > 0)
						{
							foreach(Webservice_Activity activity in webserviceTrail.activity)
							{
								if (activity.id == existingActivityId)
								{
									match = true;
								}
							}
						}
					}
					if (match == false)
					{
						deleteActivities.Add(new Activity(){id = existingActivityId});
					}
				}

				// Find the amenities to delete.
				foreach(int existingAmenityId in existingAmenityIds)
				{
					bool match = false;
					foreach(Webservice_Trails webserviceTrail in webserviceTrails_getAll)
					{
						if (webserviceTrail.amenity != null && webserviceTrail.amenity.Count > 0)
						{
							foreach(Webservice_Amenity amenity in webserviceTrail.amenity)
							{
								if (amenity.id == existingAmenityId)
								{
									match = true;
								}
							}
						}
					}
					if (match == false)
					{
						deleteAmenities.Add(new Amenity(){id = existingAmenityId});
					}
				}

				// TODO: Populate the deletion lists for TrailsToActivities and TrailsToAmenities

				// TODO: Get the remaining database data.
				MapTile[] mapTiles = new MapTile[0];
				Media[] media = new Media[0];
				Point[] points = new Point[0];
				Role[] roles = new Role[0];

				// Update the rows that must be updated.
				dataLayer.updateRows (updateActivities.ToArray(), updateAmenities.ToArray(), mapTiles, media, updateOrganizations.ToArray(), points, roles, updateTrails.ToArray(),
					updateTrailsToActivities.ToArray(), updateTrailsToAmenities.ToArray(), updateUsers.ToArray());

				// Insert the rows that must be inserted.
				dataLayer.insertRows (activities.ToArray(), amenities.ToArray(), mapTiles, media, organizations.ToArray(), points, roles, trails.ToArray(), trailsToActivities.ToArray(),
					trailsToAmenities.ToArray(), users.ToArray());

				// Delete the rows that must be deleted.
				dataLayer.deleteRows (deleteActivities.ToArray (), deleteAmenities.ToArray (), mapTiles, media, deleteOrganizations.ToArray (), points, roles, deleteTrails.ToArray (),
					deleteTrailsToActivities.ToArray (), deleteTrailsToAmenities.ToArray (), deleteUsers.ToArray ());

				// Store the current time in the database as the last-updated time.
				dataLayer.setDatabaseLastUpdated (currentTime);

				Console.WriteLine ("Database Updated!");
			}
			catch (Exception e)
			{
				Console.WriteLine (e.Message);
			}
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

