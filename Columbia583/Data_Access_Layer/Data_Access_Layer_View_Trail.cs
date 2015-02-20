using System;

namespace Columbia583
{
	/// <summary>
	/// View trail data access layer will control all requests relating to the view trail page.
	/// </summary>
	public class Data_Access_Layer_View_Trail
	{
		public Data_Access_Layer_View_Trail ()
		{

		}


		/// <summary>
		/// Gets the trail.
		/// </summary>
		/// <returns>The trail.</returns>
		/// <param name="trailId">Trail identifier.</param>
		public Trail getTrail(int trailId)
		{
			Data_Layer_View_Trail dataLayer = new Data_Layer_View_Trail();
			return dataLayer.getTrail (trailId);
		}

		public Comment[] getComments(int trailId)
		{
			Data_Layer_View_Trail dataLayer = new Data_Layer_View_Trail();
			return dataLayer.getComments (trailId).ToArray();
		}
	}
}

