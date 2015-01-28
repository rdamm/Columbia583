using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;

namespace Columbia583
{
	public class TrailsToAmenities
	{
		// Keys and references.
		[ForeignKey(typeof(Trail))]
		public int trailId { get; set; }
		[ForeignKey(typeof(Amenity))]
		public int amenityId { get; set; }

		public TrailsToAmenities ()
		{

		}

		public TrailsToAmenities(int trailId, int amenityId)
		{
			this.trailId = trailId;
			this.amenityId = amenityId;
		}
	}
}

