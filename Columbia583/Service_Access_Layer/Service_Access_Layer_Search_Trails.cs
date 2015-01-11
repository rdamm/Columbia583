using System;
using System.Net;

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
			string searchurl = "http://trails.greenways.ca/api/v1/GetAll";
			HttpWebRequest httpreq = (HttpWebRequest)HttpWebRequest.Create (new Uri (searchurl));

			// TODO: Query the webservices.
			HttpWebResponse resp = (HttpWebResponse)httpreq.GetResponse ();
			if (resp.StatusCode != HttpStatusCode.OK) {
				// Not OK
			}
			Trail[] trails = new Trail[0];

			// Return query results.
			return trails;
		}
	}
}

