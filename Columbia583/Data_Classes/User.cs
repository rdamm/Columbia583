using System;

namespace Columbia583
{
	public class User
	{
		public int id { get; set; }
		public int orgId { get; set; }
		public string email { get; set; }
		public string username { get; set; }
		public string loginToken { get; set; }
		public DateTime timestamp { get; set; }

		// NOTE: The login token is kept on the client's side to maintain that
		// user's login session.  The client should not have access to the
		// passwords or tokens of other users.

		public User()
		{

		}

		public User (int id, int orgId, string email, string username, string loginToken, DateTime timestamp)
		{
			this.id = id;
			this.orgId = orgId;
			this.email = email;
			this.username = username;
			this.loginToken = loginToken;
			this.timestamp = timestamp;
		}
	}
}

