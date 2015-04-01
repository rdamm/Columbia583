using System;

namespace Columbia583
{
	/// <summary>
	/// Search trails data access layer will control all requests relating to the search trails page.
	/// </summary>
	public class Data_Access_Layer_Search_Trails
	{
		public Data_Access_Layer_Search_Trails ()
		{

		}

		
		/// <summary>
		/// Gets the trails by search filter.
		/// </summary>
		/// <returns>The trails by search filter.</returns>
		/// <param name="searchFilter">Search filter.</param>
		public ListableTrail[] getTrailsBySearchFilter(SearchFilter searchFilter)
		{
			Data_Layer_Search_Trails dataLayer = new Data_Layer_Search_Trails();
			return dataLayer.getTrailsBySearchFilter (searchFilter).ToArray ();
		}
	}
}

