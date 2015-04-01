using System;

namespace Columbia583
{
	/**
	 *	The application layer class acts as the "controller" layer.  It controls all the logic between the user interface
	 *	layer (i.e. the "view" layer), and the business layer (i.e. the "model" layer).
	*/
	public class Application_Layer_Search_Trails
	{
		public Application_Layer_Search_Trails ()
		{

		}

		public ListableTrail[] getTrailsBySearchFilter(SearchFilter searchFilter)
		{
			// Get the trails.
			Business_Layer_Search_Trails businessLayer_searchTrails = new Business_Layer_Search_Trails ();
			return businessLayer_searchTrails.getTrailsBySearchFilter (searchFilter);
		}
	}
}

