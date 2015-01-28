using System;
using System.Collections.Generic;

namespace Columbia583
{
	public class IdTimestampComboList
	{
		public IdTimestampCombo[] activityIds { get; set; }
		public IdTimestampCombo[] amenityIds { get; set; }
		public IdTimestampCombo[] mapTileIds { get; set; }
		public IdTimestampCombo[] mediaIds { get; set; }
		public IdTimestampCombo[] organizationIds { get; set; }
		public IdTimestampCombo[] pointIds { get; set; }
		public IdTimestampCombo[] roleIds { get; set; }
		public IdTimestampCombo[] trailIds { get; set; }
		public IdTimestampCombo[] userIds { get; set; }

		public IdTimestampComboList ()
		{

		}

		public IdTimestampComboList(IdTimestampCombo[] activityIds, IdTimestampCombo[] amenityIds, IdTimestampCombo[] mapTileIds, IdTimestampCombo[] mediaIds, IdTimestampCombo[] organizationIds,
			IdTimestampCombo[] pointIds, IdTimestampCombo[] roleIds, IdTimestampCombo[] trailIds, IdTimestampCombo[] userIds)
		{
			this.activityIds = activityIds;
			this.amenityIds = amenityIds;
			this.mapTileIds = mapTileIds;
			this.mediaIds = mediaIds;
			this.organizationIds = organizationIds;
			this.pointIds = pointIds;
			this.roleIds = roleIds;
			this.trailIds = trailIds;
			this.userIds = userIds;
		}
	}
}

