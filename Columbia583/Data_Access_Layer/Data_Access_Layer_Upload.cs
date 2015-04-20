using System;

namespace Columbia583
{
	public class Data_Access_Layer_Upload
	{
		public Data_Access_Layer_Upload ()
		{
			
		}


		/// <summary>
		/// Uploads the trail to the server.  Will store the trail in the local database and upload
		/// it to the server if it is available.
		/// </summary>
		/// <param name="trail">Trail.</param>
		/// <param name="points">Points.</param>
		public void uploadTrail(Trail trail, Point[] points)
		{
			// Give the trail a random ID.
			// TODO: The ID should be based off the current timestamp appended with a random integer.
			Random rnd = new Random();
			trail.id = rnd.Next (1000, 100000);

			// Set the push to server flags.
			trail.pushToServer = true;
			foreach (Point point in points)
			{
				point.pushToServer = true;
			}

			// Get the active user.
			Data_Access_Layer_Common data_access_layer = new Data_Access_Layer_Common();
			User activeUser = data_access_layer.getActiveUser();

			// If the user is not logged in, don't allow an upload.
			if (activeUser == null) {
				return;
			}

			// Set the user's info in the trail's foreign keys.
			trail.userId = activeUser.id;
			trail.orgId = activeUser.orgId;

			// Insert the trail and its points into the database.
			Point[] pointsToInsert = points;
			Trail[] trailsToInsert = { trail };
			Data_Layer_Common dataLayer = new Data_Layer_Common();
			dataLayer.insertRows(new Activity[0], new Amenity[0], new FavouriteTrails[0], new MapTile[0], new Media[0], new Organization[0], pointsToInsert,
				new Role[0], trailsToInsert, new TrailsToActivities[0], new TrailsToAmenities[0], new User[0]);

			// Upload the points to the server.
			Service_Access_Layer_Upload serviceAccessLayerUpload = new Service_Access_Layer_Upload();
			bool pointsUploadSuccessful = serviceAccessLayerUpload.uploadPoints (points);

			// If the points upload was successful, update the database to indicate that the points were uploaded.
			if (pointsUploadSuccessful == true)
			{
				// Update the points in the database to indicate successful upload.
				foreach (Point point in points)
				{
					point.pushToServer = false;
				}
				Point[] pointsToUpdate = points;
				dataLayer.updateRows(new Activity[0], new Amenity[0], new Comment[0], new FavouriteTrails[0], new MapTile[0], new Media[0], new Organization[0],
					pointsToUpdate, new Role[0], new Trail[0], new TrailsToActivities[0], new TrailsToAmenities[0], new User[0]);

				// Upload the trail to the server.
				bool trailUploadSuccessful = serviceAccessLayerUpload.uploadTrail (trail);

				// If the trail upload was successful, update the database to indicate that the trail was uploaded.
				if (trailUploadSuccessful == true)
				{
					// Update the trail in the database to indicate successful upload.
					trail.pushToServer = false;
					Trail[] trailsToUpdate = { trail };
					dataLayer.updateRows(new Activity[0], new Amenity[0], new Comment[0], new FavouriteTrails[0], new MapTile[0], new Media[0], new Organization[0],
						new Point[0], new Role[0], trailsToUpdate, new TrailsToActivities[0], new TrailsToAmenities[0], new User[0]);
				}
			}
		}


		/// <summary>
		/// Uploads the user to the server.  Will store the user in the local database and upload
		/// it to the server if it is available.
		/// </summary>
		/// <param name="user">User.</param>
		public void uploadUser(User user)
		{
			// Give the user a random ID.
			// TODO: The ID should be based off the current timestamp appended with a random integer.
			Random rnd = new Random();
			user.id = rnd.Next (1000, 100000);

			// Set the push to server flag.
			user.pushToServer = true;

			// Insert the user into the database.
			User[] usersToInsert = { user };
			Data_Layer_Common dataLayer = new Data_Layer_Common();
			dataLayer.insertRows(new Activity[0], new Amenity[0], new FavouriteTrails[0], new MapTile[0], new Media[0], new Organization[0], new Point[0],
				new Role[0], new Trail[0], new TrailsToActivities[0], new TrailsToAmenities[0], usersToInsert);

			// Upload the user to the server.
			Service_Access_Layer_Upload serviceAccessLayerUpload = new Service_Access_Layer_Upload();
			bool uploadSuccessful = serviceAccessLayerUpload.uploadUser (user);

			// If the upload was successful, update the database to indicate that the user was uploaded.
			if (uploadSuccessful == true)
			{
				user.pushToServer = false;
				User[] usersToUpdate = { user };
				dataLayer.updateRows(new Activity[0], new Amenity[0], new Comment[0], new FavouriteTrails[0], new MapTile[0], new Media[0], new Organization[0],
					new Point[0], new Role[0], new Trail[0], new TrailsToActivities[0], new TrailsToAmenities[0], usersToUpdate);
			}
		}


