using System;

namespace Columbia583
{
	public class OAuthUser
	{
		public string id { get; set; }
		public string email { get; set; }
		public string first_name { get; set; }
		public string gender { get; set; }
		public string last_name { get; set; }
		public string link { get; set; }
		public string locale { get; set; }
		public string name { get; set; }
		public string timezone { get; set; }
		public string updated_time { get; set; }
		public bool verified { get; set; }

		public OAuthUser ()
		{
			
		}
	}
}

