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


		/// <summary>
		/// Gets the media for the given trail.
		/// </summary>
		/// <returns>The media.</returns>
		/// <param name="trailId">Trail identifier.</param>
		public Media[] getMedia(int trailId)
		{
			Data_Layer_View_Trail dataLayer = new Data_Layer_View_Trail ();
			return dataLayer.getMedia (trailId).ToArray();
		}


		public Comment[] getComments(int trailId)
		{
			Data_Layer_View_Trail dataLayer = new Data_Layer_View_Trail();
			return dataLayer.getComments (trailId).ToArray();
		}
		/*
		public User getUserForComment(Comment c)
		{
			Data_Layer_View_Trail dataLayer = new Data_Layer_View_Trail();
			return dataLayer.getUserForComment(c);
		}
		*/
	}
}

