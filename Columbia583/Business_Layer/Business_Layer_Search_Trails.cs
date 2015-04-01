using System;

namespace Columbia583
{
	/**
	 *	The business layer class acts as the entry point for the "model" layer.  It will call the data access layer
	 *	to perform the necessary gets / sets / inserts / deletes / etc.  As such, this should be the interface for
	 *	the rest of the "model" layer (business layer -> data access layer -> data layer / service access layer).
	*/
	public class Business_Layer_Search_Trails
	{
		public Business_Layer_Search_Trails ()
		{

		}

		public ListableTrail[] getTrailsBySearchFilter(SearchFilter searchFilter)
		{
			// Get the trails.
			Data_Access_Layer_Search_Trails dataAccessLayer_searchTrails = new Data_Access_Layer_Search_Trails ();
			return dataAccessLayer_searchTrails.getTrailsBySearchFilter (searchFilter);
		}
	}
}

