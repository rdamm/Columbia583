using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Columbia583
{
	/// <summary>
	/// The search trails data layer performs all the SQLite queries relating to the search trails page.
	/// </summary>
	public class Data_Layer_Search_Trails
	{
		public Data_Layer_Search_Trails ()
		{

		}

		
		/// <summary>
		/// Gets the trails by search filter.
		/// </summary>
		/// <returns>The trails by search filter.</returns>
		/// <param name="searchFilter">Search filter.</param>
		public List<ListableTrail> getTrailsBySearchFilter(SearchFilter searchFilter)
		{
			// Search filter parameters should be split by their type and ANDed together.  Each type will have different specific logic, but all rows should be met to be a match.
			// eg. WHERE (
			//		(activity = hiking OR activity = biking OR activity = skiing) AND
			//		(amenity = washrooms OR amenity = campground OR amenity = picnic area) AND
			//		(difficulty = 3 OR difficulty = 5) AND
			//		(rating >= 3) AND
			//		(duration >= 1 AND duration <= 5) AND
			//		(distance >= 2 AND distance <= 6)
			// )

			List<ListableTrail> searchResults = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(Data_Layer_Common.getPathToDatabase());

				// Build the search parameter lines and keep track of their parameters.
				List<string> lines = new List<string>();
				List<object> parameters = new List<object>();
				// (id IN (SELECT * FROM Trail INNER JOIN TrailsToActivities ON Trail.id = TrailsToActivities.trailId WHERE activityId = ?)) AND (id IN (...)), activity, nextActivity
				if (searchFilter.activities != null && searchFilter.activities.Length > 0)
				{
					string activityLine = "(";
					bool firstActivity = true;
					foreach(int activity in searchFilter.activities)
					{
						// TODO: Require a join of the lookup tables to reference foreign keys.
						if (firstActivity)
						{
							firstActivity = false;
						}
						else
						{
							// Using Greenways website's AND
							activityLine += " AND ";
						}
						activityLine += "(Trail.id IN (SELECT Trail.id FROM Trail INNER JOIN TrailsToActivities ON Trail.id = TrailsToActivities.trailId WHERE activityId = ?))";
						parameters.Add(activity);
					}
					activityLine += ")";
					// TODO: Add the line and its parameters to the lists.
					lines.Add(activityLine);
				}
				if (searchFilter.amenities != null && searchFilter.amenities.Length > 0)
				{
					string amenityLine = "(";
					bool firstAmenity = true;
					foreach(int amenity in searchFilter.amenities)
					{
						// TODO: Require a join of the lookup tables to reference foreign keys.
						if (firstAmenity)
						{
							firstAmenity = false;
						}
						else
						{
							amenityLine += " AND ";
						}
						amenityLine += "(Trail.id IN (SELECT Trail.id FROM Trail INNER JOIN TrailsToAmenities ON Trail.id = TrailsToAmenities.trailId WHERE amenityId = ?))";
						parameters.Add(amenity);
					}
					amenityLine += ")";
					// TODO: Add the line and its parameters to the lists.
					lines.Add(amenityLine);
				}
				if (searchFilter.difficulty != 0)
				{
					string difficultyLine = "(";
					bool firstDiff = true;
//					foreach(Difficulty d in searchFilter.difficulty)
//					{
//						if (firstDiff)
//						{
//							firstDiff = false;
//						}
//						else
//						{
//							difficultyLine += " OR ";
//						}
						difficultyLine += "difficulty = ?";
					parameters.Add((int)searchFilter.difficulty);
					//}
					difficultyLine += ")";
					lines.Add(difficultyLine);
				}
				if (searchFilter.rating != 0)
				{
					string ratingLine = "(rating >= ?)";
					lines.Add(ratingLine);
					parameters.Add(searchFilter.rating);
				}
				if (searchFilter.minDuration != 0 && searchFilter.maxDuration != 0)
				{
					string durationLine = "(duration >= ? AND duration <= ?)";
					lines.Add(durationLine);
					parameters.Add(searchFilter.minDuration);
					parameters.Add(searchFilter.maxDuration);
				}
				else if (searchFilter.minDuration != 0)
				{
					string durationLine = "(duration >= ?)";
					lines.Add(durationLine);
					parameters.Add(searchFilter.minDuration);
				}
				else if (searchFilter.maxDuration != 0)
				{
					string durationLine = "(duration <= ?)";
					lines.Add(durationLine);
					parameters.Add(searchFilter.maxDuration);
				}
				if (searchFilter.minDistance != 0 && searchFilter.maxDistance != 0)
				{
					string distanceLine = "(distance >= ? AND distance <= ?)";
					lines.Add(distanceLine);
					parameters.Add(searchFilter.minDistance);
					parameters.Add(searchFilter.maxDistance);
				}
				else if (searchFilter.minDistance != 0)
				{
					string distanceLine = "(distance >= ?)";
					lines.Add(distanceLine);
					parameters.Add(searchFilter.minDistance);
				}
				else if (searchFilter.maxDistance != 0)
				{
					string distanceLine = "(distance <= ?)";
					lines.Add(distanceLine);
					parameters.Add(searchFilter.maxDistance);
				}

				// Take the search parameter lines and create the WHERE query.
				string whereQuery = "WHERE (";
				bool firstLine = true;
				foreach(string parameterLine in lines)
				{
					// Prepend the line with AND, unless it is the first line.
					if (firstLine == true)
					{
						firstLine = false;
					}
					else
					{
						whereQuery += " AND ";
					}

					whereQuery += parameterLine;
				}
				whereQuery += ")";

				// Get all trails that match the search filter.
				List<Trail> response;

				//test to see if whereQuery is empty, if so get all trails by default.
				if(whereQuery == "WHERE ()"){
					 response = connection.Query<Trail>("SELECT * FROM Trail");
				}else
					 response = connection.Query<Trail>("SELECT * FROM Trail " + whereQuery, parameters.ToArray());

				// For each matching trail, get its points, activities, and amenities.
				searchResults = new List<ListableTrail>();
				foreach (Trail trailRow in response)
				{
					List<Point> points = new List<Point>();
					List<Activity> activities = new List<Activity>();
					List<Amenity> amenities = new List<Amenity>();

					// Get the points.
					var pointsQueryResponse = connection.Query<Point>("SELECT * FROM Point WHERE trailId = ?", trailRow.id);
					foreach(Point point in pointsQueryResponse)
					{
						points.Add(point);
					}

					// Get the activities.
					var activitiesQueryResponse = connection.Query<Activity>("SELECT * FROM Activity INNER JOIN TrailsToActivities ON Activity.id = TrailsToActivities.activityId WHERE trailId = ?", trailRow.id);
					foreach(Activity activity in activitiesQueryResponse)
					{
						activities.Add(activity);
					}

					// Get the amenities.
					var amenitiesQueryResponse = connection.Query<Amenity>("SELECT * FROM Amenity INNER JOIN TrailsToAmenities ON Amenity.id = TrailsToAmenities.amenityId WHERE trailId = ?", trailRow.id);
					foreach(Amenity amenity in amenitiesQueryResponse)
					{
						amenities.Add(amenity);
					}

					// Encapsulate the data into a search result and add it to the list.
					ListableTrail searchResult = new ListableTrail(trailRow, points.ToArray(), activities.ToArray(), amenities.ToArray());
					searchResults.Add(searchResult);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return searchResults;
		}

		public int getActivityIdByName(string name)
		{
			int activityId = -1;
			Console.WriteLine ("Activity name: " + name);
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(Data_Layer_Common.getPathToDatabase());

				// Get the user.
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				Activity acti = connection.Query<Activity>("SELECT * FROM Activity WHERE activityName = ?", name)[0];
				activityId = acti.id;

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			Console.WriteLine ("Activity name: " + name + ", Activity id: ", + activityId);
			return activityId;
		}

		public int getAmenityIdByName(string name)
		{
			int amenityId = -1;
			Console.WriteLine ("Amenity name: " + name);
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(Data_Layer_Common.getPathToDatabase());

				// Get the user.
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				Amenity ame = connection.Query<Amenity>("SELECT * FROM Amenity WHERE amenityName = ?", name)[0];
				amenityId = ame.id;

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
			Console.WriteLine ("Amenity name: " + name + ", Amenity id: " + amenityId);

			return amenityId;
		}
	}
}

