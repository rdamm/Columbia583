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
		/// Converts the API's time to date time.  Returns the converted datetime, or the epoch's datetime if the conversion fails.
		/// </summary>
		/// <returns>The time to date time.</returns>
		public static DateTime convertTimeToDateTime(string apiTime)
		{
			try
			{
				return Convert.ToDateTime(apiTime);
			}
			catch (Exception e)
			{
				return new DateTime(1970, 1, 1);
			}
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
			// List<Point> points = null;

			try
			{
				trails = new List<Trail>();
				organizations = new List<Organization>();
				users = new List<User>();
				activities = new List<Activity>();
				amenities = new List<Amenity>();
				trailsToActivities = new List<TrailsToActivities>();
				trailsToAmenities = new List<TrailsToAmenities>();
				// points = new List<Point>();

				// For each trail retrieved from the webservice, parse the data into the local classes.
				foreach(Webservice_Trails currentTrail in webserviceTrails)
				{
					int userId = 0;
					int orgId = 0;

					// Get this trail's user.
					if (currentTrail.user != null)
					{
						// Convert the API's times into datetime objects.
						DateTime createdTime = convertTimeToDateTime(currentTrail.user.created_at);
						DateTime updatedTime = convertTimeToDateTime(currentTrail.user.updated_at);

						// Insert into the database.
						users.Add(new User(currentTrail.user.id, currentTrail.user.org_id, currentTrail.user.email, currentTrail.user.username, createdTime, updatedTime, false));
						userId = currentTrail.user.id;
					}

					// Get this trail's organization.
					if (currentTrail.organization != null)
					{
						// Convert the API's times into datetime objects.
						DateTime updatedTime = convertTimeToDateTime(currentTrail.organization.updated_at);

						// Insert into the database.
						organizations.Add(new Organization(currentTrail.organization.id, currentTrail.organization.organization, updatedTime));
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

							// Convert the API's times into datetime objects.
							DateTime updatedTime = convertTimeToDateTime(activity.updated_at);

							// Insert into the database.
							activities.Add(new Activity(activity.id, activity.name, imageBytes, updatedTime));
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

							// Convert the API's times into datetime objects.
							DateTime updatedTime = convertTimeToDateTime(amenity.updated_at);

							// Insert into the database.
							amenities.Add(new Amenity(amenity.id, amenity.name, imageBytes, updatedTime));
							trailsToAmenities.Add(new TrailsToAmenities(currentTrail.id, amenity.id));
						}
					}

					/*
					 * if (currentTrail.point != null && currentTrail.point.Count > 0)
					 * {
					 * 		foreach (Webservice_Point point in currentTrail.point)
					 * 		{
					 * 			points.Add(new Point(point.id, point.trailId, point.mapTileId, point.nextPointId, point.title, point.description, point.lat, point.lon, point.primary, point.timestamp));
					 * 		}
					 * }
					 */

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

					// Convert the API's times into datetime objects.
					DateTime trailUpdatedTime = convertTimeToDateTime(currentTrail.updated_at);

					// Get the base trail.
					trails.Add(new Trail(currentTrail.id, userId, orgId, currentTrail.name, currentTrail.location, currentTrail.kml_name, currentTrail.kml_content, currentTrail.distance,
						currentTrail.duration, currentTrail.description, currentTrail.directions, trailDifficulty, currentTrail.rating, currentTrail.hazards, currentTrail.surface,
						currentTrail.landAccess, currentTrail.maintenance, currentTrail.season, trailOpen, currentTrail.active, trailUpdatedTime,Convert.ToDateTime(currentTrail.created_at), true));
				}

				// Remove duplicate entries in the resulting lists.
				trails = trails.Distinct().ToList();
				organizations = organizations.Distinct().ToList();
				users = users.Distinct().ToList();
				activities = activities.Distinct().ToList();
				amenities = amenities.Distinct().ToList();
				trailsToActivities = trailsToActivities.Distinct().ToList();
				trailsToAmenities = trailsToAmenities.Distinct().ToList();
				// points = points.Distinct().ToList();
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

			// Default the favourite trails to an empty list.  They are not stored on the server.
			FavouriteTrails[] favouriteTrails = new FavouriteTrails[0];
			
			// Store the data in the local database.
			dataLayer.insertRows (activities.ToArray(), amenities.ToArray(), favouriteTrails, mapTiles, media, organizations.ToArray(), points, roles, trails.ToArray(),
				trailsToActivities.ToArray(), trailsToAmenities.ToArray(), users.ToArray());

			// Store the current time in the database as the last-updated time.
			Data_Layer_App_Globals dataLayerAppGlobals = new Data_Layer_App_Globals();
			dataLayerAppGlobals.setDatabaseLastUpdated (currentTime);

			Console.WriteLine ("Database Initialized!");
		}

		public void initializeComments()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			Service_Access_Layer_Common serviceAccessLayer = new Service_Access_Layer_Common ();
			dataLayer.createCommentTable ();

			// Get the current time prior to calling the webservice.
			DateTime currentTime = DateTime.Now;

			// Get the data from the webservice.
			List<Webservice_Comment> webserviceComments = serviceAccessLayer.getComments ();

			List<Comment> comments = new List<Comment> ();

			// real comments
			try {
				foreach (Webservice_Comment currentComment in webserviceComments) {
					string username;
					if (currentComment.user != null) {
						username = currentComment.user.username;
					} else {
						username = "Anonymous";
					}

					// Convert the API's times into datetime objects.
					DateTime updatedTime = convertTimeToDateTime(currentComment.updated_at);

					// Get base comment
					comments.Add(new Comment(currentComment.id, currentComment.trail_id, currentComment.comment, currentComment.rating, username, updatedTime));
				}

				comments = comments.Distinct ().ToList ();
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}

			dataLayer.insertCommentRows (comments.ToArray ());

			// Store the current time in the database as the last-updated time.
			Data_Layer_App_Globals dataLayerAppGlobals = new Data_Layer_App_Globals();
			dataLayerAppGlobals.setDatabaseLastUpdated (currentTime);

			Console.WriteLine ("Comments Initialized!");
		}


		/// <summary>
		/// Updates the local database so that it is up-to-date with the live database.  This should be called on a regular interval.
		/// Also updates comments!
		/// </summary>
		public void updateDatabase()
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			Service_Access_Layer_Common serviceAccessLayer = new Service_Access_Layer_Common ();

			// Get the current time prior to calling the webservice.
			DateTime currentTime = DateTime.Now;

			// Get the last time the database was updated.
			Data_Layer_App_Globals dataLayerAppGlobals = new Data_Layer_App_Globals();
			DateTime lastUpdated = dataLayerAppGlobals.getDatabaseLastUpdated ();

			// Get the data from the webservice.
			string urlDate = lastUpdated.ToString("yyyy-MM-dd");
			string commentUrlDate = lastUpdated.ToString ("ddMMMyyyy");
			List<Webservice_Trails> webserviceTrails_getAll = serviceAccessLayer.getAll ();
			List<Webservice_Trails> webserviceTrails = serviceAccessLayer.updateAll (urlDate);
			List<Webservice_Comment> webserviceComments_getAll = serviceAccessLayer.getComments ();
			List<Webservice_Comment> webserviceComments = serviceAccessLayer.updateComments (commentUrlDate);

			// Define lists to copy the data to.
			List<Trail> insertTrails = null;
			List<Organization> insertOrganizations = null;
			List<User> insertUsers = null;
			List<Activity> insertActivities = null;
			List<Amenity> insertAmenities = null;
			List<Comment> insertComments = null;
			List<Point> insertPoints = null;

			List<Trail> updateTrails = null;
			List<Organization> updateOrganizations = null;
			List<User> updateUsers = null;
			List<Activity> updateActivities = null;
			List<Amenity> updateAmenities = null;
			List<Comment> updateComments = null;
			List<Point> updatePoints = null;

			List<Trail> deleteTrails = null;
			List<Organization> deleteOrganizations = null;
			List<User> deleteUsers = null;
			List<Activity> deleteActivities = null;
			List<Amenity> deleteAmenities = null;
			List<Comment> deleteComments = null;
			List<Point> deletePoints = null;

			List<TrailsToActivities> trailsToActivities = null;
			List<TrailsToAmenities> trailsToAmenities = null;
			List<TrailsToActivities> remainingTrailsToActivities = null;
			List<TrailsToAmenities> remainingTrailsToAmenities = null;

			try
			{
				insertTrails = new List<Trail>();
				insertOrganizations = new List<Organization>();
				insertUsers = new List<User>();
				insertActivities = new List<Activity>();
				insertAmenities = new List<Amenity>();
				insertComments = new List<Comment>();
				insertPoints = new List<Point>();

				updateTrails = new List<Trail>();
				updateOrganizations = new List<Organization>();
				updateUsers = new List<User>();
				updateActivities = new List<Activity>();
				updateAmenities = new List<Amenity>();
				updateComments = new List<Comment>();
				updatePoints = new List<Point>();

				deleteTrails = new List<Trail>();
				deleteOrganizations = new List<Organization>();
				deleteUsers = new List<User>();
				deleteActivities = new List<Activity>();
				deleteAmenities = new List<Amenity>();
				deleteComments = new List<Comment>();
				deletePoints = new List<Point>();

				trailsToActivities = new List<TrailsToActivities>();
				trailsToAmenities = new List<TrailsToAmenities>();

				// Get the IDs for the existing database rows.
				List<int> existingTrailIds = new List<int>(dataLayer.getTrailIds());
				List<int> existingUserIds = new List<int>(dataLayer.getUserIds());
				List<int> existingOrganizationIds = new List<int>(dataLayer.getOrganizationIds());
				List<int> existingActivityIds = new List<int>(dataLayer.getActivityIds());
				List<int> existingAmenityIds = new List<int>(dataLayer.getAmenityIds());
				List<int> existingCommentIds = new List<int>(dataLayer.getCommentIds());
				List<int> existingPointIds = new List<int>(dataLayer.getPointIds());
				List<TrailsToActivities> existingTrailsToActivities = new List<TrailsToActivities>(dataLayer.getTrailsToActivities());
				List<TrailsToAmenities> existingTrailsToAmenities = new List<TrailsToAmenities>(dataLayer.getTrailsToAmenities());
				remainingTrailsToActivities = new List<TrailsToActivities>(dataLayer.getTrailsToActivities());
				remainingTrailsToAmenities = new List<TrailsToAmenities>(dataLayer.getTrailsToAmenities());

				// Insert / update rows in the database.
				foreach(Webservice_Trails currentTrail in webserviceTrails)
				{
					int userId = 0;
					int orgId = 0;

					remainingTrailsToActivities.RemoveAll(i => i.trailId == currentTrail.id);
					remainingTrailsToAmenities.RemoveAll(i => i.trailId == currentTrail.id);

					// Get this trail's user.
					if (currentTrail.user != null)
					{
						// Convert the API's times into datetime objects.
						DateTime createdTime = convertTimeToDateTime(currentTrail.user.created_at);
						DateTime updatedTime = convertTimeToDateTime(currentTrail.user.updated_at);

						// If the user exists, update it.  Otherwise, insert it.
						if(dataLayer.getUser(currentTrail.user.id) != null)
						{
							updateUsers.Add(new User(currentTrail.user.id, currentTrail.user.org_id, currentTrail.user.email, currentTrail.user.username, createdTime, updatedTime, false));
						}
						else
						{
							insertUsers.Add(new User(currentTrail.user.id, currentTrail.user.org_id, currentTrail.user.email, currentTrail.user.username, createdTime, updatedTime, false));
						}
						userId = currentTrail.user.id;
					}

					// Get this trail's organization.
					if (currentTrail.organization != null)
					{
						// Convert the API's times into datetime objects.
						DateTime updatedTime = convertTimeToDateTime(currentTrail.organization.updated_at);

						// If the organization exists, update it.  Otherwise, insert it.
						if(dataLayer.getOrganization(currentTrail.organization.id) != null)
						{
							updateOrganizations.Add(new Organization(currentTrail.organization.id, currentTrail.organization.organization, updatedTime));
						}
						else
						{
							insertOrganizations.Add(new Organization(currentTrail.organization.id, currentTrail.organization.organization, updatedTime));
						}
						orgId = currentTrail.organization.id;
					}

					// Get this trail's activities.
					if (currentTrail.activity != null && currentTrail.activity.Count > 0)
					{
						foreach(Webservice_Activity activity in currentTrail.activity)
						{
							// Fetch the activity icon's image from the net.
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

							// Convert the API's times into datetime objects.
							DateTime updatedTime = convertTimeToDateTime(activity.updated_at);

							// If the activity exists, update it.  Otherwise, insert it.
							if(dataLayer.getActivity(activity.id) != null)
							{
								updateActivities.Add(new Activity(activity.id, activity.name, imageBytes, updatedTime));
							}
							else
							{
								insertActivities.Add(new Activity(activity.id, activity.name, imageBytes, updatedTime));
							}

							// Add this activity to the trail's activities.
							trailsToActivities.Add(new TrailsToActivities(currentTrail.id, activity.id));
						}
					}

					// Get this trail's amenities.
					if (currentTrail.amenity != null && currentTrail.amenity.Count > 0)
					{
						foreach(Webservice_Amenity amenity in currentTrail.amenity)
						{
							// Fetch the amenity icon's image from the net.
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

							// Convert the API's times into datetime objects.
							DateTime updatedTime = convertTimeToDateTime(amenity.updated_at);

							// If the amenity exists, update it.  Otherwise, insert it.
							if(dataLayer.getAmenity(amenity.id) != null)
							{
								updateAmenities.Add(new Amenity(amenity.id, amenity.name, imageBytes, updatedTime));
							}
							else
							{
								insertAmenities.Add(new Amenity(amenity.id, amenity.name, imageBytes, updatedTime));
							}

							// Add this amenity to the trail's amenities.
							trailsToAmenities.Add(new TrailsToAmenities(currentTrail.id, amenity.id));
						}
					}
						
					/*
					 * if (currentTrail.point != null && currentTrail.point.Count > 0)
					 * {
					 * 		foreach (Webservice_Point point in currentTrail.point)
					 * 		{
					 * 			if (dataLayet.getPoint(point.id) != null)
					 * 			{
					 * 				updatePoints.Add(new Point(point.id, point.trailId, point.mapTileId, point.nextPointId, point.title, point.description, point.lat, point.lon, point.primary, point.timestamp));
					 * 			}
					 * 			else
					 * 			{
					 * 				insertPoints.Add(new Point(point.id, point.trailId, point.mapTileId, point.nextPointId, point.title, point.description, point.lat, point.lon, point.primary, point.timestamp));
					 * 			}
					 * 		}
					 * }
					 */

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

					// Convert the API's times into datetime objects.
					DateTime trailUpdatedTime = convertTimeToDateTime(currentTrail.updated_at);

					// If the base trail exists, update it.  Otherwise, insert it.
					if(dataLayer.getTrail(currentTrail.id) != null)
					{
						updateTrails.Add(new Trail(currentTrail.id, userId, orgId, currentTrail.name, currentTrail.location, currentTrail.kml_name, currentTrail.kml_content, currentTrail.distance,
							currentTrail.duration, currentTrail.description, currentTrail.directions, trailDifficulty, currentTrail.rating, currentTrail.hazards, currentTrail.surface,
							currentTrail.landAccess, currentTrail.maintenance, currentTrail.season, trailOpen, currentTrail.active, trailUpdatedTime, Convert.ToDateTime(currentTrail.created_at), true));
					}
					else
					{
						insertTrails.Add(new Trail(currentTrail.id, userId, orgId, currentTrail.name, currentTrail.location, currentTrail.kml_name, currentTrail.kml_content, currentTrail.distance,
							currentTrail.duration, currentTrail.description, currentTrail.directions, trailDifficulty, currentTrail.rating, currentTrail.hazards, currentTrail.surface,
							currentTrail.landAccess, currentTrail.maintenance, currentTrail.season, trailOpen, currentTrail.active, trailUpdatedTime, Convert.ToDateTime(currentTrail.created_at), true));
					}
				}

				foreach(Webservice_Comment currentComment in webserviceComments)
				{
					// Convert the API's times into datetime objects.
					DateTime updatedTime = convertTimeToDateTime(currentComment.updated_at);

					// Get base comment
					if (dataLayer.getComment(currentComment.id) != null) {
						updateComments.Add(new Comment(currentComment.id, currentComment.trail_id, currentComment.comment, currentComment.rating, currentComment.user.username, updatedTime));
					} else {
						insertComments.Add(new Comment(currentComment.id, currentComment.trail_id, currentComment.comment, currentComment.rating, currentComment.user.username, updatedTime));
					}
				}

				// Remove duplicate entries in the resulting lists.
				insertTrails = insertTrails.Distinct().ToList();
				insertOrganizations = insertOrganizations.Distinct().ToList();
				insertUsers = insertUsers.Distinct().ToList();
				insertActivities = insertActivities.Distinct().ToList();
				insertAmenities = insertAmenities.Distinct().ToList();
				insertComments = insertComments.Distinct().ToList();

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
						if (webserviceTrail.user != null && webserviceTrail.user.id == existingUserId)
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
						if (webserviceTrail.organization != null && webserviceTrail.organization.id == existingOrganizationId)
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

				// Find the comments to delete.
				foreach(int existingCommentId in existingCommentIds)
				{
					bool match = false;
					foreach(Webservice_Comment webserviceComment in webserviceComments_getAll)
					{
						if (webserviceComment.id == existingCommentId)
						{
							match = true;
						}
					}
					if (!match)
					{
						deleteComments.Add(new Comment(){id = existingCommentId});
					}
				}

				// Find the points to delete.
				foreach(int existingPointId in existingPointIds)
				{
					bool match = false;
					/*
					* foreach(Webservice_Trail webserviceTrail in webserviceTrails_getAll)
					* {
					* 	if (webserviceTrail.point != null && webserviceTrail.point.Count > 0)
					* 	{
					* 		foreach (Webservice_Point point in webserviceTrail.point)
					* 		{
					* 			if (point.id == existingPointId)
					* 				match = true;
					* 		}
					* 	}
					* }
					* if (!match)
					* {
					* 	deletePoints.Add(new Point(){id = existingPointId});
					* }
					*/
				}

				// Delete ALL entries in the pairing tables.
				dataLayer.deleteRows(new Activity[0], new Amenity[0], new Comment[0], new FavouriteTrails[0], new MapTile[0], new Media[0], new Organization[0],
					new Point[0], new Role[0], new Trail[0], existingTrailsToActivities.ToArray(), existingTrailsToAmenities.ToArray(), new User[0]);

				// TODO: Get the remaining database data.
				MapTile[] mapTiles = new MapTile[0];
				Media[] media = new Media[0];
				Point[] points = new Point[0];
				Role[] roles = new Role[0];

				// Do not update the favourite trails.
				FavouriteTrails[] favouriteTrails = new FavouriteTrails[0];

				// Fuse all the pairing table data together
				var enumTrailsToActivities = trailsToActivities.Concat(remainingTrailsToActivities);
				var enumTrailsToAmenities = trailsToAmenities.Concat(remainingTrailsToAmenities);
				trailsToActivities = enumTrailsToActivities.ToList();
				trailsToAmenities = enumTrailsToAmenities.ToList();

				foreach (Activity activity in deleteActivities)
				{
					trailsToActivities.RemoveAll(i => i.activityId == activity.id);
				}
				foreach (Amenity amenity in deleteAmenities)
				{
					trailsToAmenities.RemoveAll(i => i.amenityId == amenity.id);
				}
				foreach (Trail trail in deleteTrails)
				{
					trailsToActivities.RemoveAll(i => i.trailId == trail.id);
					trailsToAmenities.RemoveAll(i => i.trailId == trail.id);
				}

				// Delete all Points with deleted Trails
				// Should be done above
				// Delete all Trails with deleted Organizations
				foreach (Organization org in deleteOrganizations)
				{
					foreach (Webservice_Trails webTrail in webserviceTrails_getAll)
					{
						if (webTrail.organization != null && webTrail.orgId == org.id)
						{
							deleteTrails.Add(new Trail(){id = webTrail.id});
						}
					}

					foreach (User u in dataLayer.getUsers()) {
						if (u.orgId == org.id) {
							updateUsers.Add(new User(u.id, 0, u.email, u.username, currentTime, currentTime, false));
						}
					}
				}
				deleteTrails = deleteTrails.Distinct().ToList();
				updateUsers = updateUsers.Distinct().ToList();


				// Try nulling nullable fields
				// See above for nulling User's Organization
				// Update all Trails with deleted Users
				foreach (Webservice_Trails currentTrail in webserviceTrails_getAll) {
					foreach (User u in deleteUsers) {
						int orgId = 0;
						if (currentTrail.organization != null) {
							orgId = currentTrail.organization.id;
						}
						// trailOpen
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
						if (currentTrail.user != null && currentTrail.user.id == u.id) {
							updateTrails.Add(new Trail(currentTrail.id, currentTrail.user.id, orgId, currentTrail.name, currentTrail.location, currentTrail.kml_name, currentTrail.kml_content, currentTrail.distance,
								currentTrail.duration, currentTrail.description, currentTrail.directions, trailDifficulty, currentTrail.rating, currentTrail.hazards, currentTrail.surface,
								currentTrail.landAccess, currentTrail.maintenance, currentTrail.season, trailOpen, currentTrail.active, currentTime, Convert.ToDateTime(currentTrail.created_at), true));
						}
					}
				}
				updateTrails = updateTrails.Distinct().ToList();

				// Update the rows that must be updated.
				dataLayer.updateRows (updateActivities.ToArray(), updateAmenities.ToArray(), updateComments.ToArray(), favouriteTrails, mapTiles, media, updateOrganizations.ToArray(),
					points, roles, updateTrails.ToArray(), new TrailsToActivities[0], new TrailsToAmenities[0], updateUsers.ToArray());

				// Insert the rows that must be inserted.
				dataLayer.insertRows (insertActivities.ToArray(), insertAmenities.ToArray(), favouriteTrails, mapTiles, media, insertOrganizations.ToArray(), points, roles,
					insertTrails.ToArray(), trailsToActivities.ToArray(), trailsToAmenities.ToArray(), insertUsers.ToArray());
				dataLayer.insertCommentRows(insertComments.ToArray());

				// Delete the rows that must be deleted.
				dataLayer.deleteRows (deleteActivities.ToArray (), deleteAmenities.ToArray (), deleteComments.ToArray(), favouriteTrails, mapTiles, media, deleteOrganizations.ToArray (),
					points, roles, deleteTrails.ToArray (), new TrailsToActivities[0], new TrailsToAmenities[0], deleteUsers.ToArray ());

				// Store the current time in the database as the last-updated time.
				dataLayerAppGlobals.setDatabaseLastUpdated (currentTime);

				Console.WriteLine ("Database Updated!");
			}
			catch (Exception e)
			{
				Console.WriteLine (e.Message);
			}
		}


		/// <summary>
		/// Gets the active user.
		/// </summary>
		/// <returns>The active user.</returns>
		public User getActiveUser()
		{
			// Get the active user's ID.
			Data_Layer_App_Globals dataLayerAppGlobals = new Data_Layer_App_Globals ();
			int activeUserId = dataLayerAppGlobals.getActiveUserId ();

			// Get the active user from the ID.
			Data_Layer_Common dataLayerCommon = new Data_Layer_Common();
			return dataLayerCommon.getUser (activeUserId);
		}


		/// <summary>
		/// Sets the active user.
		/// </summary>
		/// <param name="userId">User identifier.</param>
		public void setActiveUser(int userId)
		{
			// Set the active user's ID.
			Data_Layer_App_Globals dataLayerAppGlobals = new Data_Layer_App_Globals();
			dataLayerAppGlobals.setActiveUserId (userId);
		}


		/// <summary>
		/// Gets the user by their email.
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="email">Email.</param>
		public User getUserByEmail(string email)
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			return dataLayer.getUserByEmail (email);
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


		/// <summary>
		/// Gets the media.
		/// </summary>
		/// <returns>The media.</returns>
		/// <param name="id">Identifier.</param>
		public Media getMedia(int id)
		{
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			return dataLayer.getMedia(id);
		}

		//insert comment, call to data layer common
		public void insertComment(Comment comment){
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			dataLayer.insertComment (comment);
		}

		//insert trail, call to data layer common
		public void insertTrail(Trail trail){
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			dataLayer.insertTrail (trail);
		}

		//get a specific comment id.
		public Comment getComment(int id){
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			return dataLayer.getComment (id);
		}

		//get all comments
		public List<Comment> getCommentID(){
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			return dataLayer.getCommentid ();
		}

		/// <summary>
		/// get a specific trail id.
		/// </summary>
		/// <returns>The trail I.</returns>
		/// <param name="id">Identifier.</param>
		public Trail getTrailID(int id){
			Data_Layer_Common dataLayer = new Data_Layer_Common ();
			return dataLayer.getTrailid (id);
		}

	}
}

