using System;

namespace Columbia583
{
	public class Webservice_Activity
	{
		public int id { get; set; }
		public string name { get; set; }
		public string icon { get; set; }
		public string button { get; set; }
		public string mini { get; set; }
		public string marker { get; set; }
		public string created_at { get; set; }
		public string updated_at { get; set; }

		public Webservice_Activity(int id, string name, string icon, string button, string mini, string marker, string created_at, string updated_at)
		{
			this.id = id;
			this.name = name;
			this.icon = icon;
			this.created_at = created_at;
			this.updated_at = updated_at;
			this.marker = marker;
			this.mini = mini;
			this.button = button;
		}
	}
}

