using System;

namespace Columbia583
{
	public class Webservice_Organization
	{
		public int id { get; set; }
		public string organization { get; set; }
		public string created_at { get; set; }
		public string updated_at { get; set; }

		public Webservice_Organization(int id, string organization, string created_at, string updated_at)
		{
			this.id = id;
			this.organization = organization;
			this.created_at = created_at;
			this.updated_at = updated_at;
		}
	}
}

