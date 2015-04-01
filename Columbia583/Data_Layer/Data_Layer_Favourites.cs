using System;
using SQLite;
using System.Collections.Generic;

namespace Columbia583
{
	public class Data_Layer_Favourites
	{
		public Data_Layer_Favourites ()
		{
			
		}


		/// <summary>
		/// Gets the user's favourite trails.
		/// </summary>
		/// <returns>The favourite trails.</returns>
		/// <param name="userId">User identifier.</param>
		public List<ListableTrail> getFavouriteTrails(int userId)
		{
			List<ListableTrail> favouriteTrails = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(Data_Layer_Common.getPathToDatabase());

				favouriteTrails = new List<ListableTrail>();
				
				// Get the trails from the user's favourites.
				List<Trail> trails = new List<Trail>();
				var trailResponse = connection.Query<Trail>("SELECT * FROM Trail INNER JOIN FavouriteTrails ON Trail.id = FavouriteTrails.trailId WHERE FavouriteTrails.userId = ?", userId);
				foreach(Trail trail in trailResponse)
				{
					trails.Add(trail);
				}

				// Get the details for each trail.
				foreach(Trail trail in trails)
				{
					List<Point> points = new List<Point>();
					List<Activity> activities = new List<Activity>();
					List<Amenity> amenities = new List<Amenity>();

					// Get the points.
					var pointsQueryResponse = connection.Query<Point>("SELECT * FROM Point WHERE trailId = ?", trail.id);
					foreach(Point point in pointsQueryResponse)
					{
						points.Add(point);
					}

					// Get the activities.
					var activitiesQueryResponse = connection.Query<Activity>("SELECT * FROM Activity INNER JOIN TrailsToActivities ON Activity.id = TrailsToActivities.activityId WHERE trailId = ?", trail.id);
					foreach(Activity activity in activitiesQueryResponse)
					{
						activities.Add(activity);
					}

					// Get the amenities.
					var amenitiesQueryResponse = connection.Query<Amenity>("SELECT * FROM Amenity INNER JOIN TrailsToAmenities ON Amenity.id = TrailsToAmenities.amenityId WHERE trailId = ?", trail.id);
					foreach(Amenity amenity in amenitiesQueryResponse)
					{
						amenities.Add(amenity);
					}

					// Encapsulate the data into a listable trail.
					ListableTrail listableTrail = new ListableTrail(trail, points.ToArray(), activities.ToArray(), amenities.ToArray());
					favouriteTrails.Add(listableTrail);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return favouriteTrails;
		}


		/// <summary>
		/// Adds the trail to the user's favourites.
		/// </summary>
		/// <param name="userId">User identifier.</param>
		/// <param name="trailId">Trail identifier.</param>
		public void addFavouriteTrail(int userId, int trailId)
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(Data_Layer_Common.getPathToDatabase());

				// Add the trail to the user's favourites.
				FavouriteTrails favouriteTrail = new FavouriteTrails(userId, trailId);
				connection.Insert(favouriteTrail);
				
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
		/// Removes the trail from the user's favourites.
		/// </summary>
		/// <param name="userId">User identifier.</param>
		/// <param name="trailId">Trail identifier.</param>
		public void removeFavouriteTrail(int userId, int trailId)
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(Data_Layer_Common.getPathToDatabase());

				// Delete the favourites with matching user ID and trail ID.
				object[] args = new object[2];
				args[0] = userId;
				args[1] = trailId;
				var response = connection.Query<FavouriteTrails>("SELECT * FROM FavouriteTrails WHERE userId = ? AND trailId = ?", args);
				foreach(FavouriteTrails favouriteTrail in response)
				{
					connection.Delete(favouriteTrail);
				}

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

