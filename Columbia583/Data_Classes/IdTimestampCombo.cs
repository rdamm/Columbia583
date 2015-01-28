using System;

namespace Columbia583
{
	public class IdTimestampCombo
	{
		public int id { get; set; }
		public DateTime timestamp { get; set; }

		public IdTimestampCombo ()
		{

		}

		public IdTimestampCombo (int id, DateTime timestamp)
		{
			this.id = id;
			this.timestamp = timestamp;
		}
	}
}

