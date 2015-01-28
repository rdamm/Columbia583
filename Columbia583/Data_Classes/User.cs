using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;

namespace Columbia583
{
	public class User
	{
		// Keys and references.
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }
		[ForeignKey(typeof(Organization))]
		public int orgId { get; set; }

		// Data.
		// NOTE: The login token will be kept separate from the database so that the login information
		// of other users cannot be compromised.  The client should not have access to the passwords
		// or tokens of other users.
		public string email { get; set; }
		public string username { get; set; }
		public DateTime timestamp { get; set; }

		public User()
		{

		}

		public User (int id, int orgId, string email, string username, DateTime timestamp)
		{
			this.id = id;
			this.orgId = orgId;
			this.email = email;
			this.username = username;
			this.timestamp = timestamp;
		}
	}
}

