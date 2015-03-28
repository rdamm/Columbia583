using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Columbia583
{
	/// <summary>
	/// The view trail data layer performs all the SQLite queries relating to the view trail page.
	/// </summary>
	public class Data_Layer_View_Trail
	{
		public Data_Layer_View_Trail ()
		{

		}


		/// <summary>
		/// Gets the trail.
		/// </summary>
		/// <returns>The trail.</returns>
		/// <param name="trailId">Trail identifier.</param>
		public Trail getTrail(int trailId)
		{
			Trail trail = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(Data_Layer_Common.getPathToDatabase());

				// Get the trail.
				// TODO: Get the other necessary trail information (eg. Media)
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				trail = connection.Find<Trail>(trailId);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return trail;
		}


		/// <summary>
		/// Gets the media for the given trail.
		/// </summary>
		/// <returns>The media.</returns>
		/// <param name="trailId">Trail identifier.</param>
		public List<Media> getMedia(int trailId)
		{
			List<Media> mediaList = new List<Media> ();
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(Data_Layer_Common.getPathToDatabase());

				// Get the media for this trail.
				var response = connection.Query<Media>("SELECT * FROM Media WHERE trailId = ?", trailId);
				foreach (Media media in response)
				{
					mediaList.Add(media);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return mediaList;
		}


		public List<Comment> getComments(int trailId)
		{
			List<Comment> results = new List<Comment> ();
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(Data_Layer_Common.getPathToDatabase());

				var response = connection.Query<Comment>("SELECT * FROM Comment WHERE trailId = ?", trailId);
				foreach (Comment c in response)
				{
					results.Add(c);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
			return results;
		}
		/*
		public User getUserForComment(Comment c)
		{
			User user = null;

			try {
				// Open connection to local database.
				var connection = new SQLiteConnection (Data_Layer_Common.getPathToDatabase ());

				user = connection.Find<User>(c.userId);
				//user = connection.Query<User>("SELECT * FROM User WHERE id = ?", c.userId)[0];

				connection.Close();
			} catch (SQLiteException ex) {
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return user;
		}
		*/
	}
}

