using System;

namespace Columbia583
{
	public class Data_Access_Layer_Upload
	{
		public Data_Access_Layer_Upload ()
		{
		}


		/// <summary>
		/// Uploads the media to the server.  Will store the media in the local database and upload
		/// it to the server if it is available.
		/// </summary>
		/// <param name="media">Media.</param>
		public void uploadMedia(Media media)
		{
			// TODO: Insert the media into the database.
			Data_Layer_Common dataLayer = new Data_Layer_Common();

			// Upload the media to the server.
			Service_Access_Layer_Upload serviceAccessLayerUpload = new Service_Access_Layer_Upload();
			bool uploadSuccessful = serviceAccessLayerUpload.uploadMedia (media);

			// If the upload was successful, update the database to indicate that the media was uploaded.
			if (uploadSuccessful == true) {
				// TODO: Update the database.
			}
		}
	}
}

