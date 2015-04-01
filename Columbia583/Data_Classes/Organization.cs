using System;

namespace Columbia583
{
	public class Organization
	{
		public int id { get; set; }
		public string organization { get; set; }
		public DateTime timestamp { get; set; }

		public Organization()
		{

		}

		public Organization (int id, string organization, DateTime timestamp)
		{
			this.id = id;
			this.organization = organization;
			this.timestamp = timestamp;
		}
	}
}

