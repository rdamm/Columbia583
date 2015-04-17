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

				// Get the IDs of the user's favourite trails.
				List<FavouriteTrails> favouriteTrailRows = connection.Query<FavouriteTrails>("SELECT * FROM FavouriteTrails WHERE FavouriteTrails.userId = ?", userId);

				// Get the details of each trail.
				foreach(FavouriteTrails favouriteTrailRow in favouriteTrailRows)
				{
					List<Point> points = new List<Point>();
					List<Activity> activities = new List<Activity>();
					List<Amenity> amenities = new List<Amenity>();

					// Get the general trail info.
					Trail trail = connection.Find<Trail>(favouriteTrailRow.trailId);

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
		/// Checks to see if the trail has been favourited by the given user.
		/// </summary>
		/// <returns><c>true</c>, if the trail is favourited, <c>false</c> otherwise.</returns>
		/// <param name="userId">User identifier.</param>
		/// <param name="trailId">Trail identifier.</param>
		public bool trailIsFavourited(int userId, int trailId)
		{
			bool favourited = false;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(Data_Layer_Common.getPathToDatabase());

				// Check to see if the trail was favourited.
				List<object> parameters = new List<object>();
				parameters.Add(userId);
				parameters.Add(trailId);
				int resultCount = connection.ExecuteScalar<int> ("SELECT COUNT(*) FROM FavouriteTrails WHERE userId = ? AND trailId = ?", parameters.ToArray());
				if (resultCount > 0)
				{
					favourited = true;
				}

				List<FavouriteTrails> trailResponse = connection.Query<FavouriteTrails>("SELECT * FROM FavouriteTrails WHERE userId = ? AND trailId = ?", parameters.ToArray());
				/*
				if (trailResponse != null && trailResponse.Count > 0)
				{
					favourited = true;
				}
				*/

				List<FavouriteTrails> allTrailsResponse = connection.Query<FavouriteTrails>("SELECT * FROM FavouriteTrails", new object[0]);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return favourited;
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
				List<FavouriteTrails> allTrailsBeforeResponse = connection.Query<FavouriteTrails>("SELECT * FROM FavouriteTrails", new object[0]);
				FavouriteTrails favouriteTrail = new FavouriteTrails(0, userId, trailId);
				connection.Insert(favouriteTrail);
				List<FavouriteTrails> allTrailsAfterResponse = connection.Query<FavouriteTrails>("SELECT * FROM FavouriteTrails", new object[0]);

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
				List<FavouriteTrails> response = connection.Query<FavouriteTrails>("SELECT * FROM FavouriteTrails WHERE userId = ? AND trailId = ?", args);
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

