using System;

namespace Columbia583
{
	/**
	 *	The data layer class performs all the SQLite queries to the local (i.e. cache) database, as well as to
	 *	any other local files, for the "model" layer.  The data layer should be called from the data access layer.
	*/
	public class Data_Layer_Search_Trails
	{
		public Data_Layer_Search_Trails ()
		{

		}

		public Trail[] getTrailsByFilters(SearchFilter searchFilter)
		{
			// TODO: Open connection to local database.

			// TODO: Query the local database.
			Trail[] trails = new Trail[0];

			// TODO: Close connection to local database.

			// Return query results.
			return trails;
		}
	}
}

