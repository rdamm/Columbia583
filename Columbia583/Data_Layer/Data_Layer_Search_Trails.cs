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


		// TODO: Finish the activity and amenity search parameters.


		/// <summary>
		/// Gets the trails by search filter.
		/// </summary>
		/// <returns>The trails by search filter.</returns>
		/// <param name="searchFilter">Search filter.</param>
		public List<SearchResult> getTrailsBySearchFilter(SearchFilter searchFilter)
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

			List<SearchResult> searchResults = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(Data_Layer_Common.getPathToDatabase());

				// Build the search parameter lines and keep track of their parameters.
				List<string> lines = new List<string>();
				List<object> parameters = new List<object>();
				// id IN (SELECT * FROM Trail INNER JOIN TrailsToActivities ON Trail.id = TrailsToActivities.trailId WHERE (activityId = ? OR activityId = ?)), activity, nextActivity
				if (searchFilter.activities != null && searchFilter.activities.Length > 0)
				{
					string activityLine = "(id IN (SELECT id FROM Trail INNER JOIN TrailsToActivities ON Trail.id = TrailsToActivities.trailId WHERE (";
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
							// Greenways website has " AND ", though--which should I follow?
							activityLine += " OR ";
						}
						activityLine += "activityId = ?";
						parameters.Add(activity);
					}
					activityLine += ")))";
					// TODO: Add the line and its parameters to the lists.
					lines.Add(activityLine);
				}
				if (searchFilter.amenities != null && searchFilter.amenities.Length > 0)
				{
					string amenityLine = "(id IN (SELECT id FROM Trail INNER JOIN TrailsToAmenities ON Trail.id = TrailsToAmenities.trailId WHERE (";
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
							amenityLine += " OR ";
						}
						amenityLine += "amenityId = ?";
						parameters.Add(amenity);
					}
					amenityLine += ")))";
					// TODO: Add the line and its parameters to the lists.
					lines.Add(amenityLine);
				}
				if (searchFilter.difficulty != null && searchFilter.difficulty.Length > 0)
				{
					string difficultyLine = "(";
					bool firstDiff = true;
					foreach(Difficulty d in searchFilter.difficulty)
					{
						if (firstDiff)
						{
							firstDiff = false;
						}
						else
						{
							difficultyLine += " OR ";
						}
						difficultyLine += "difficulty = ?";
						parameters.Add((int)d);
					}
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
				var response = connection.Query<Trail>("SELECT * FROM Trail " + whereQuery, parameters.ToArray());

				// For each matching trail, get its points, activities, and amenities.
				searchResults = new List<SearchResult>();
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
					SearchResult searchResult = new SearchResult(trailRow, points.ToArray(), activities.ToArray(), amenities.ToArray());
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
	}
}

