using System;

namespace Columbia583
{
	public class Data_Access_Layer_Tests
	{
		public Data_Access_Layer_Tests ()
		{

		}


		public string initializeDatabase()
		{
			Data_Access_Layer_Common dataAccessLayer = new Data_Access_Layer_Common ();
			dataAccessLayer.initializeDatabase ();

			return "Successfully initialized the database.";
		}


		public string updateDatabase()
		{
			Data_Access_Layer_Common dataAccessLayer = new Data_Access_Layer_Common ();
			dataAccessLayer.updateDatabase ();

			return "Successfully updated the database.";
		}


		public string getActivities()
		{
			Data_Access_Layer_Common dataAccessLayer = new Data_Access_Layer_Common ();
			Activity[] activities = dataAccessLayer.getActivities ();
			if (activities != null) {
				foreach (Activity activity in activities) {
					Console.WriteLine (activity.id + " - " + activity.activityName);
				}
				return "Successfully got activities.";
			} else {
				return "Failed to get activities.";
			}
		}


		public string getAmenities()
		{
			Data_Access_Layer_Common dataAccessLayer = new Data_Access_Layer_Common ();
			Amenity[] amenities = dataAccessLayer.getAmenities ();
			if (amenities != null) {
				foreach (Amenity amenity in amenities) {
					Console.WriteLine (amenity.id + " - " + amenity.amenityName);
				}
				return "Successfully got amenities.";
			} else {
				return "Failed to get amenities.";
			}
		}
	}
}

