using System;

namespace Columbia583
{
	public class Webservice_Amenity
	{
		public int id { get; set; }
		public string name { get; set; }
		public string icon { get; set; }
		public string created_at { get; set; }
		public string updated_at { get; set; }

		public Webservice_Amenity(int id, string name, string icon, string created_at, string updated_at)
		{
			this.id = id;
			this.name = name;
			this.icon = icon;
			this.created_at = created_at;
			this.updated_at = updated_at;
		}
	}
}