		/// <summary>
		/// Uploads the comment to the server.  Will store the comment in the local database and upload
		/// it to the server if it is available.
		/// </summary>
		/// <param name="comment">Comment.</param>
		public void uploadComment(Comment comment)
		{
			// Give the comment a random ID.
			// TODO: The ID should be based off the current timestamp appended with a random integer.
			Random rnd = new Random();
			comment.id = rnd.Next (1000, 100000);

			// Set the push to server flag.
			comment.pushToServer = true;

			// Get the active user.
			Data_Access_Layer_Common data_access_layer = new Data_Access_Layer_Common();
			User activeUser = data_access_layer.getActiveUser();

			// If the user is not logged in, the comment is anonymous.
			if (activeUser != null)
			{
				comment.username = activeUser.username;
			}
			else
			{
				comment.username = "Anonymous";
			}

			// Insert the comment into the database.
			Data_Layer_Common dataLayer = new Data_Layer_Common();
			dataLayer.insertComment (comment);

			// Upload the comment to the server.
			Service_Access_Layer_Upload serviceAccessLayerUpload = new Service_Access_Layer_Upload();
			bool uploadSuccessful = serviceAccessLayerUpload.uploadComment (comment);

			// If the upload was successful, update the database to indicate that the media was uploaded.
			if (uploadSuccessful == true)
			{
				comment.pushToServer = false;
				Comment[] commentsToUpdate = { comment };
				dataLayer.updateRows(new Activity[0], new Amenity[0], commentsToUpdate, new FavouriteTrails[0], new MapTile[0], new Media[0],
					new Organization[0], new Point[0], new Role[0], new Trail[0], new TrailsToActivities[0], new TrailsToAmenities[0], new User[0]);
			}
		}


		/// <summary>
		/// Uploads the media to the server.  Will store the media in the local database and upload
		/// it to the server if it is available.
		/// </summary>
		/// <param name="media">Media.</param>
		public void uploadMedia(Media media)
		{
			// Give the media a random ID.
			// TODO: The ID should be based off the current timestamp appended with a random integer.
			Random rnd = new Random();
			media.id = rnd.Next (1000, 100000);

			// Set the push to server flag.
			media.pushToServer = true;

			// Insert the media into the database.
			Media[] mediaToInsert = { media };
			Data_Layer_Common dataLayer = new Data_Layer_Common();
			dataLayer.insertRows(new Activity[0], new Amenity[0], new FavouriteTrails[0], new MapTile[0], mediaToInsert, new Organization[0], new Point[0],
				new Role[0], new Trail[0], new TrailsToActivities[0], new TrailsToAmenities[0], new User[0]);

			// Upload the media to the server.
			Service_Access_Layer_Upload serviceAccessLayerUpload = new Service_Access_Layer_Upload();
			bool uploadSuccessful = serviceAccessLayerUpload.uploadMedia (media);

			// If the upload was successful, update the database to indicate that the media was uploaded.
			if (uploadSuccessful == true)
			{
				media.pushToServer = false;
				Media[] mediaToUpdate = { media };
				dataLayer.updateRows(new Activity[0], new Amenity[0], new Comment[0], new FavouriteTrails[0], new MapTile[0], mediaToUpdate,
					new Organization[0], new Point[0], new Role[0], new Trail[0], new TrailsToActivities[0], new TrailsToAmenities[0], new User[0]);
			}
		}
	}
}

