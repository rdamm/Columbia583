using System;

namespace Columbia583
{
	public class Data_Access_Layer_Upload
	{
		public Data_Access_Layer_Upload ()
		{
			
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

