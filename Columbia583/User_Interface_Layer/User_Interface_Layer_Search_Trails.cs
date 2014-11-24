using System;

namespace Columbia583
{
	/**
	 *	The user interface layer class acts as the "view" layer.  It controls all the event handlers for the GUI.
	*/
	public class User_Interface_Layer_Search_Trails
	{
		public User_Interface_Layer_Search_Trails ()
		{

		}

		public void getTrailsByFilters()
		{
			// TODO: Get the search filter parameters from the view.
			Activity[] activities = new Activity[0];
			Difficulty[] difficulties = new Difficulty[0];
			int minDuration = 0;
			int maxDuration = 8;
			int minDistance = 0;
			int maxDistance = 100;

			// Encapsulate the filter parameters.
			SearchFilter searchFilter = new SearchFilter(activities, difficulties, minDuration, maxDuration, minDistance, maxDistance);

			// Get the trails.
			Application_Layer_Search_Trails applicationLayer_searchTrails = new Application_Layer_Search_Trails ();
			Trail[] trails = applicationLayer_searchTrails.getTrailsByFilters (searchFilter);

			// TODO: Display the trails in the view.
		}
	}
}

