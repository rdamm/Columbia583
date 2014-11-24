using System;

namespace Columbia583
{
	/**
	 *	The service access layer class performs all the webservice calls for the "model" layer.  The service access
	 *	layer should be called from the data access layer.
	*/
	public class Service_Access_Layer_Search_Trails
	{
		public Service_Access_Layer_Search_Trails ()
		{

		}

		public Trail[] getTrailsByFilters(SearchFilter searchFilter)
		{
			// TODO: Instantiate SOAP library.

			// TODO: Query the webservices.
			Trail[] trails = new Trail[0];

			// Return query results.
			return trails;
		}
	}
}

