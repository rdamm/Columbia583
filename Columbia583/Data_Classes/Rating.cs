using System;

namespace Columbia583
{
	public class Rating
	{
		private int ratingNumber;

		public Rating (int ratingNum)
		{
			this.ratingNumber = ratingNum;
		}

		public int RatingNumber {
			get {
				return ratingNumber;
			}
			set {
				ratingNumber = value;
			}
		}

		// Rating as # of stars--useful for displayed Trail
		public string RatingStars {
			get {
				string ratingStars = "";
				for (int i = 0; i < ratingNumber; i++) {
					ratingStars += "*";
				}
				return ratingStars;
			}
		}
	}
}

