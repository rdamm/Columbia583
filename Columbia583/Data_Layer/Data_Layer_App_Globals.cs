using System;
using SQLite;

namespace Columbia583
{
	public class Data_Layer_App_Globals
	{
		public Data_Layer_App_Globals ()
		{
			
		}


		/// <summary>
		/// Gets the active user's ID.
		/// </summary>
		/// <returns>The active user identifier.</returns>
		public int getActiveUserId()
		{
			int activeUserId = 0;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(Data_Layer_Common.getPathToDatabase());

				// Get the active user's ID.
				AppGlobals appGlobals = connection.Find<AppGlobals>(AppGlobals.AppGlobalsRowId);
				activeUserId = appGlobals.activeUserId;

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return activeUserId;
		}


		/// <summary>
		/// Sets the active user's ID.
		/// </summary>
		/// <param name="activeUserId">Active user identifier.</param>
		public void setActiveUserId(int activeUserId)
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(Data_Layer_Common.getPathToDatabase());

				// Get the current app globals.
				AppGlobals appGlobals = connection.Find<AppGlobals>(AppGlobals.AppGlobalsRowId);

				// Set the active user's ID.
				appGlobals.activeUserId = activeUserId;
				connection.Update(appGlobals);

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
		/// Gets the datetime of the last database update.
		/// </summary>
		/// <returns>The datetime of the last update.</returns>
		public DateTime getDatabaseLastUpdated()
		{
			DateTime databaseLastUpdated = DateTime.FromOADate(0);
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(Data_Layer_Common.getPathToDatabase());

				// Get the last time the database was updated.
				AppGlobals appGlobals = connection.Find<AppGlobals>(AppGlobals.AppGlobalsRowId);
				databaseLastUpdated = appGlobals.databaseLastUpdated;

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return databaseLastUpdated;
		}


		/// <summary>
		/// Sets the datetime of the last database update.
		/// </summary>
		/// <param name="newUpdateTime">The new datetime for the last database update.</param>
		public void setDatabaseLastUpdated(DateTime newUpdateTime)
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(Data_Layer_Common.getPathToDatabase());

				// Get the current app globals.
				AppGlobals appGlobals = connection.Find<AppGlobals>(AppGlobals.AppGlobalsRowId);

				// Set the last time the database was updated.
				appGlobals.databaseLastUpdated = newUpdateTime;
				connection.Update(appGlobals);

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

