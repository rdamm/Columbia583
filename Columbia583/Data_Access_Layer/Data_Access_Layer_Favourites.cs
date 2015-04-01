using System;

namespace Columbia583
{
	public class Data_Access_Layer_Favourites
	{
		public Data_Access_Layer_Favourites ()
		{
			
		}


		/// <summary>
		/// Gets the user's favourite trails.
		/// </summary>
		/// <returns>The favourite trails.</returns>
		/// <param name="userId">User identifier.</param>
		public ListableTrail[] getFavouriteTrails(int userId)
		{
			Data_Layer_Favourites dataLayerFavourites = new Data_Layer_Favourites ();
			return dataLayerFavourites.getFavouriteTrails (userId).ToArray();
		}


		/// <summary>
		/// Adds the trail to the user's favourites.
		/// </summary>
		/// <param name="userId">User identifier.</param>
		/// <param name="trailId">Trail identifier.</param>
		public void addFavouriteTrail(int userId, int trailId)
		{
			Data_Layer_Favourites dataLayerFavourites = new Data_Layer_Favourites ();
			dataLayerFavourites.addFavouriteTrail (userId, trailId);
		}


		/// <summary>
		/// Removes the trail from the user's favourites.
		/// </summary>
		/// <param name="userId">User identifier.</param>
		/// <param name="trailId">Trail identifier.</param>
		public void removeFavouriteTrail(int userId, int trailId)
		{
			Data_Layer_Favourites dataLayerFavourites = new Data_Layer_Favourites ();
			dataLayerFavourites.removeFavouriteTrail (userId, trailId);
		}
	}
}

