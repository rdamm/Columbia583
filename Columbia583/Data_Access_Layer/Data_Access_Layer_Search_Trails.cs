using System;

namespace Columbia583
{
	/**
	 *	The data access layer class acts as the controlling logic for the "model" layer.  It will call the data layer
	 *	or the service access layer to perform the necessary gets / sets / inserts / deletes / etc.  The data access
	 *	layer should be called from the business layer.
	*/
	public class Data_Access_Layer_Search_Trails
	{
		public Data_Access_Layer_Search_Trails ()
		{

		}

		public Trail[] getTrailsByFilters(SearchFilter searchFilter)
		{
			// Check if cached data is available and up to date.
			// TODO: Figure out trail search caching.
			bool cachedDataAvailable = false;
			bool cachedDataUpToDate = false;

			// If cached data available and up to date, query the cached data.
			if (cachedDataAvailable == true && cachedDataUpToDate == true) {
				Data_Layer_Search_Trails dataLayer_searchTrails = new Data_Layer_Search_Trails ();
				return dataLayer_searchTrails.getTrailsByFilters (searchFilter);
			}
			// Otherwise, check if webservices are available.  If so, query the webservices.
			else {
				// TODO: Check if webservices are available.
				bool webservicesAvailable = false;

				// If webservices are available, query the webservices.
				if (webservicesAvailable == true) {
					// Query the webservices.
					Service_Access_Layer_Search_Trails serviceAccessLayer_searchTrails = new Service_Access_Layer_Search_Trails ();
					Trail[] trails = serviceAccessLayer_searchTrails.getTrailsByFilters (searchFilter);

					// If cached data available, update the cached data.
					if (cachedDataAvailable == true) {
						// TODO: Update the cached data.
					}

					return trails;
				}
				// Otherwise, return a flag to indicate that no data was gathered.
				else {
					return null;
				}
			}
		}
	}
}

