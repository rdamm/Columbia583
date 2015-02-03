using System;

namespace Columbia583
{
	public class Webservice_User
	{
		public int id { get; set; }
		public int org_id { get; set; }
		public string email { get; set; }
		public string username { get; set; }
		public string created_at { get; set; }
		public string updated_at { get; set; }
		
		public Webservice_User(int id, int org_id, string email, string username, string created_at, string updated_at)
		{
			this.id = id;
			this.org_id = org_id;
			this.email = email;
			this.username = username;
			this.created_at = created_at;
			this.updated_at = updated_at;
		}
	}
}

