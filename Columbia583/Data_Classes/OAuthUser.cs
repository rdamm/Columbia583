using System;

namespace Columbia583
{
	public class OAuthUser
	{
		public string id { get; set; }
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
			//{"id":"10152618837852315","first_name":"Ryan","gender":"male","last_name":"Damm","link":"https:\/\/www.facebook.com\/app_scoped_user_id\/10152618837852315\/","locale":"en_US","name":"Ryan Damm","timezone":-6,"updated_time":"2015-03-23T22:41:59+0000","verified":true}
		}
	}
}

